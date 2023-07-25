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
        public string MyProp
        {
            get => (string)GetValue(MyPropDP);
            set => SetValue(MyPropDP, value);
        }

        public static readonly DependencyProperty MyPropDP = DependencyProperty.Register(
            nameof(MyProp),
            typeof(string),
            typeof(MainWindow),
            new PropertyMetadata(default(string)));

        public MainWindow()
        {
            InitializeComponent();
        }

        async private void OpenMenuItem_Click(object sender, RoutedEventArgs e) // after takeout UIDialog here maybe got memory leak
        {
            if (DataContext == null)
            {
                return;
            }

            var dataContext = (MainWindowViewModel)DataContext;

            var openFileDialog = new OpenFileDialog()
            {
                Title = "Choose file",
                Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }

            await dataContext.OpenFileAsync(openFileDialog.FileName);
        }

        async private void SaveMenuItem_Click(object sender, RoutedEventArgs e) // after takeout UIDialog here maybe got memory leak
        {
            if (DataContext == null)
            {
                return;
            }

            var dataContext = (MainWindowViewModel)DataContext;

            var saveFileDialog = new SaveFileDialog()
            {
                Title = "Save file",
                DefaultExt = ".txt",
                AddExtension = true,
                CreatePrompt = false,
                OverwritePrompt = false,
                Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (saveFileDialog.ShowDialog() == false)
            {
                return;
            }

            await dataContext.SaveFileAsync(saveFileDialog.FileName);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            MyProp = "test click";
        }
    }
}