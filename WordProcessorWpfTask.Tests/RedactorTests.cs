using System.IO.Abstractions.TestingHelpers;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;

namespace WordProcessorWpfTask.Tests
{
	public class RedactorTests
	{
		private IRedactor _redactor;
		private MockFileSystem _mockFileSystem;

		[SetUp]
		public void Setup()
		{
			_mockFileSystem = new MockFileSystem();
			_redactor = new Redactor(_mockFileSystem);
		}

		#region tests of RemoveMarksInSeveralTextFiles

		[Test]
		async public Task RemoveMarks_ExpectedStringEmpty()
		{
			// assing
			var expected = "\r\n"; //rn becuase writeLine in redactor
			var mockInputFile = new MockFileData(".,!?:()\\\";-");
			var tempPathFile = "C:\\temp\\tempFile.txt";
			_mockFileSystem.AddFile(tempPathFile, mockInputFile);
			var textFile = new TextFile() { TempFilePath = tempPathFile };
			var list = new List<TextFile>
			{
				textFile
			};

			// act
			await _redactor.RemoveAllMarksParallelAsync(list);

			// assert
			var actual = _mockFileSystem.GetFile(textFile.TempFilePath);
			Assert.IsTrue(actual.TextContents == expected);
		}

		[Test]
		async public Task RemoveWords_ExpectedStringEmpty()
		{
			// assing
			var expected = "\r\n"; //rn becuase writeLine in redactor
			var mockInputFile = new MockFileData("five5");
			var tempPathFile = "C:\\temp\\tempFile.txt";
			_mockFileSystem.AddFile(tempPathFile, mockInputFile);
			var textFile = new TextFile() { TempFilePath = tempPathFile };
			var list = new List<TextFile>
			{
				textFile
			};

			// act
			await _redactor.RemoveWordsParallelAsync(list, 5);

			// assert
			var actual = _mockFileSystem.GetFile(textFile.TempFilePath);
			Assert.IsTrue(actual.TextContents == expected);
		}

		//[TestCase(".,!?:()\\\";-", "")]
		//[Category("RemoveMarksInSeveralTextFiles")]
		//async public Task RemoveMarks_ExpectedStringEmpty(string marks, string expected)
		//{
		//	// assing
		//	var textFiles = new TextFile[2];
		//	textFiles[0] = new TextFile() { Text = marks };
		//	textFiles[1] = new TextFile() { Text = marks };

		//	_redactor.Add(textFiles[0]);
		//	_redactor.Add(textFiles[1]);

		//	// act
		//	var actual = await _redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(textFiles.Select(textFile => textFile.Id));

		//	// assert
		//	Assert.IsTrue(actual.All(textFile => textFile.Text == expected));
		//}

		//[Test]
		//[Category("RemoveMarksInSeveralTextFiles")]
		//public void IdArrayIsNull_ThrowArgumentNullEception()
		//{
		//	AsyncTestDelegate testDelegate = async () => { await _redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(null); };

		//	Assert.ThrowsAsync<ArgumentNullException>(testDelegate);
		//}

		//[Test]
		//[Category("RemoveMarksInSeveralTextFiles")]
		//async public Task IdArrayIsEmpty_ExpectedEmptyArray()
		//{
		//	var idEmptyArray = Enumerable.Empty<Guid>();
		//	var expected = Enumerable.Empty<TextFile>();

		//	var actual = await _redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(idEmptyArray);

		//	Assert.That(actual, Is.EqualTo(expected));
		//}

		//[Test]
		//[Category("RemoveMarksInSeveralTextFiles")]
		//public void IdIncorrect_ThrowArgumentNullEception()
		//{
		//	var guidArray = new Guid[] { new Guid() };

		//	AsyncTestDelegate testDelegate = async () => { await _redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(guidArray); };

		//	Assert.ThrowsAsync<ArgumentException>(testDelegate);
		//}
		//#endregion

