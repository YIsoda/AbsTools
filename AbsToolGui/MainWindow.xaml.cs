﻿using System;
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
			var checker = new FileFormatChecker();
			if (dialog.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
			{
				var fileinfolist = new List<FileInfo>();
				foreach (FileState item in this.RowDataListBox.ItemsSource)
				{
					if(checker.IsValidFormat(item.FileInfo.FullName))
						fileinfolist.Add(item.FileInfo);
				}
				Export(fileinfolist, dialog.FileName);
			}
		}



		public void Export(IEnumerable<FileInfo> args, string exportFolder)
		{
			foreach (var info in args)
			{
				var checker = new FileFormatChecker();
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
			MyFileList list = this.DataContext as MyFileList;
			
		}
	}

	public class FileState
	{
		public FileInfo FileInfo { get; set; }
		public string StateString { get; set; }

		public override string ToString() => FileInfo.Name;
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