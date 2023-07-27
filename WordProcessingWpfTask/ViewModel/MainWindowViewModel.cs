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
			TextFilesCollection = new ObservableCollection<TextFile>();
		}

		private readonly IRedactor _redactor;

		public ObservableCollection<TextFile> TextFilesCollection { get; set; }

		private int _lettersCount;
		private string _lettersCountText;
		public string LettersCountText
		{
			get => _lettersCountText;
			set
			{
				Set(ref _lettersCountText, value);

				if (int.TryParse(_lettersCountText, out int result))
				{
					_lettersCount = result;
				}

				RaiseCanExecute();
			}
		}

		private TextFile _selectedTextFile;
		public TextFile SelectedTextFile
		{
			get => _selectedTextFile;
			set
			{
				Set(ref _selectedTextFile, value);
				RaiseCanExecute();
			}
		}

		// Remove Opeartion
		private IAsyncCommand _removeWordsAsync;
		public IAsyncCommand RemoveWordsAsync
		{
			get
			{
				if (_removeWordsAsync != null)
				{
					return _removeWordsAsync;
				}

				return _removeWordsAsync = new AsyncCommand(
				async () =>
				{
					var idArray = TextFilesCollection.Select(textFile => textFile.Id);
					await _redactor.RemoveWordsInsideSeveralTextFilesParallelAsync(idArray, _lettersCount);
				},
				obj =>
				{
					if (TextFilesCollection.Count == 0
					|| !TextFilesCollection.Any(item => item.IsEditable == true)
					|| string.IsNullOrEmpty(LettersCountText)
					|| !int.TryParse(LettersCountText, out int lettersCount)
					|| lettersCount == 0)
					{
						return false;
					}

					return true;
				});
			}
		}

		private IAsyncCommand _removeMarksAsync;
		public IAsyncCommand RemoveMarksAsync
		{
			get
			{
				if (_removeMarksAsync != null)
				{
					return _removeMarksAsync;
				}

				return _removeMarksAsync = new AsyncCommand(async () =>
				{
					var idArray = TextFilesCollection.Select(textFile => textFile.Id);
					await _redactor.RemoveAllMarksInsideSeveralTextFilesParallelAsync(idArray);
				},
				obj =>
				{
					if (!TextFilesCollection.Any(item => item.IsEditable == true) || TextFilesCollection.Count == 0)
					{
						return false;
					}

					return true;
				});
			}
		}

		// open/save opertaion
		private IAsyncCommand<object> _openAsync;
		public IAsyncCommand<object> OpenAsync
		{
			get
			{
				if (_openAsync != null)
				{
					return _openAsync;
				}

				return _openAsync = new AsyncCommand<object>(async obj =>
				{
					if (obj is string filePath)
					{
						TextFilesCollection.Add(await _redactor.OpenFileAsync(filePath));
					}
				});
			}
		}

		private IAsyncCommand<object> _saveAsync { get; set; }
		public IAsyncCommand<object> SaveAsync
		{
			get
			{
				if (_saveAsync != null)
				{
					return _saveAsync;
				}

				return _saveAsync = new AsyncCommand<object>(async obj =>
				{
					if (obj is string filePath)
					{
						await _redactor.SaveFileAsync(SelectedTextFile.Id, filePath);
					}
				});
			}
		}

		// supporting
		private ICommand _close;
		public ICommand Close
		{
			get
			{
				if (_close != null)
				{
					return _close;
				}

				return _close = new RelayCommand(textFile =>
				{
					TextFilesCollection.Remove((TextFile)textFile);
					_redactor.Remove(((TextFile)textFile).Id);
				});
			}
		}

		private ICommand _clear;
		public ICommand Clear
		{
			get
			{
				if (_clear != null)
				{
					return _clear;
				}

				return _clear = new RelayCommand(p => SelectedTextFile.Title = null);
			}

		}

		public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());

		public ICommand OnChecked
		{
			get
			{
				return new RelayCommand(obj => RaiseCanExecute());
			}
		}

		private void RaiseCanExecute()
		{
			RemoveMarksAsync.RaiseCanExecuteChanged();
			RemoveWordsAsync.RaiseCanExecuteChanged();
		}
	}
}