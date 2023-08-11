using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WordProcessingWpfTask.Model
{
	public class TextFile : INotifyPropertyChanged
	{
		public TextFile() { }

		public Guid Id { get; } = Guid.NewGuid();

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
				_text = value;
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

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (ReferenceEquals(null, obj) || obj.GetType() != GetType())
			{
				return false;
			}

			var textFile = (TextFile)obj;

			return Id == textFile.Id && FilePath == textFile.FilePath;
		}

		public override int GetHashCode()
		{
			var prime = 73;
			var hash = 37;

			return prime * hash + Id.GetHashCode();
		}
	}
}