using AsyncAwaitBestPractices.MVVM;
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
        }

        private readonly IRedactor _redactor;

        public string FilePath { get; private set; }
        private string _currentText { get; set; } = "File => Open => Choose and open file...";
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

        public IAsyncCommand RemoveWordsAsync { get; set; }

        public IAsyncCommand RemoveMarksAsync { get; set; }

        public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());


        async private Task OnRemoveWordsExecutedAsync() // диалог окн и мессджбокс наруш мввм
        {
            var temp = CurrentText;
            temp = await _redactor.RemoveWordsParallelAsync(temp, int.Parse(LettersCount));

            CurrentText = null;
            CurrentText = temp;

            MessageBox.Show($"Words with count of letter = {LettersCount} removed", "Done", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
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

        async private Task OnRemoveMarksExecutedAsync() // диалог окн и мессджбокс наруш мввм
        {
            var temp = CurrentText;
            CurrentText = null;
            CurrentText = await _redactor.RemoveAllMarksParallelAsync(temp);

            MessageBox.Show("All separators removed", "Done", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        async public Task OpenFileAsync(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                FilePath = filePath;
                CurrentText = null;
                CurrentText = await streamReader.ReadToEndAsync();
            }
        }

        async public Task SaveFileAsync(string filePath)
        {
            using (var writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write)))
            {
                var temp = CurrentText;
                await writer.WriteAsync(CurrentText);
            }

            FilePath = filePath;
        }
    }
}