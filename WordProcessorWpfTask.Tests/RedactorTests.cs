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
			Assert.That(actual.TextContents == expected, Is.True);
		}

		[TestCase("five5\r\n", 100, "\r\n")] //rn because writeLine in redactor
		[TestCase("$#five5&4\r\n", 100, "$#&\r\n")]
		[TestCase("five5\r\n", 1, "five5\r\n")]
		async public Task RemoveWords(string input, int lettersCount, string expected)
		{
			// assing
			var mockInputFile = new MockFileData(input);
			var tempPathFile = "C:\\temp\\tempFile.txt";
			_mockFileSystem.AddFile(tempPathFile, mockInputFile);
			var textFile = new TextFile() { TempFilePath = tempPathFile };
			var list = new List<TextFile>
			{
				textFile
			};

			// act
			await _redactor.RemoveWordsParallelAsync(list, lettersCount);

			// assert
			var actual = _mockFileSystem.GetFile(textFile.TempFilePath);
			Assert.That(actual.TextContents == expected, Is.True);
		}

		[Test]
		public void OpenFile_ExpectedNewTextFile()
		{
			var mockInputFile = new MockFileData("new file");
			var tempPathFile = "C:\\temp\\tempFile.txt";
			_mockFileSystem.AddFile(tempPathFile, mockInputFile);
			_mockFileSystem.AddDirectory(Path.GetTempPath());

			var actual = _redactor.OpenFile(tempPathFile);

			Assert.That(actual.FilePath == tempPathFile && actual.Title == "tempFile" && _mockFileSystem.FileExists(actual.TempFilePath), Is.True);
		}

		[Test]
		public void SaveFile_ExpectedNewFile() // should separate settings of MockFileSystem inside Setup
		{
			var mockInputFile = new MockFileData("empty");
			var tempPathFile = "C:\\temp\\tempFile.txt";
			_mockFileSystem.AddFile(tempPathFile, mockInputFile);

			var textFile = new TextFile()
			{
				Title = "newFile",
				TempFilePath = tempPathFile
			};

			var destinationFolder = "C:\\FolderWithTextFiles";
			var destinationPath = "C:\\FolderWithTextFiles\\tempFile.txt";
			_mockFileSystem.AddDirectory(destinationFolder);

			//act
			_redactor.SaveFile(textFile, destinationPath);

			Assert.That(_mockFileSystem.FileExists(destinationPath), Is.True);
		}
	}
}