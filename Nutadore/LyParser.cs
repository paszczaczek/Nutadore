using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nutadore
{
	internal class LyParser
	{
		static private bool debug = true;

		#region regex
		// # ...
		static Regex reComment = new Regex(@"(?mx)
			%.*
			\r*
			$");

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

		// ... | ... | ...
		static Regex reMeasures = new Regex(@"(?x)
			(
				(?<measures>[^|]+)
				\|?
			)+");

		// -\markup{c2}
		static Regex reMarkup = new Regex(@"(?x)
			(
				[-_^]
				\\markup
				\{
					(?<markup>[^{}]*)
				\}
			)
		");

		// cis
		static Regex reLetter = new Regex(@"(?x)
			(
				(?<letter>[a-g])
				(?<accidental>is|es|)
			)");

		// '''
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

		// 4
		static Regex reDuration = new Regex(@"(?x)
			(?<duration>1|2|4|8|16)
		");

		// ~
		static Regex reTie = new Regex(@"(?x)
			(?<tie>~)
		");

		// fis''-2~-\markup{fis2}
		static Regex reNote = new Regex($@"(?x)
			(?<note>
				{reLetter}
				{reOctave}?
				{reDuration}?
				{reFinger}?
				{reMarkup}?
			)");

		// <fis''-2 ces,-3>~-\markup{fis2}
		static Regex reChord = new Regex($@"(?x)
			(?<chord>
				<
					[^<>]*
				>
				{reDuration}?
				{reTie}?
				{reMarkup}?
			)");

		// fis''-2 c des,
		static Regex reNotesInChord = new Regex($@"(?x)
			(
				\s*
				(?<notesInChord>
					{reLetter}
					{reOctave}?
					{reFinger}?
				)
			)+");

		// \grace
		static Regex reGrace = new Regex(@"(?x)
			\\grace");

		// <fis''-2 ces,-3>~-\markup{fis2} d'^\markup{b} r4 ...
		static Regex reSigns = new Regex($@"(?x)
			(
				\s*
				(
					(?<signs>{reChord}|{reNote})
					| {reGrace}
				)
			)+");
		#endregion

		static public List<Sign>[] ParallelMusic(string lyFileName)
		{
			List<Sign>[] voices = null;

			string lyText = File.ReadAllText(lyFileName);

			// % ...
			lyText = reComment.Replace(lyText, "\r");

			// \parallelMusic #'(voiceA voiceB voiceC) {...}
			Match parallelMusic = reParallelMusic.Match(lyText);
			if (parallelMusic.Success)
			{
				CaptureCollection voicesNames = parallelMusic.Groups["voicesNames"].Captures;
				voices = new List<Sign>[voicesNames.Count];
				for (int v = 0; v < voices.Count(); v++)
					voices[v] = new List<Sign>();
				string curlyBrackets = parallelMusic.Groups["curlyBrackets"].Value;
				if (debug)
				{
					Console.Write("\\parallelMusic #'(");
					foreach (Capture voiceName in voicesNames)
						Console.Write($"{voiceName} ");
					Console.WriteLine($") {{\n{curlyBrackets}}}");
				}

				// ... | ... | ... |
				CaptureCollection measures = reMeasures.Match(curlyBrackets).Groups["measures"].Captures;
				for (int m = 0; m < measures.Count; m++)
				{
					Capture measure = measures[m];
					if (debug)
						Console.WriteLine($"measure: {measure.ToString().Trim()}");

					// <fis''-2 ces,-3>~-\markup{fis2} d'^\markup{b} ...
					Match signs = reSigns.Match(measure.Value);
					foreach (Capture sign in signs.Groups["signs"].Captures)
					{
						//if (debug)
						//	Console.WriteLine($"\tsign: {sign}");						

						// <fis''-2 ces,-3>~-\markup{fis2}
						Match mChord = reChord.Match(sign.Value);
						if (mChord.Success)
						{
							if (debug)
								Console.WriteLine($"\tchord: {mChord}");
							Match notesInChord = reNotesInChord.Match(mChord.Value);
							Chord chord = new Chord();
							foreach (Capture noteInChord in notesInChord.Groups["notesInChord"].Captures)
							{
								Match not = reNote.Match(noteInChord.Value);
								if (not.Success)
								{
									if (debug)
										Console.WriteLine($"\t\tnote: {not.Groups["note"].Captures[0]}");
									chord.Add(CreateNote(not));
								}
								else
								{
									throw new Exception();
								}
							}
							voices[m % 3].Add(chord);
						}

						// fis''-2~-\markup{fis2}
						Match note = reNote.Match(sign.Value);
						if (!mChord.Success && note.Success)
						{
							if (debug)
								Console.WriteLine($"\tnote: {note.Groups["note"].Captures[0]}");
							voices[m % 3].Add(CreateNote(note));
						}
					}
				}
			}

			return voices;
		}

		private static Note CreateNote(Match mNote)
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

			Note note = new Note(letter, accidentalType, octave);
			int finger;
			if (int.TryParse(mNote.Groups["finger"].Value, out finger))
				note.finger = finger;

			return note;
		}
	}
}