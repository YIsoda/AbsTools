using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
namespace AbsToolGui
{
	using AbsConvertCore;
	using System.IO;

	using Microsoft.Win32;
	using Microsoft.WindowsAPICodePack.Dialogs;
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.OpenFileDialog()
			{
				Filter = "テキスト形式（*.txt）|*.txt|すべてのファイル（*,*）|*.*",
				Title = "CHEMUSBで取得した生データのファイルを選択",
				Multiselect = true,
			};

			if (dialog.ShowDialog() ?? false)
			{
				AddFilesToListBox(dialog.FileNames.Select(filename => new System.IO.FileInfo(filename)));
			}
		}

		private void AddFilesToListBox(IEnumerable<FileInfo> info)
		{
			var checker = new FileFormatChecker();
			MyFileList list = this.DataContext as MyFileList;

			foreach (var item in info)
			{
				list.FileStates.Add(new FileState { FileInfo = item, StateString = checker.IsValidFormat(item.FullName) ? "" : "無効なファイル形式です" });
			}
			//this.RowDataListBox.ItemsSource =  info
			//	.Select(
			//		fileinfo=>
			//		(
			//			fileinfo,
			//			checker.IsValidFormat(fileinfo.FullName)?"":"無効なファイル形式です"
			//		)
			//	);
		}


		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog()
			{
				IsFolderPicker = true
			};
			if (dialog.ShowDialog() != Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok) return;
			var checker = new FileFormatChecker();

			var fileinfolist = new List<FileInfo>();
			foreach (FileState item in this.RowDataListBox.ItemsSource)
			{
				if (checker.IsValidFormat(item.FileInfo.FullName))
					fileinfolist.Add(item.FileInfo);
			}
			Export(fileinfolist, dialog.FileName);

			System.Diagnostics.Process.Start(dialog.FileName);
		}

		public void Export(IEnumerable<FileInfo> args, string exportFolder)
		{
			var checker = new FileFormatChecker();
			foreach (var info in args)
			{
				if (!checker.IsValidFormat(info.FullName))
				{
					throw new FormatException($"{info.Name}: エラー: データの形式が間違っていないか確認してください");
				}

				var loader = new AbsDataLoader();
				loader.LoadFromFile(info.FullName);

				var absdata = new Absdata(loader.WaveLength, loader.AbsValue);

				var newFileName = exportFolder + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(info.Name) + "-変換済み.txt";

				File.WriteAllLines(newFileName, absdata.GetConvertedAbs().Select(x => x.ToString("0.0000")));
				System.Diagnostics.Debug.WriteLine($"\t{info.Name} \t->\t {newFileName}");
			}
		}

		private void RowDataListBox_Drop(object sender, DragEventArgs e)
		{
			AddFilesToListBox(
				((string[])e.Data.GetData(DataFormats.FileDrop))
				.Select(filename => new FileInfo(filename))
				.Where(fileinfo => fileinfo.Extension == "txt" || fileinfo.Extension == ".txt")
				);
		}

		private void Button_Click_Delete(object sender, RoutedEventArgs e)
		{
			// MyFileList list = this.DataContext as MyFileList;

		}
		/// <summary>
		/// export in the same folder
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			var directoryList = new List<string>();

			var infolist = new List<FileInfo>();
			var checker = new FileFormatChecker();
			foreach (FileState item in this.RowDataListBox.ItemsSource)
			{
				if (checker.IsValidFormat(item.FileInfo.FullName))
					infolist.Add(item.FileInfo);
			}

			foreach (var item in infolist)
			{
				var loader = new AbsDataLoader();
				loader.LoadFromFile(item.FullName);

				var absdata = new Absdata(loader.WaveLength, loader.AbsValue);
				var newfilename = item.DirectoryName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(item.Name) + "-変換済み.txt";

				File.WriteAllLines(newfilename, absdata.GetConvertedAbs().Select(X => X.ToString("0.0000")));

				if (!directoryList.Contains(item.DirectoryName)) directoryList.Add(item.DirectoryName);
			}

			foreach (var item in directoryList)
			{
				System.Diagnostics.Process.Start(item);
			}
		}
		/// <summary>
		/// export one file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			var dialog = new Microsoft.Win32.SaveFileDialog()// Microsoft.WindowsAPICodePack.Dialogs.CommonSaveFileDialog()
			{

			};
			dialog.DefaultExt = "tsv";

			if (!(bool)dialog.ShowDialog()) return;
			var checker = new FileFormatChecker();

			var fileinfolist = new List<FileInfo>();
			foreach (FileState item in this.RowDataListBox.ItemsSource)
			{
				if (checker.IsValidFormat(item.FileInfo.FullName))
					fileinfolist.Add(item.FileInfo);
			}
			ExportAsOneFile(fileinfolist, dialog.FileName);

			System.Diagnostics.Process.Start(Path.GetDirectoryName(dialog.FileName));
		}

		private void ExportAsOneFile(List<FileInfo> fileinfolist, string fileName)
		{

			var absDatas = new List<List<double>>();

			foreach (var item in fileinfolist)
			{
				var loader = new AbsDataLoader();
				loader.LoadFromFile(item.FullName);
				var absdata = new Absdata(loader.WaveLength, loader.AbsValue);

				absDatas.Add(absdata.GetConvertedAbs().ToList());
			}

			var waveLength = Enumerable.Range(300, 800 - 300 + 1);//CAUTION! magic number


			var header = new List<string> { "waveLength/nm" };
			header.AddRange(fileinfolist.Select(info => info.Name));

			File.WriteAllText(fileName,
				string.Join("\t", header) + "\n" +
				string.Join(
					"\n",
					waveLength.Select((l, n) => l + "\t" + string.Join("\t", absDatas.Select(data => data[n].ToString("0.0000"))))
					)
				//absdata.GetConvertedAbs().Select(x => x.ToString("0.0000"))

				);


		}
	}

	public class FileState
	{
		public FileInfo FileInfo { get; set; }
		public string StateString { get; set; }

		public override string ToString() => FileInfo.Name;

		public bool HasExported { get; set; } = false;
	}

	public class MyFileList
	{
		public ObservableCollection<FileState> FileStates { get; private set; }

		public MyFileList()
		{
			this.FileStates = new ObservableCollection<FileState>();
		}
	}
}