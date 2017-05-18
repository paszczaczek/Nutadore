using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nutadore
{
	internal class LyParser
	{
		/*
		 % !!!
		% Nutadore nie uwzglednia dlugosci nuty z poprzednej nuty, np. dla c8 d e wychodzi c*8 d*4 e*4
		% zaimplementowac !
		% !!!
		*/

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
				{reBar}
			)");
		#endregion

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
				for (int v = 0; v < voices.Count(); v++)
					voices[v] = new List<Sign>();

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

								chord.Add(GetNote(noteInChord));
							}
							chord.duration = GetDuration(mChord);
							voices[measureNo % 3].Add(chord);
							continue;
						}

						// Czy to jest nuta?
						Match note = reNote.Match(sign.Value);
						if (note.Success)
						{
							if (debug)
								Console.WriteLine($"\tnote: {note}");
							voices[measureNo % 3].Add(GetNote(note));
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
	
		private static Note GetNote(Match mNote)
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

			Duration duration = GetDuration(mNote);

			Note note = new Note(letter, accidentalType, octave, duration);
			int finger;
			if (int.TryParse(mNote.Groups["finger"].Value, out finger))
				note.finger = finger;

			return note;
		}

		static private Duration GetDuration(Match durable)
		{
			Duration duration = new Duration();
			switch (durable.Groups["duration"].Value)
			{
				case "1": duration.value = Duration.Value.Whole; break;
				case "2": duration.value = Duration.Value.Half; break;
				case "":
				case "4": duration.value = Duration.Value.Quarter; break;
				case "8": duration.value = Duration.Value.Eighth; break;
				case "16": duration.value = Duration.Value.Sixteenth; break;
				case "32": duration.value = Duration.Value.ThirtySecond; break;
				default:
					throw new ArgumentOutOfRangeException("duration");
			}
			if (durable.Groups["dotted"].Value == ".")
				duration.dotted = true;

			return duration;
		}
	}
}