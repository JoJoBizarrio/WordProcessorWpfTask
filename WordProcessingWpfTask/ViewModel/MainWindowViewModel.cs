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
            Clear = new RelayCommand(OnClearExecuted);
            //Test = new RelayCommand(OnTest);
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

        public IAsyncCommand RemoveWordsAsync { get; set; }

        public IAsyncCommand RemoveMarksAsync { get; set; }

        async private Task OnRemoveWordsExecutedAsync() // диалог окн и мессджбокс наруш мввм
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

        async private Task OnRemoveMarksExecutedAsync() // диалог окн и мессджбокс наруш мввм
        {
            var temp = CurrentText;
            CurrentText = await _redactor.RemoveAllMarksParallelAsync(temp);

            MessageBox.Show("All separators removed", "Done", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        async public Task OpenFileAsync(string filePath)
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                FilePath = filePath;
                var temp = await streamReader.ReadToEndAsync();
                CurrentText = temp;
            }
        }

        async public Task SaveFileAsync(string filePath)
        {
            using (var writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write)))
            {
                await writer.WriteAsync(CurrentText);
            }

            FilePath = filePath;
        }


        // supporting
        public ICommand Clear { get; set; }

        public ICommand Quit { get; } = new RelayCommand(p => Application.Current.Shutdown());

        public void OnClearExecuted(object obj)
        {
            CurrentText = null;
        }

        private RelayCommand _test;

        public RelayCommand Test { 
            get 
            {
                if (_test != null)
                {
                    return _test;
                }

                return _test = new RelayCommand(obj =>
                {
                    MessageBox.Show($"Test {obj}", "alert");
                });
            }
        }

        //private void OnTest(object p)
        //{
        //    MessageBox.Show($"Test {p}", "alert");
        //}
    }
}