using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

namespace AbsToolGui
{

}

namespace MVVMModel
{
	using Prism.Mvvm;

	class Model : BindableBase
	{
		public ObservableCollection<DataFile> DataFileList { get; private set; }

		public void ExportFiles() { }
	}

	public class DataFile : BindableBase
	{
		private FileInfo _fileInfo;

		private bool _isValidData;

		public FileInfo FileInfo { get => _fileInfo; set => SetProperty(ref _fileInfo, value); }

		public bool IsValidData { get => _isValidData; set => SetProperty(ref _isValidData, value); }
	}
}

namespace ViewModel
{
	public class ViewModel
	{
		private MVVMModel.Model model = new MVVMModel.Model();
	}
}
