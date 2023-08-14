using NLog;
using System.Threading.Tasks;
using System;
using System.Windows;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;
using WordProcessingWpfTask.View;
using WordProcessingWpfTask.ViewModel;
using System.IO.Abstractions;

namespace WordProcessingWpfTask
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            IRedactor redactor = new Redactor(new FileSystem());
            mainWindow.DataContext = new MainWindowViewModel(redactor);
            mainWindow.Show();

            SetupExceptionHandling();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                _logger.Error(exception, message);
            }
        }
    }
}