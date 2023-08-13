using Moq;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;
using WordProcessingWpfTask.ViewModel;

namespace WordProcessorWpfTask.Tests
{
    [TestFixture]
    [TestOf(typeof(MainWindowViewModel))]
    //public class ViewModelTests
    //{
    //    private MainWindowViewModel _viewModel;
    //    private Mock<IRedactor> _redactorMock;
    //    [SetUp]
    //    public void Setup()
    //    {
    //        _redactorMock = new Mock<IRedactor>();
    //        _viewModel = new MainWindowViewModel(_redactorMock.Object);
    //    }

    //    [Test]
    //    [Category("Operation with files")]
    //    public void OnOpenAsync_ExpectedNewFileInCollection()
    //    {
    //        _redactorMock.Setup(redactor => redactor.OpenFileAsync(string.Empty).Result).Returns(new TextFile() { Text = "test" }).Verifiable();

    //        _viewModel.OpenAsync.ExecuteAsync(string.Empty);

    //        _redactorMock.Verify();
    //        Assert.That(_viewModel.TextFilesCollection, Is.Not.Empty);
    //    }

    //    [Test]
    //    [Category("Operation with files")]
    //    public void OnSaveAsync()
    //    {
    //        _viewModel.SelectedTextFile = new TextFile();
    //        _redactorMock.Setup(redactor => redactor.SaveFileAsync(_viewModel.SelectedTextFile.Id, string.Empty));

    //        _viewModel.SaveAsync.ExecuteAsync(string.Empty);

    //        _redactorMock.Verify();
    //    }

    //    [Test]
    //    [Category("RemoveMarks")]
    //    public void OnRemoveMarksExecute()
    //    {
    //        //assign
    //        var textFiles = new TextFile[] { new TextFile() { IsEditable = true }, new TextFile()};
    //        var idArray = textFiles.Where(textFile => textFile.IsEditable).Select(textFile => textFile.Id).ToArray();

    //        foreach (TextFile textFile in textFiles)
    //        {
    //            _viewModel.TextFilesCollection.Add(textFile);
    //        }

    //        _redactorMock.Setup(redactor => redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(idArray).Result).Verifiable();

    //        //act
    //        _viewModel.RemoveMarksAsync.ExecuteAsync();

    //        //assert
    //        _redactorMock.Verify();
    //    }

    //    [Test]
    //    [Category("RemoveMarks")]
    //    public void OnRemoveMarksCanExecute_Allright_ExpectedTrue()
    //    {
    //        _viewModel.TextFilesCollection.Add(new TextFile() { IsEditable = true });

    //        var actual = _viewModel.RemoveMarksAsync.CanExecute(null);

    //        Assert.That(actual, Is.True);
    //    }

    //    [Test]
    //    [Category("RemoveWords")]
    //    public void OnRemoveWordsExecute()
    //    {
    //        //assign
    //        var textFiles = new TextFile[] { new TextFile() { IsEditable = true }, new TextFile() };
    //        var idArray = textFiles.Where(textFile => textFile.IsEditable).Select(textFile => textFile.Id).ToArray();
    //        var minLettersCount = 1;

    //        _viewModel.LettersCountText = minLettersCount.ToString();

    //        foreach (TextFile textFile in textFiles)
    //        {
    //            _viewModel.TextFilesCollection.Add(textFile);
    //        }

    //        _redactorMock.Setup(redactor => redactor.RemoveWordsInSeveralTextFilesParallelAsync(idArray, minLettersCount).Result).Verifiable();

    //        //act
    //        _viewModel.RemoveWordsAsync.ExecuteAsync();

    //        //assert
    //        _redactorMock.Verify();
    //    }

    //    [Test]
    //    [Category("RemoveWords")]
    //    public void OnRemoveWordsCanExecute_Allright_ExpectedTrue()
    //    {
    //        var minLetterCount = 1;
    //        _viewModel.LettersCountText = minLetterCount.ToString();
    //        _viewModel.TextFilesCollection.Add(new TextFile() { IsEditable = true });

    //        var actual = _viewModel.RemoveWordsAsync.CanExecute(null);

    //        Assert.That(actual, Is.True);
    //    }
    }
}