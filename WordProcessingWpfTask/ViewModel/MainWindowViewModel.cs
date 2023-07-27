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

            Clear = new RelayCommand(OnClearExecuted);

            TextFilesCollection = new ObservableCollection<TextFile>();
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
            set
            {
                Set(ref _selectedTextFile, value);
            }
        }

        // Remove Opeartion
        public IAsyncCommand RemoveWordsAsync { get; set; }

        public IAsyncCommand RemoveMarksAsync { get; set; }

        async private Task OnRemoveWordsExecutedAsync()
        {
            await _redactor.RemoveWordsParallelAsync(SelectedTextFile.Id, int.Parse(LettersCount));
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
            await _redactor.RemoveAllMarksParallelAsync(SelectedTextFile.Id);
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
        public ICommand Clear { get; set; }
        public void OnClearExecuted(object obj)
        {
            SelectedTextFile.Text = null;
        }

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

        public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());
    }
}