using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static System.Console;
using static System.IO.File;

namespace AbsSimplifierForWindows
{

	using AbsConvertCore;
	class Program
	{
		static void Main(string[] args)
		{
			foreach (var arg in args)
			{
				var info = new FileInfo(arg);

				if (!info.Exists)
				{
					WriteLine($"\t{info.Name}: エラー: ファイルが存在しません");
					continue;
				}
				if (
					!ReadLines(arg).Skip(30).Take(10)
					.All(x => Regex.IsMatch(x, @"[0-9\.]*\t[\-0-9\.]*"))
					)
				{
					WriteLine($"\t{info.Name}: エラー: データの形式が間違っていないか確認してください");
					continue;
				}


				var _data = ReadLines(arg).Skip(18);//.Take().SkipLast(2)
				var data = _data
					.Take(_data.Count() - 3)
					.Select(x => { var t = x.Split('\t'); return new { WaveLength = double.Parse(t[0]), Abs = double.Parse(t[1]) }; });

				var absdata = new Absdata(
					data.Select(x => x.WaveLength),
					data.Select(x => x.Abs)
					);
				var newFileName = info.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(info.Name) + "-simplified.txt";

				WriteAllLines(newFileName, absdata.GetConvertedAbs().Select(x => x.ToString("0.0000")));
				WriteLine($"\t{arg} \t->\t {newFileName}");

			}

#if Debug
			ReadKey();
#endif
		}
	}
}
