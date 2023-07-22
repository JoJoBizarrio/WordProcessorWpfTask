using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WordProcessingWpfTask.Model;

namespace WordProcessingWpfTask.ViewModel
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{
		public MainWindowViewModel(IRedactor redactor)
		{
			_redactor = redactor;
		}

		private readonly IRedactor _redactor;

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private string _currentText { get; set; }
		public string CurrentText
		{
			get => _currentText;
			set
			{
				_currentText = value;
				OnPropertyChanged(nameof(CurrentText));
			}
		}

		//private string _pathToFile;
		//public string PathToFile
		//{
		//	get => _pathToFile;
		//	set
		//	{
		//		if (_pathToFile != value && File.Exists(_pathToFile))
		//		{
		//			using (StreamReader streamReader = new StreamReader(_pathToFile))
		//			{
		//				CurrentText = streamReader.ReadToEnd();
		//			}
		//		}

		//		OnPropertyChanged(nameof(PathToFile));
		//	}
		//}

		private ICommand _open;

		public ICommand Open
		{
			get
			{
				return _open ?? (_open = new RelayCommand(obj =>
				{
					OpenFileDialog openFileDialog = new OpenFileDialog();
					openFileDialog.Title = "Choose file";
					openFileDialog.InitialDirectory = Environment.CurrentDirectory;
					openFileDialog.Filter = "Text (*.txt)|*.txt|WinWord (*.docx)|*.docx|All files (*.*)|*.*";

					if (openFileDialog.ShowDialog() == false)
					{
						return;
					}

					using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
					{
						CurrentText = streamReader.ReadToEnd();
					}
				}));
			}
		}

		public ICommand Quit { get => new RelayCommand(p => Application.Current.Shutdown()); }

		public MainWindowViewModel()
		{
			CurrentText = "Choose and open file...";
		}
	}
}