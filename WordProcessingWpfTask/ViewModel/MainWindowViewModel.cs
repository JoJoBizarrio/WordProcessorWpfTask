using AsyncAwaitBestPractices.MVVM;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;
using CommunityToolkit.Mvvm.Input;
using System;

namespace WordProcessingWpfTask.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
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
        public IAsyncCommand RemoveWordsAsync => _removeWordsAsync ??= new AsyncCommand(
        async () =>
        {
            await _redactor.RemoveWordsParallelAsync(TextFilesCollection.Where(item => item.IsEditable), _lettersCount);
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

        private IAsyncCommand _removeMarksAsync;
        public IAsyncCommand RemoveMarksAsync => _removeMarksAsync ??= new AsyncCommand(async () =>
        {
            var textFiles = TextFilesCollection.Where(textFile => textFile.IsEditable);
            await _redactor.RemoveAllMarksParallelAsync(textFiles);
        },
        obj =>
        {
            if (!TextFilesCollection.Any(item => item.IsEditable == true) || TextFilesCollection.Count == 0)
            {
                return false;
            }

            return true;
        });

        // open/save opertaion
        private RelayCommand<object> _openAsync;
        public RelayCommand<object> OpenAsync => _openAsync ??= new RelayCommand<object>(obj =>
        {
            if (obj is string filePath)
            {
                TextFilesCollection.Add(_redactor.OpenFile(filePath));
            }
        });

        private IRelayCommand<object> _saveAsync { get; set; }
        public IRelayCommand<object> SaveAsync => _saveAsync ??= new RelayCommand<object>(obj =>
        {
            if (obj is string filePath && SelectedTextFile != null)
            {
                _redactor.SaveFile(SelectedTextFile, filePath);
            }
        });

        // supporting
        private IRelayCommand<object> _close;
        public IRelayCommand<object> Close => _close ??= new RelayCommand<object>(obj =>
        {
            if (obj is Guid id)
            {
                TextFilesCollection.Remove(TextFilesCollection.First(item => item.Id == id));
            }
        });

        private ICommand _clear;
        public ICommand Clear
        {
            get
            {
                if (_clear != null)
                {
                    return _clear;
                }

                return _clear = new RelayCommand(() => SelectedTextFile.Text = null);
            }
        }

        public ICommand Quit { get; } = new RelayCommand(() => Application.Current.Shutdown());

        public ICommand OnChecked
        {
            get
            {
                return new RelayCommand(() => RaiseCanExecute());
            }
        }

        private void RaiseCanExecute()
        {
            RemoveMarksAsync.RaiseCanExecuteChanged();
            RemoveWordsAsync.RaiseCanExecuteChanged();
        }
    }
}