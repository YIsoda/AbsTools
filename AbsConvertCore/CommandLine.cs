using System.Linq;
using System.IO;
using static System.Console;
using System.Text.RegularExpressions;


namespace AbsConvertCore
{
	using static System.IO.File;

	static class CommandLineApp
	{
		public static void RunApplication(string[] args)
		{
			foreach (var arg in args)
			{
				var info = new FileInfo(arg);

				if (!info.Exists)
				{
					WriteLine($"\t{info.Name}: エラー: ファイルが存在しません");
					continue;
				}

				var checker = new FileFormatChecker();

				if (!checker.IsValidFormat(arg))
				{
					WriteLine($"\t{info.Name}: エラー: データの形式が間違っていないか確認してください");
					continue;
				}

				var loader = new AbsDataLoader();
				loader.LoadFromFile(arg);
				
				var absdata = new Absdata(loader.WaveLength,loader.AbsValue);

				var newFileName = info.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(info.Name) + "-simplified.txt";

				WriteAllLines(newFileName, absdata.GetConvertedAbs().Select(x => x.ToString("0.0000")));
				WriteLine($"\t{arg} \t->\t {newFileName}");
			}
		}


	}
}
