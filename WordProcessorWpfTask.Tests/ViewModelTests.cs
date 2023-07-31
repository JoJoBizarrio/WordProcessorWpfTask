using Moq;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;
using WordProcessingWpfTask.ViewModel;

namespace WordProcessorWpfTask.Tests
{
	[TestFixture]
	[TestOf(typeof(MainWindowViewModel))]
	public class ViewModelTests
	{
		private MainWindowViewModel _viewModel;
		private Mock<IRedactor> _redactorMock;
		[SetUp]
		public void Setup()
		{
			
			
		}

		[Test]
		async public Task OpenAsync_ExpectedNewTextFileInCollection()
		{
			_redactorMock = new Mock<IRedactor>();
			var result = new Task<TextFile>(() => { return new TextFile(); });
			_redactorMock.Setup(redactor => redactor.OpenFileAsync(null)).Returns(result).Verifiable();
			_viewModel = new MainWindowViewModel(_redactorMock.Object);
			await _viewModel.OpenAsync.ExecuteAsync(string.Empty);

			//_redactorMock.Verify();
			Assert.IsTrue(_viewModel.TextFilesCollection.Count == 1);
		}

		[Test]
		async public Task RemoveMarks_Execute()
		{
			var textFiles = new TextFile[4] { new TextFile() { IsEditable = true }, new TextFile(), new TextFile(), new TextFile() };
			var expected = textFiles[0];
			var result = new Task<IEnumerable<TextFile>>(() => textFiles.Where(item => item.IsEditable == true));
			foreach (TextFile textFile in textFiles)
			{
				_viewModel.TextFilesCollection.Add(textFile);
			}

			_redactorMock.Setup(redactor => redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(null))
						 .Returns(result)
						 .Verifiable();

			//act
			await _viewModel.RemoveMarksAsync.ExecuteAsync();

			_redactorMock.Verify(x => x.RemoveAllMarksInSeveralTextFilesParallelAsync(null));
		}
	}
}