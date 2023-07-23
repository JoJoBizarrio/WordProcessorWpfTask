using AsyncAwaitBestPractices.MVVM;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
			RemoveWordsAsync = new AsyncCommand(OnRemoveWordsExecutedAsync);
			RemoveSeparatorsAsync = new AsyncCommand(OnRemoveSeparatorExecutedAsync);
		}

		private readonly IRedactor _redactor;

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

		public string LettersCount { get; set; } = "Count of letters";

		public IAsyncCommand OpenAsync { get; }

		public IAsyncCommand RemoveWordsAsync { get; }

		public IAsyncCommand RemoveSeparatorsAsync { get; }

		public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());

		async public Task OnOpenExecutedAsync()
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
				CurrentText = null;
				CurrentText = await streamReader.ReadToEndAsync();
			}
		}

		async public Task OnRemoveWordsExecutedAsync()
		{
			if (string.IsNullOrEmpty(LettersCount))
			{
				MessageBox.Show("Please, write number", "Value is empty");
				return;
			}

			if (!int.TryParse(LettersCount, out int lettersCount))
			{
				MessageBox.Show("It isnt number", "Value is wrong");
				return;
			}

			var messageBoxResult = MessageBox.Show("Are you sure?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK);

			if (messageBoxResult == MessageBoxResult.Cancel)
			{
				return;
			}

			var temp = CurrentText;

			if (lettersCount != 0)
			{
				temp = await _redactor.RemoveWordsParallelAsync(temp, lettersCount);
			}

			LettersCount = null;
			LettersCount = "Count of letter";
			CurrentText = null;
			CurrentText = temp;

			MessageBox.Show($"Worlds with count of letter = {lettersCount} removed", "Result", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
		}

		async public Task OnRemoveSeparatorExecutedAsync()
		{
			var messageBoxResult = MessageBox.Show("Are you sure?", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK);

			if (messageBoxResult == MessageBoxResult.Cancel)
			{
				return;
			}

			var temp = CurrentText;
			CurrentText = null;
			CurrentText = await _redactor.RemoveAllSeparatorsParallelAsync(temp);

			MessageBox.Show("All separators removed", "Result", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}