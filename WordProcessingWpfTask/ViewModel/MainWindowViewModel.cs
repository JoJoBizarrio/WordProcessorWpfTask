using AsyncAwaitBestPractices.MVVM;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
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
			CurrentText = "File => Open => Choose and open file...";
			_redactor = redactor;
			OpenAsync = new AsyncCommand(OnOpenExecutedAsync);
			RemoveWordsAsync = new AsyncCommand(OnRemoveWordsExecutedAsync, OnRemoveWordsCanExecuted);
			RemoveSeparatorsAsync = new AsyncCommand(OnRemoveSeparatorExecutedAsync);
			SaveAsync = new AsyncCommand(OnSaveExecutedAsync);
		}

		private readonly IRedactor _redactor;

		private string _filePath { get; set; }
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

		private string _lettersCount { get; set; }
		public string LettersCount
		{
			get => _lettersCount;
			set
			{
				_lettersCount = value;
				OnPropertyChanged(nameof(LettersCount));
				RemoveWordsAsync.RaiseCanExecuteChanged();
			}
		}

		public IAsyncCommand OpenAsync { get; set; }

		public IAsyncCommand RemoveWordsAsync { get; set; }

		public IAsyncCommand RemoveSeparatorsAsync { get; set; }

		public IAsyncCommand SaveAsync { get; set; }

		public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());

		async private Task OnOpenExecutedAsync() // диалог окн наруш мввм
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Choose file";
			openFileDialog.InitialDirectory = Environment.CurrentDirectory;
			openFileDialog.Filter = "Text (*.txt)|*.txt|Word (*.docx)|*.docx|All files (*.*)|*.*";

			if (openFileDialog.ShowDialog() == false)
			{
				return;
			}

			using (StreamReader streamReader = new StreamReader(openFileDialog.FileName))
			{
				_filePath = openFileDialog.FileName;
				CurrentText = null;
				CurrentText = await streamReader.ReadToEndAsync();
			}
		}

		async private Task OnRemoveWordsExecutedAsync() // диалог окн и мессджбокс наруш мввм
		{
			var messageBoxResult = MessageBox.Show("Are you sure?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK);

			if (messageBoxResult == MessageBoxResult.Cancel)
			{
				return;
			}

			var temp = CurrentText;
			temp = await _redactor.RemoveWordsParallelAsync(temp, int.Parse(LettersCount));

			CurrentText = null;
			CurrentText = temp;

			MessageBox.Show($"Worlds with count of letter = {LettersCount} removed", "Done", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
		}

		private bool OnRemoveWordsCanExecuted(object obj)
		{
			if (string.IsNullOrEmpty(LettersCount))
			{
				return false;
			}

			if (!int.TryParse(LettersCount, out int lettersCount))
			{
				return false;
			}

			if (lettersCount == 0)
			{
				return false;
			}

			return true;
		}

		async private Task OnRemoveSeparatorExecutedAsync() // диалог окн и мессджбокс наруш мввм
		{
			var messageBoxResult = MessageBox.Show("Are you sure?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK);

			if (messageBoxResult == MessageBoxResult.Cancel)
			{
				return;
			}

			var temp = CurrentText;
			CurrentText = null;
			CurrentText = await _redactor.RemoveAllSeparatorsParallelAsync(temp);

			MessageBox.Show("All separators removed", "Done", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
		}

		async private Task OnSaveExecutedAsync() // диалог окн наруш мввм
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				DefaultExt = ".txt",
				AddExtension = true,
				Title = "Save file",
				CreatePrompt = true,
				OverwritePrompt = true,
				Filter = "Text (*.txt)|*.txt|Word (*.docx)|*.docx|All files (*.*)|*.*",
				InitialDirectory = _filePath
			};

			if (saveFileDialog.ShowDialog() == false)
			{
				return;
			}

			using (var writer = new StreamWriter(new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write)))
			{
				await writer.WriteAsync(CurrentText);
			}

			_filePath = saveFileDialog.FileName;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}