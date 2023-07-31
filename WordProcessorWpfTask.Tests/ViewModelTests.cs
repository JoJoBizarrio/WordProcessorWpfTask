using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.ViewModel;

namespace WordProcessorWpfTask.Tests
{
	[TestFixture]
	public class ViewModelTests
	{
		private MainWindowViewModel _viewModel;

		[SetUp]
		public void Setup()
		{
			_viewModel = new MainWindowViewModel(null);
		}
	}
}