using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace AbsConvertCore
{
	using System.IO;
	using static System.IO.File;
	using static System.Console;
	class Program
	{
		static void Main(string[] args)
		{
			//ReadKey();
			foreach (var arg in args)
			{
				var info = new FileInfo(arg);

				if (!info.Exists)
				{
					WriteLine($"\t{info.Name}: エラー: ファイルが存在しません");
					continue;
				}


				if(
					!ReadLines(arg).Skip(30).Take(10)
					.All(x => Regex.IsMatch(x, @"[0-9\.]*\t[\-0-9\.]*"))
					)
				{
					WriteLine($"\t{info.Name}: エラー: データの形式が間違っていないか確認してください");
					continue;
				}

				var data = ReadLines(arg).Skip(18).SkipLast(2)
					.Select(x => { var t = x.Split("\t"); return (double.Parse(t[0]), double.Parse(t[1])); });
				var absdata = new Absdata(
					data.Select(x => x.Item1),
					data.Select(x => x.Item2)
					);
				var newFileName = info.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(info.Name) + "-simplified.txt";

				WriteAllLines(newFileName, absdata.GetConvertedAbs().Select(x => x.ToString()));
				WriteLine($"\t{arg} \t->\t {newFileName}");
			}

		}
	}
}
