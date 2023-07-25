using AsyncAwaitBestPractices.MVVM;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;

namespace WordProcessingWpfTask.ViewModel
{
	internal class MainWindowViewModel : ViewModelBase
	{
		public MainWindowViewModel(IRedactor redactor)
		{
			_redactor = redactor;
			RemoveWordsAsync = new AsyncCommand(OnRemoveWordsExecutedAsync, OnRemoveWordsCanExecuted);
			RemoveMarksAsync = new AsyncCommand(OnRemoveMarksExecutedAsync);

			OpenAsync = new RelayCommand(async obj => await OnOpenExecutedAsync(obj));
			SaveAsync = new RelayCommand(async obj => await OnSaveExecutedAsync(obj));

			Clear = new RelayCommand(OnClearExecuted);

			MyOC = new ObservableCollection<TextFile>();
			MyOC.Add(new TextFile() { Title = "MyName", Text = "MyText" });
		}

		private readonly IRedactor _redactor;

		public string FilePath { get; private set; }

		private string _currentText = "File => Open => Choose and open file...";
		public string CurrentText
		{
			get => _currentText;
			set => Set(ref _currentText, value);

		}

		private string _lettersCount;
		public string LettersCount
		{
			get => _lettersCount;
			set
			{
				Set(ref _lettersCount, value);
				RemoveWordsAsync.RaiseCanExecuteChanged();
			}
		}

		// Remove Opeartion
		public IAsyncCommand RemoveWordsAsync { get; set; }

		public IAsyncCommand RemoveMarksAsync { get; set; }

		async private Task OnRemoveWordsExecutedAsync()
		{
			var temp = CurrentText;
			temp = await _redactor.RemoveWordsParallelAsync(temp, int.Parse(LettersCount));

			CurrentText = temp;
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

		async private Task OnRemoveMarksExecutedAsync()
		{
			var temp = CurrentText;
			CurrentText = await _redactor.RemoveAllMarksParallelAsync(temp);
		}

		// logic of save file
		//TODO: separate from VM
		public RelayCommand OpenAsync { get; set; }
		public RelayCommand SaveAsync { get; set; }

		async private Task OnOpenExecutedAsync(object obj)
		{
			if (obj is string filePath)
			{
				using (StreamReader streamReader = new StreamReader(filePath))
				{
					FilePath = filePath;
					var temp = await streamReader.ReadToEndAsync();
					CurrentText = temp;
				}
			}
		}

		async private Task OnSaveExecutedAsync(object obj)
		{
			if (obj is string filePath)
			{
				using (var writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write)))
				{
					await writer.WriteAsync(CurrentText);
				}
			}
		}

		// supporting
		public ICommand Clear { get; set; }

		public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());

		public void OnClearExecuted(object obj)
		{
			CurrentText = null;
		}

		public ObservableCollection<TextFile> MyOC { get; set; }
	}
}