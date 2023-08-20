using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WordProcessingWpfTask.View
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public string FilePath
		{
			get => (string)GetValue(FilePathProperty);
			set => SetValue(FilePathProperty, value);
		}

		public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
			nameof(FilePath),
			typeof(string),
			typeof(MainWindow),
			new PropertyMetadata(default(string)));

		private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new OpenFileDialog()
			{
				Title = "Choose file",
				DefaultExt = ".txt",
				Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*"
			};

			if (openFileDialog.ShowDialog() == true)
			{
				FilePath = openFileDialog.FileName;
			}
			else
			{
				FilePath = null;
			}
		}

		private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
		{
			var saveFileDialog = new SaveFileDialog()
			{
				Title = "Save file",
				DefaultExt = ".txt",
				AddExtension = true,
				CreatePrompt = false,
				OverwritePrompt = false,
				Filter = "Text (*.txt)|*.txt|All files (*.*)|*.*"
			};

			if (saveFileDialog.ShowDialog() == true)
			{
				FilePath = saveFileDialog.FileName;
			}
			else
			{
				FilePath = null;
			}
		}
	}
}