using AsyncAwaitBestPractices.MVVM;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

			OpenAsync = new RelayCommand(async obj => await OnOpenExecutedAsync(obj)); // async/await dosnt work here
			SaveAsync = new RelayCommand(async obj => await OnSaveExecutedAsync(obj)); // async/await dosnt work here

			Clear = new RelayCommand(OnClearExecuted);

			TextFilesCollection = new ObservableCollection<TextFile>();
			TextFilesCollection.Add(new TextFile() { Title = "MyName1", Text = "MyText1", FilePath = "MyPath" });
			TextFilesCollection.Add(new TextFile() { Title = "MyName2", Text = "MyText2", FilePath = "MyPath" });
		}

		private readonly IRedactor _redactor;

		public ObservableCollection<TextFile> TextFilesCollection { get; set; }


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

		private TextFile _selectedTextFile;
		public TextFile SelectedTextFile
		{
			get => _selectedTextFile;
			set => Set(ref _selectedTextFile, value);
		}

		// Remove Opeartion
		public IAsyncCommand RemoveWordsAsync { get; set; }

		public IAsyncCommand RemoveMarksAsync { get; set; }

		async private Task OnRemoveWordsExecutedAsync()
		{
			var temp = SelectedTextFile;
			temp.Text = await _redactor.RemoveWordsParallelAsync(temp.Text, int.Parse(LettersCount));
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
			var temp = SelectedTextFile;
			temp.Text = await _redactor.RemoveAllMarksParallelAsync(temp.Text);
		}

		// logic of save file
		//TODO: separate from VM
		//TODO: RelayCommandAsync instead of current
		public RelayCommand OpenAsync { get; set; }
		public RelayCommand SaveAsync { get; set; }

		async private Task OnOpenExecutedAsync(object obj)
		{
			if (obj is string filePath)
			{
				using (StreamReader streamReader = new StreamReader(filePath))
				{
					var temp = await streamReader.ReadToEndAsync();
					var newTextFile = new TextFile()
					{
						Title = filePath.Remove(filePath.LastIndexOf('.')).Substring(filePath.LastIndexOf('\\') + 1),
						Text = temp,
						FilePath = filePath
					};

					TextFilesCollection.Add(newTextFile);
				}
			}
		}

		async private Task OnSaveExecutedAsync(object obj)
		{
			if (obj is string filePath)
			{
				using (var writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write)))
				{
					await writer.WriteAsync(SelectedTextFile.Text);
				}
			}
		}

		// supporting
		public ICommand Clear { get; set; }

		private ICommand _close;
		public ICommand Close
		{
			get
			{
				if (_close != null)
				{
					return _close;
				}

				return _close = new RelayCommand(p => TextFilesCollection.Remove((TextFile)p));
			}
		}

		public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());

		public void OnClearExecuted(object obj)
		{
			SelectedTextFile.Text = null;
		}
	}
}