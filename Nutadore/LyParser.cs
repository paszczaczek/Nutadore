using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nutadore
{
	internal class LyParser
	{
		public LyParser(string lyFileName)
		{
			Regex reComment = new Regex(
				@"(?m)%.*\r*$");
			Regex reCurlyBrackets = new Regex(
				@"\s*\{" +
				@"(?<curlyBrackets>" +
					@"[^{]*" +
					@"(" +
						@"((?<Open>{)[^{}]*)+" +
						@"((?<Close-Open>})[^{}]*)+" +
					@")*" +
					@"(?(Open)(?!))" +
				@")" +
				@"\s*\}");
			Regex reParallelMusic = new Regex(
				@"\\parallelMusic\s*#'\(((?<voicesNames>\w+)\s*)+\)" +
				reCurlyBrackets);
			Regex reMeasure = new Regex(
				@"((?<measures>[^|]+)\|?)+");
			Regex reMarkup = new Regex(
				@"([-_^]\\markup\{(?<markup>[^{}]*)\})");
			Regex rePitch = new Regex(
				@"\s*(?<pitch>[a-g](is|es)?)\s*");
			Regex reOctave = new Regex(
				@"\s*(?<octave>'{1,4}|,{1,3})?\s*");
			Regex reFinger = new Regex(
				@"\s*(-(?<finger>[1-9]))?\s*");
			Regex reDuration = new Regex(
				@"\s*(?<duration>1|2|4|8|16)?\s*");
			Regex reTie = new Regex(
				@"(?<tie>~)?");
			Regex reNote = new Regex(
				@"\s*(?<note>" +
					rePitch +
					reOctave +
					reDuration +
					reFinger +
					reMarkup +
				@")\s*");			
			Regex reChord = new Regex(
				@"\s*(?<chord>" +
					"<[^<>]*>" +
					reDuration +
					reTie +
					reMarkup +
				@")\s*");
			Regex reNotesInChord = new Regex(
				@"(?<notesInChord>" +
					rePitch +
					reOctave +
					reFinger +
				")+");
			Regex reSigns = new Regex(
				@"(" +
					"(?<signs>" +
						reNote +
						"|" + 
						reChord +
					@")+\s*" +
				")");

			string lyText = File.ReadAllText(lyFileName);
			lyText = reComment.Replace(lyText, "\r");
			Match m = reParallelMusic.Match(lyText);
			if (m.Success)
			{
				CaptureCollection voicesNames = m.Groups["voicesNames"].Captures;
				string paralelMusic = m.Groups["curlyBrackets"].Value;
				CaptureCollection measures = reMeasure.Match(paralelMusic).Groups["measures"].Captures;
				foreach (Capture measure in measures)
				{
					Match mSigns = reSigns.Match(measure.Value);
					foreach (Capture sign in mSigns.Groups["signs"].Captures)
					{
						Match mNote = reNote.Match(sign.Value);
						if (mNote.Success)
						{
							Console.WriteLine("{0}", mNote.Groups["note"].Captures[0]);
						}
						Match mChord = reChord.Match(sign.Value);
						if (mChord.Success)
						{
							Match mNotesInChord = reNotesInChord.Match(mChord.Value);
							Console.WriteLine("{0}", mNotesInChord.Value);
							foreach (Capture noteInChord in mNotesInChord.Groups["notesInChord"].Captures)
							{
								Console.WriteLine("\t{0}", noteInChord.Value);
							}
						}
					}
				}
			}

		}
	}
}