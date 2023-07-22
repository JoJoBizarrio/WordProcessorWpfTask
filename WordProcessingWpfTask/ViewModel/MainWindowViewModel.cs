using AsyncAwaitBestPractices.MVVM;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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
			OpenAsync = new AsyncCommand(OpenCommand);
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

		public IAsyncCommand OpenAsync { get; }

		async public Task OpenCommand()
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
				CurrentText = await streamReader.ReadToEndAsync();
			}
		}

		public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());

		public MainWindowViewModel()
		{
			CurrentText = "Choose and open file...";
		}
	}
}