using System.Windows;
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
			mainWindow.DataContext = new MainWindowViewModel();
			mainWindow.Show();
		}
	}
}