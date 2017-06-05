using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nutadore
{
	internal class LyParser
	{
		static private bool debug = false;

		#region regex
		// # ...
		static Regex reLineComment = new Regex(@"(?mx)
			%.*
			\r*
			$");

		// %{ ... %}
		static Regex reBlockComment = new Regex(@"(?sx)
			%\{
			.*?
			%\}");

		// { ... }
		static Regex reCurlyBrackets = new Regex(@"(?x)
			\{
				(?<curlyBrackets>
					[^{]*
					(
						((?<Open>{)[^{}]*)+
						((?<Close-Open>})[^{}]*)+
					)*
					(?(Open)(?!))
				)
			\}");

		// \parallelMusic #'(voiceA voiceB voiceC) {...}
		static Regex reParallelMusic = new Regex($@"(?x)
			\\parallelMusic
			\s*
			\#'
			\(
				((?<voicesNames>\w+)\s*)+
			\)
			\s*
			{reCurlyBrackets}");

		// ... |
		static Regex reMeasure = new Regex(@"(?x)
			(
				[^|]+
				\|?
			)");

		// -\markup{c2}
		static Regex reMarkup = new Regex(@"(?x)
			(
				[-_^]
				\\markup
				\{
					(?<markup>.*?)
				\}
			)
		");

		// cis
		static Regex reLetter = new Regex(@"(?x)
			(
				(?<!\\) # \grace to nie g
				\b
				(?<letter>[a-g])
				(?<accidental>is|es|)
			)");

		// ''' lub ,,
		static Regex reOctave = new Regex(@"(?x)
			(?<octave>
				'{1,4} |
				,{1,3}
			)");

		// -5
		static Regex reFinger = new Regex(@"(?x)
			(
				-
				(?<finger>[1-9])
			)");

		// 4.
		static Regex reDuration = new Regex(@"(?x)
			(
				(?<duration>1|2|4|8|16|32)
				(?<dotted>\.?)
			)");

		// ~
		static Regex reTie = new Regex(@"(?x)
			(?<tie>~)
		");

		// fis''-2~-\markup{fis2}
		static Regex reNote = new Regex($@"(?x)
			(
				{reLetter}
				{reOctave}?
				{reDuration}?
				{reFinger}?
				{reMarkup}?
			)");

		// <fis''-2 ces,-3>~-\markup{fis2}
		static Regex reChord = new Regex($@"(?x)
			(
				<
					(?<notes>.*?)
				>
				{reDuration}?
				{reTie}?
				{reMarkup}?
			)");

		// r4
		static Regex reRest = new Regex($@"(?x)
			(
				r
				{reDuration}?
			)");

		// |
		static Regex reBar = new Regex(@"(?x)
			\|");

		// fis''-2
		static Regex reNoteInChord = new Regex($@"(?x)
			(
				{reLetter}
				{reOctave}?
				{reFinger}?
			)");

		static Regex reSign = new Regex($@"(?x)
			(
				{reChord} |
				{reNote} |
				{reRest} |
				{reBar}
			)");
		#endregion

		private static Duration[] lastDuration;

		internal static void Load(Score score, string fileName)
		{
			// Wczytaj glosy zapisane jako \parallelMusic
			List<Sign>[] parallelMusic = ParallelMusic(fileName);

			// Znajdź najkrótszą nutę lub pauzę w utworzne.
			Duration.Name shortestDurationName = parallelMusic.Min(voice =>
				voice
					.Select(sign => sign as IDuration)
					.Where(signd => signd != null)
					.DefaultIfEmpty()
					.Min(signd => signd?.duration.name ?? Duration.Name.Whole));

			// Znajdź najktótszą nutę lub pauzę z kropką.
			Duration.Name shortestDottedDurationName = parallelMusic.Min(voice =>
				voice
					.Select(sign => sign as IDuration)
					.Where(signd => signd != null && signd.duration.dotted)
					.DefaultIfEmpty()
					.Min(signd => signd?.duration.name ?? Duration.Name.Whole)
			);

			// Wyznacz najmniejszą jednostkę rytmiczną.
			if (shortestDurationName == shortestDottedDurationName)
				shortestDurationName--;
			Duration tickDuration = new Duration(shortestDurationName);

			// Rozmieść nuty i pauzy w tablicy rytmicznej (wiersze to glosy, kolumny to jednostki rytmiczne).
			List<Sign>[] voiceTable = new List<Sign>[parallelMusic.Count()];
			for (int voiceNo = 0; voiceNo < parallelMusic.Count(); voiceNo++)
			{
				voiceTable[voiceNo] = new List<Sign>();
				foreach (Sign sign in parallelMusic[voiceNo])
				{
					IDuration signd = sign as IDuration;
					if (signd != null)
					{
						int count = signd.duration.ContainsHowMuch(tickDuration);
						voiceTable[voiceNo].Add(sign);
						for (int i = 1; i < count; i++)
							voiceTable[voiceNo].Add(null);
					}
				}
			}

			// Sprawdź czy długość wszystkich głosów jest równa.
			int minCount = voiceTable.Min(voice => voice.Count());
			int maxCount = voiceTable.Max(voice => voice.Count());
			//if (minCount != maxCount)
			//	throw new Exception("Długości rytmiczne głosów nie są równe.");

			// Na ile liczy się takt? (time signature)
			int numerator = 4;
			Duration denominator = new Duration(Duration.Name.Quarter);
			double measureCount = numerator * denominator.Count();

			// Przeglądamy tablicę rytmiczną 
			double musicCount = 0;
			for (int v = 0; v < maxCount; v++)
			{
				// Liczymy wszystkie jednostki rytmiczne, żeby móc dzielić na takty.
				musicCount += tickDuration.Count();

				// Czy jednostka rytmiczna zawiera przynajmniej jedną nutę lub pauzę?
				if (!voiceTable.All(voice => v >= voice.Count() || voice.ElementAt(v) == null))
				{
					// Zawiera, Dodajemy do kroku.
					Step step = new Step();
					foreach (List<Sign> voice in voiceTable)
						if (v < voice.Count())
							if (voice.ElementAt(v) != null)
								step.AddVoice(voice.ElementAt(v));
					score.Add(step);
				}

				// Może tu powinien być koniec taktu?
				if (musicCount % measureCount == 0)
					score.Add(new Step().AddVoice(new Bar()));
			}
		}

		static public List<Sign>[] ParallelMusic(string lyFileName)
		{
			List<Sign>[] voices = null;

			string lyText = File.ReadAllText(lyFileName);

			// Usuwanie komentarzy.
			lyText = reBlockComment.Replace(lyText, "");
			lyText = reLineComment.Replace(lyText, "");

			// Wczytanie parallelMusic.
			Match parallelMusic = reParallelMusic.Match(lyText);
			if (parallelMusic.Success)
			{
				// Liczba głosów w parellelMusic.
				CaptureCollection voicesNames = parallelMusic.Groups["voicesNames"].Captures;
				voices = new List<Sign>[voicesNames.Count];
				lastDuration = new Duration[voices.Count()];
				for (int v = 0; v < voices.Count(); v++)
				{
					voices[v] = new List<Sign>();
					lastDuration[v] = new Duration();
				}


				// Zawartość nawisaów klamrowych w parellelMusic.
				string curlyBrackets = parallelMusic.Groups["curlyBrackets"].Value;
				if (debug)
				{
					Console.Write("\\parallelMusic #'(");
					foreach (Capture voiceName in voicesNames)
						Console.Write($"{voiceName} ");
					Console.WriteLine($") {{\n{curlyBrackets}}}");
				}

				// Wczytywanie kolejnych taktów w parallelMusic.
				int measureNo = -1;
				foreach (Match measure in reMeasure.Matches(curlyBrackets))
				{
					measureNo++;
					if (debug)
						Console.WriteLine("measure {0}: {1}",
							measureNo,
							Regex.Replace(measure.ToString().Trim(), @"\r+|\n+", ""));

					// Wczytanie kolejnych znaków w takcie.
					foreach (Match sign in reSign.Matches(measure.Value))
					{
						int voiceNo = measureNo % 3;

						// Czy to jest akord?
						Match mChord = reChord.Match(sign.Value);
						if (mChord.Success)
						{
							if (debug)
								Console.WriteLine($"\tchord: {mChord}");
							Chord chord = new Chord();
							foreach (Match noteInChord in reNoteInChord.Matches(mChord.Groups["notes"].Value))
							{
								if (debug)
									Console.WriteLine($"\t\tnote: {noteInChord}");

								chord.AddNote(GetNote(noteInChord, voiceNo));
							}
							chord.duration = GetDuration(mChord, voiceNo);
							voices[voiceNo].Add(chord);
							continue;
						}

						// Czy to jest nuta?
						Match note = reNote.Match(sign.Value);
						if (note.Success)
						{
							if (debug)
								Console.WriteLine($"\tnote: {note}");
							voices[voiceNo].Add(GetNote(note, voiceNo));
							continue;
						}

						// Czy to jest pauza?
						Match rest = reRest.Match(sign.Value);
						if (rest.Success)
						{
							if (debug)
								Console.WriteLine($"\rest: {rest}");
							voices[measureNo % 3].Add(GetRest(rest, voiceNo));
							continue;
						}

						// Czy to jest koniec taktu?
						if (reBar.IsMatch(sign.Value))
						{
							if (debug)
								Console.WriteLine("\tbar");
							voices[measureNo % 3].Add(new Bar());
							continue;
						}
					}
				}
			}

			return voices;
		}

		private static Note GetNote(Match mNote, int voiceNo)
		{
			Note.Letter letter;
			switch (mNote.Groups["letter"].Value)
			{
				case "c": letter = Note.Letter.C; break;
				case "d": letter = Note.Letter.D; break;
				case "e": letter = Note.Letter.E; break;
				case "f": letter = Note.Letter.F; break;
				case "g": letter = Note.Letter.G; break;
				case "a": letter = Note.Letter.A; break;
				case "b":
				case "h": letter = Note.Letter.H; break;
				default:
					throw new ArgumentOutOfRangeException("letter");
			}

			Accidental.Type accidentalType;
			switch (mNote.Groups["accidental"].Value)
			{
				case "is": accidentalType = Accidental.Type.Sharp; break;
				case "es": accidentalType = Accidental.Type.Flat; break;
				case "": accidentalType = Accidental.Type.None; break;
				default:
					throw new ArgumentOutOfRangeException("accidental");
			}

			Note.Octave octave;
			switch (mNote.Groups["octave"].Value)
			{
				case ",,,": octave = Note.Octave.SubContra; break;
				case ",,": octave = Note.Octave.Contra; break;
				case ",": octave = Note.Octave.Great; break;
				case "": octave = Note.Octave.Small; break;
				case "'": octave = Note.Octave.OneLined; break;
				case "''": octave = Note.Octave.TwoLined; break;
				case "'''": octave = Note.Octave.ThreeLined; break;
				case "''''": octave = Note.Octave.FourLined; break;
				case "'''''": octave = Note.Octave.FiveLined; break;
				default:
					throw new ArgumentOutOfRangeException("octave");
			}

			Duration duration = GetDuration(mNote, voiceNo);

			Note note = new Note(letter, accidentalType, octave, duration);
			int finger;
			if (int.TryParse(mNote.Groups["finger"].Value, out finger))
				note.finger = finger;

			return note;
		}

		private static Duration GetDuration(Match durable, int voiceNo)
		{
			Duration duration = new Duration();
			switch (durable.Groups["duration"].Value)
			{
				case "1": duration.name = Duration.Name.Whole; break;
				case "2": duration.name = Duration.Name.Half; break;
				case "4": duration.name = Duration.Name.Quarter; break;
				case "8": duration.name = Duration.Name.Eighth; break;
				case "16": duration.name = Duration.Name.Sixteenth; break;
				case "32": duration.name = Duration.Name.ThirtySecond; break;
				case "":
					duration = lastDuration[voiceNo]; break;
				default:
					throw new ArgumentOutOfRangeException("duration");
			}
			if (durable.Groups["dotted"].Value == ".")
				duration.dotted = true;

			lastDuration[voiceNo] = duration;

			return duration;
		}

		private static Rest GetRest(Match mRest, int voiceNo)
		{
			Duration duration = GetDuration(mRest, voiceNo);

			Rest rest = new Rest(duration);

			return rest;
		}

	}
}