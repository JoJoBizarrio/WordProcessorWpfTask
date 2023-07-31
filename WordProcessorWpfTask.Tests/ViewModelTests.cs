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
			_redactorMock = new Mock<IRedactor>();
			_viewModel = new MainWindowViewModel(_redactorMock.Object);
		}

		[Test]
		async public Task OpenAsync_ExpectedNewTextFileInCollection()
		{
			var result = new Task<TextFile>(() => { return new TextFile() { Text = "mytext" }; });
			_redactorMock.Setup(redactor => redactor.OpenFileAsync(string.Empty)).Verifiable();
			_viewModel.OpenAsync.Execute(string.Empty);

			_redactorMock.Verify();
			//Assert.IsTrue(_viewModel.TextFilesCollection.Count == 1);
		}

		[Test]
		async public Task RemoveMarks_Execute()
		{
			var textFiles = new TextFile[4] { new TextFile() { IsEditable = true }, new TextFile(), new TextFile(), new TextFile() };
			var expected = textFiles[0];
			var result = new Task<IEnumerable<TextFile>>(() => textFiles.Where(item => item.IsEditable == true));

			//foreach (TextFile textFile in textFiles)
			//{
			//	_viewModel.TextFilesCollection.Add(textFile);
			//}

			//_redactorMock.Setup(redactor => redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(textFiles.Select(item => item.Id)))
			//			 .Returns(result)
			//			 .Verifiable();
			var idArray = textFiles.Select(item => item.Id).ToArray();
			//_redactorMock.Setup(redactor => redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(idArray)).Verifiable();

			//act
			_viewModel.RemoveMarksAsync.Execute(null);

			_redactorMock.Verify(redactor => redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(Array.Empty<Guid>()));
			var temp = _viewModel.TextFilesCollection.ToArray();
		}
	}
}