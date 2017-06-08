using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nutadore
{
	public class LilyPond
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
						(
							(?<Open>{)
							[^{}]*
						)+
						(
							(?<Close-Open>})
							[^{}]*
						)+
					)*
					(?(Open)(?!))
				)
			\}");

		// << ... >>
		static Regex reAngleBrackets = new Regex(@"(?sx)
			<<
				(?'angleBrackets'
					.*?(?=<<|>>)
					(
						(
							(?'Open'<<)
							.*?(?=<<|>>)
						)+
						(
							(?'Close-Open'>>)
							.*?(?=<<|>>)
						)+
					)*
					(?(Open)(?!))
				)
			");

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

		// \score {...}
		static Regex reScore = new Regex($@"(?x)
			\\score
			\s*
			{reCurlyBrackets}");

		// \PianoStaff <<...>>
		static Regex rePianoStaff = new Regex($@"(?x)
			\\new
			\s*
			PianoStaff
			\s*
			{reAngleBrackets}");

		// \Staff <<...>>
		static Regex reStaff = new Regex($@"(?x)
			\\new
			\s*
			Staff
			\s*
			{reAngleBrackets}");

		//  \key d \minor
		static Regex reKey = new Regex($@"(?x)
			\\key
			\s*
			(?<letter>[cdefgab])
			\s*
			\\(?<type>major|minor)
			");
		#endregion

		private static Duration[] lastDuration;

		public static void Parse(string fileName, Score score)
		{
			string lyText = File.ReadAllText(fileName);

			// Wczytaj glosy zapisane jako \parallelMusic
			List<Sign>[] parallelMusic = ParallelMusic(lyText);

			// Znajdź najkrótszą nutę lub pauzę w utworzne.
			Duration.Name shortestDurationName = parallelMusic.Min(voice =>
				voice
					.Select(sign => sign as IDuration)
					.Where(signd => signd != null)
					.DefaultIfEmpty()
					.Min(signd => signd?.duration.name ?? Duration.Name.Whole));

			// Znajdź najkrótszą nutę lub pauzę z kropką.
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

			// Wczytaj \score
			Score(lyText, score);
		}

		private static List<Sign>[] ParallelMusic(string lyText)
		{
			List<Sign>[] voices = null;

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
			Note.Letter letter = GetLetter(mNote.Groups["letter"].Value);
			Accidental.Type accidentalType = GetAccidentalType(mNote.Groups["accidental"].Value);

			Note.Octave octave = GetOctave(mNote.Groups["octave"].Value);

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

		private static Note.Letter GetLetter(string s)
		{
			switch (s)
			{
				case "c": return Note.Letter.C;
				case "d": return Note.Letter.D;
				case "e": return Note.Letter.E;
				case "f": return Note.Letter.F;
				case "g": return Note.Letter.G;
				case "a": return Note.Letter.A;
				case "b": return Note.Letter.B;
				default:
					throw new ArgumentOutOfRangeException("s", s);
			}
		}

		private static Accidental.Type GetAccidentalType(string s)
		{
			switch (s)
			{
				case "is": return Accidental.Type.Sharp;
				case "es": return Accidental.Type.Flat;
				case "": return Accidental.Type.None;
				default:
					throw new ArgumentOutOfRangeException("accidental", s);
			}
		}

		private static Note.Octave GetOctave(string s)
		{
			switch (s)
			{
				case ",,,": return Note.Octave.SubContra;
				case ",,": return Note.Octave.Contra;
				case ",": return Note.Octave.Great;
				case "": return Note.Octave.Small;
				case "'": return Note.Octave.OneLined;
				case "''": return Note.Octave.TwoLined;
				case "'''": return Note.Octave.ThreeLined;
				case "''''": return Note.Octave.FourLined;
				case "'''''": return Note.Octave.FiveLined;
				default:
					throw new ArgumentOutOfRangeException("octave", s);
			}
		}

		private static Scale.Type GetScaleType(string s)
		{
			switch (s)
			{
				case "major": return Scale.Type.Major;
				case "minor": return Scale.Type.Minor;
				default:
					throw new ArgumentOutOfRangeException("s", s);
			}
		}

		private static void Score(string lyText, Score score)
		{
			// Wczytanie \score {...}
			Match mScore = reScore.Match(lyText);
			string scoreCB = mScore.Groups["curlyBrackets"].Value;

			// Wczytanie \new PianoStaff
			Match pianoStaff = rePianoStaff.Match(scoreCB);
			string pianoStaffAB = pianoStaff.Groups["angleBrackets"].Value;

			// Wczytanie \Staff
			foreach (Match staff in reStaff.Matches(pianoStaffAB))
			{
				string staffAB = staff.Groups["angleBrackets"].Value;
				Match key = reKey.Match(staffAB);
				string sLetter = key.Groups["letter"].Value;
				string sScale = key.Groups["type"].Value;

				Note.Letter letter = GetLetter(sLetter);
				Scale.Type type = GetScaleType(sScale);
				score.scale = new Scale(letter, type);
			}
		}
	}
}