		//#region tests of RemoveWordsInSeveralTextFiles
		//[Test]
		//[Category("RemoveWordsInSeveralTextFiles")]
		//public void IdArrayIsNull_ThrowArgumentNullException()
		//{
		//	var minimalLetterCount = 1;
		//	AsyncTestDelegate testDelegate = async () => { await _redactor.RemoveWordsInSeveralTextFilesParallelAsync(null, minimalLetterCount); };

		//	Assert.ThrowsAsync<ArgumentNullException>(testDelegate);
		//}

		//[Test]
		//[Category("RemoveWordsInSeveralTextFiles")]
		//public void LetterCountLessZero_ThrowArgumentException()
		//{
		//	var illegalLetterCount = -1;
		//	var idArray = Enumerable.Empty<Guid>();

		//	AsyncTestDelegate testDelegate = async () => { await _redactor.RemoveWordsInSeveralTextFilesParallelAsync(idArray, illegalLetterCount); };

		//	Assert.ThrowsAsync<ArgumentException>(testDelegate);
		//}

		//[Test]
		//[Category("RemoveWordsInSeveralTextFiles")]
		//async public Task LetterCountEqualToZero_ExpectedEmptyArray()
		//{
		//	var letterCount = 0;
		//	var idArray = new Guid[] { Guid.NewGuid() };
		//	var expected = Enumerable.Empty<Guid>();

		//	var actual = await _redactor.RemoveWordsInSeveralTextFilesParallelAsync(idArray, letterCount);

		//	Assert.That(actual, Is.EqualTo(expected));
		//}

		//[Test]
		//[Category("RemoveWordsInSeveralTextFiles")]
		//async public Task EmptyIdArray_ExpectedEmptyArray()
		//{
		//	var letterCount = 1;
		//	var idArray = Enumerable.Empty<Guid>();
		//	var expected = Enumerable.Empty<Guid>();

		//	var actual = await _redactor.RemoveWordsInSeveralTextFilesParallelAsync(idArray, letterCount);

		//	Assert.That(actual, Is.EqualTo(expected));
		//}

		//[Category("RemoveWordsInSeveralTextFiles")]
		//[TestCase("test start, testWord, test end", " start, testWord,  ", 4)]
		//[TestCase("test start, testWord, test end", " , ,  ", 1001)]
		//async public Task RemoveWords(string words, string expected, int lettersCount)
		//{
		//	var textFiles = new TextFile[] { new TextFile() { Text = words } };
		//	_redactor.Add(textFiles[0]);

		//	var actual = await _redactor.RemoveWordsInSeveralTextFilesParallelAsync(textFiles.Select(item => item.Id), lettersCount);

		//	Assert.That(actual.First().Text, Is.EqualTo(expected));
		//}
		//#endregion

		//#region operation with files
		//[Test]
		//[Category("OperationWithFile")]
		//async public Task SaveFileAsync()
		//{
		//	var textFile = new TextFile()
		//	{
		//		Title = "test save",
		//		FilePath = Environment.CurrentDirectory + "\\test save.txt",
		//		Text = "run test of saveAsync"
		//	};

		//	_redactor.Add(textFile);

		//	await _redactor.SaveFileAsync(textFile.Id, textFile.FilePath);

		//	Assert.That(File.Exists(textFile.FilePath), Is.True);
		//}

		//[Test]
		//[Category("OperationWithFile")]
		//async public Task OpenFileAsync()
		//{
		//	await SaveFileAsync();
		//	var path = Environment.CurrentDirectory + "\\test save.txt";

		//	var textFile = await _redactor.OpenFileAsync(path);

		//	Assert.That(File.Exists(textFile.FilePath), Is.True);
		//}

		//[Test]
		//[Category("OperationWithFile")]
		//public void OpenFileAsync_IncorrectPath_ThrowFileNotFoundException()
		//{
		//	var wrongPath = Environment.CurrentDirectory;

		//	AsyncTestDelegate testDelegate = async () => { await _redactor.OpenFileAsync(wrongPath); }; //async Task testDelegate() { await _redactor.OpenFileAsync(wrongPath); } wtf?

		//	Assert.ThrowsAsync<FileNotFoundException>(testDelegate);
		//}
		#endregion
	}
}