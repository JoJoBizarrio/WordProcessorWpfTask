using System.Windows;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;
using WordProcessingWpfTask.View;
using WordProcessingWpfTask.ViewModel;

namespace WordProcessingWpfTask
{
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			MainWindow mainWindow = new MainWindow();
			IRedactor redactor = new Redactor();
			mainWindow.DataContext = new MainWindowViewModel(redactor);
			mainWindow.Show();
		}
	}
}