using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WordProcessingWpfTask.Abstract
{
	public abstract class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string Property = null)
		{
			if (Equals(field, value))
			{
				return false;
			}

			field = value;
			OnPropertyChanged(Property);

			return true;
		}
	}
}
