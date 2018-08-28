using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsConvertCore
{
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
			new {wavelen,abs}
#endif
			)
				.SkipWhile((x) => x.wavelen < this.MinWaveLength - 1)
				.TakeWhile((x) => x.wavelen < this.MaxWaveLen + 1);//.ToList;


			var range = Enumerable.Range(MinWaveLength, MaxWaveLen - MinWaveLength + 1).ToList();

			var prev=zippedEnumerable.First();
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
