using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Model
{
	internal class TextFile : INotifyPropertyChanged
	{
		public Guid Id { get; private set; } = Guid.NewGuid();

		private bool _isEditable;
		public bool IsEditable
		{
			get => _isEditable;
			set
			{
				_isEditable = value;
				OnPropertyChanged();
			}
		}

		private string _title;
		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				OnPropertyChanged();
			}
		}

		private string _text;
		public string Text
		{
			get => _text;
			set
			{
				_text = value;   // bad solution timerCallBack? 
				OnPropertyChanged();
			}
		}

		private string _filePath;
		public string FilePath
		{
			get => _filePath;
			set
			{
				_filePath = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}