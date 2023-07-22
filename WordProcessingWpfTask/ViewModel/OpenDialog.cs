using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WordProcessingWpfTask.ViewModel
{
	internal class OpenDialog : Freezable
	{
		public string Title
		{
			get => (string)GetValue(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
			nameof(Title),
			typeof(string),
			typeof(OpenDialog),
			new PropertyMetadata(default(string)));

		public string Filter
		{
			get => (string)GetValue(FilterProperty);
			set => SetValue(FilterProperty, value);
		}

		public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
			nameof(Filter),
			typeof(string),
			typeof(OpenDialog),
			new PropertyMetadata("Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*)"));

		public string SelectedFile
		{
			get => (string)GetValue(SelectedFileProperty);
			set => SetValue(TitleProperty, value);
		}

		public static readonly DependencyProperty SelectedFileProperty = DependencyProperty.Register(
			nameof(SelectedFile),
			typeof(string),
			typeof(OpenDialog),
			new PropertyMetadata(default(string)));

		private ICommand _open;

		public OpenDialog()
		{
			_open = new RelayCommand(OnOpenCommandExecuted);
		}

		private void OnOpenCommandExecuted(object obj)
		{
			var openFileDialog = new OpenFileDialog
			{
				Title = Title,
				Filter = Filter,
				RestoreDirectory = true,
				InitialDirectory = Environment.CurrentDirectory
			};

			if (openFileDialog.ShowDialog() == false)
			{
				return;
			}

			SelectedFile = openFileDialog.FileName;
		}

		protected override Freezable CreateInstanceCore()
		{
			return new OpenDialog();
		}
	}
}