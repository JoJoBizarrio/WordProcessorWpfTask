using System.IO;
using System;
using System.Windows;
using Microsoft.Win32;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.ViewModel;

namespace WordProcessingWpfTask.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string _filter = "Text (*.txt)|*.txt|Word (*.docx)|*.docx|All files (*.*)|*.*";

        private OpenFileDialog _openFileDialog = new OpenFileDialog()
        {
            Title = "Choose file",
            InitialDirectory = Environment.CurrentDirectory,
            Filter = _filter
        };

        private SaveFileDialog _saveFileDialog = new SaveFileDialog()
        {
            Title = "Save file",
            DefaultExt = ".txt",
            AddExtension = true,
            CreatePrompt = false,
            OverwritePrompt = false,
            Filter = _filter,
            InitialDirectory = Environment.CurrentDirectory
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        async private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            var dataContext = (MainWindowViewModel)DataContext;

            if (!String.IsNullOrEmpty(dataContext.FilePath))
            {
                _openFileDialog.InitialDirectory = dataContext.FilePath;
            }

            if (_openFileDialog.ShowDialog() == false)
            {
                return;
            }

            await dataContext.OpenFileAsync(_openFileDialog.FileName);
        }

        async private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext == null)
            {
                return;
            }

            var dataContext = (MainWindowViewModel)DataContext;

            if (!String.IsNullOrEmpty(dataContext.FilePath))
            {
                _saveFileDialog.InitialDirectory = dataContext.FilePath;
            }

            if (_saveFileDialog.ShowDialog() == false)
            {
                return;
            }

            await dataContext.SaveFileAsync(_saveFileDialog.FileName);
        }
    }
}