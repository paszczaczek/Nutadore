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
			Regex comment = new Regex(
				@"(?m)%.*\r*$");
			Regex curlyBrackets = new Regex(
				@"\s*\{" +
				@"(?'curlyBrackets'" +
					@"[^{]*" +
					@"(" +
					@"((?'Open'{)[^{}]*)+" +
					@"((?'Close-Open'})[^{}]*)+" +
					@")*" +
					@"(?(Open)(?!))" +
				@")" +
				@"\s*\}");
			Regex parallelMusic = new Regex(
				@"\\parallelMusic\s*#'\([^)]*\)" +
				curlyBrackets);

			string lyText = File.ReadAllText(lyFileName);
			lyText = comment.Replace(lyText, "\r");
			Match m = parallelMusic.Match(lyText);
			if (m.Success)
			{
				var pm = m.Groups["curlyBrackets"].Value;
				Console.WriteLine(pm);
			}

		}
	}
}