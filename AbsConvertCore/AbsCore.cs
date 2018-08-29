using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;
using System.Text.RegularExpressions;


namespace AbsConvertCore
{
	using static System.IO.File;
	using CommandLine;
#if NETCOREAPP2_0

#else
	public static class EnumerableExtension
	{
		public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
		{
			return source.Take(source.Count() - count);
		}
	}
#endif

	public class DataFormatChecker
	{
		public DataFormatChecker() { }

		public bool IsValid(string path)
		{
			return !ReadLines(path).Skip(30).Take(10)
					.All(x => Regex.IsMatch(x, @"[0-9\.]*\t[\-0-9\.]*"));
		}
	}
	/// <summary>
	/// コマンドラインオプション
	/// </summary>
	public class Options
	{
		[Option('s', "suffix", Required = false, Default = "-変換後", HelpText = "変換後のファイル名に追加する文字列．")]
		public string Suffix { get; set; }

		//[Option('v', "vsebose", Required = false, HelpText = "samle")]
		//public bool Verbose { get; set; }

		[Option('u', "upperlimit", Required = false, Default = 800, HelpText = "波長[nm]の最大値")]
		public int UpperLimit { get; set; }

		[Option('l', "lowerlimit", Required = false, Default = 350, HelpText = "波長[nm]の最小値")]
		public int LowerLimit { get; set; }

	}
	static class CommandLineApp
	{
		public static void RunApplication(string[] args)
		{
			var options = new Options();
			//var isSuccess = 
			Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
			{
				WriteLine($"suffix: {o.Suffix}");
			});
			
		}

		private static void ConvertOneFile(string arg)
		{
			FileInfo info = new FileInfo(arg);

			if (!info.Exists)
			{
				WriteLine($"\t{info.Name}: エラー: ファイルが存在しません");
				return;
			}

			var checker = new DataFormatChecker();
			if (!checker.IsValid(arg))
			{
				WriteLine($"\t{info.Name}: エラー: データの形式が間違っていないか確認してください");
				return;
			}

			var data = ReadLines(arg).Skip(18).SkipLast(2)
				.Select(x =>
				{
					var t = x.Split('\t');
#if NETCOREAPP2_0
					return (double.Parse(t[0]), double.Parse(t[1]));
#else
						return new { Item1 = double.Parse(t[0]), Item2 = double.Parse(t[1]) };
#endif
				});
			var absdata = new Absdata(
#if NETCOREAPP2_0
					data.Select(x => x.Item1), data.Select(x => x.Item2)
#else
					data.Select(x => x.Item1), data.Select(x => x.Item2)
#endif
					);
			var newFileName = info.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(info.Name) + "-simplified.txt";

			WriteAllLines(newFileName, absdata.GetConvertedAbs().Select(x => x.ToString("0.0000")));
			WriteLine($"\t{arg} \t->\t {newFileName}");
		}
	}


	class Absdata
	{


		private SortedDictionary<int, double> ToInterpolatedData(IEnumerable<double> waveLen, IEnumerable<double> abs)
		{

			var rangedList = waveLen.Zip(abs, (_wavelen, _abs) => new { WaveLen = _wavelen, Abs = _abs })
				.SkipWhile(x => x.WaveLen < 300)
				.TakeWhile(x => x.WaveLen < 700);



			throw new NotImplementedException();
		}


		//private IEnumerable<double> WaveLength { get => _waveLength; set => _waveLength = value; }

		private readonly IEnumerable<double> _waveLength;
		private readonly IEnumerable<double> _abs;



		private double LinearInterpolation(double x1, double x2, double y1, double y2, int x)
		{
			if (x1 > x && x2 > x) throw new ArgumentException($"{nameof(x)}は既知の{nameof(x1)}, {nameof(x2)}のいずれよりも小さいです。{nameof(x1)} <= {nameof(x)} <={nameof(x2)}] を満たす値を指定してください");
			if (x1 < x && x2 < x) throw new ArgumentException($"{nameof(x)}は既知の{nameof(x1)}, {nameof(x2)}のいずれよりも大きいです。{nameof(x1)} <= {nameof(x)} <={nameof(x2)}] を満たす値を指定してください");

			return y1 + (x - x1) * (y2 - y1) / (x2 - x1);
		}
		/// <summary>
		/// <see cref="MinWaveLength"/>～<see cref="MaxWaveLen"/>の範囲の整数値の波長に対応する吸光度のリストを返します。
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<double> GetConvertedAbs()
		{
			//var min=this._waveLength.tol
			var zippedEnumerable = this._waveLength.Zip(_abs, (wavelen, abs) =>
#if NETCOREAPP2_0
			(wavelen, abs)
#else
			new { wavelen, abs }
#endif
			)
				.SkipWhile((x) => x.wavelen < this.MinWaveLength - 1)
				.TakeWhile((x) => x.wavelen < this.MaxWaveLen + 1);//.ToList;


			var range = Enumerable.Range(MinWaveLength, MaxWaveLen - MinWaveLength + 1).ToList();

			var prev = zippedEnumerable.First();
			int rangeindex = 0;

			foreach (var item in zippedEnumerable.Skip(1))
			{
				if (prev.wavelen <= range[rangeindex] && item.wavelen > range[rangeindex])
				{
					yield return LinearInterpolation(prev.wavelen, item.wavelen, prev.abs, item.abs, range[rangeindex]);
					rangeindex++;
					if (rangeindex == range.Count - 1) yield break;
				}
				prev = item;
			}

		}

		public int MinWaveLength { get; }
		public int MaxWaveLen { get; }

		public Absdata(IEnumerable<double> waveLen, IEnumerable<double> abs, int minWaveLen = 300, int maxwaveLen = 800)
		{
			this._waveLength = waveLen;
			this._abs = abs;
			this.MinWaveLength = minWaveLen;
			this.MaxWaveLen = maxwaveLen;
		}
	}

	static class AbsConvertor
	{

	}
}
