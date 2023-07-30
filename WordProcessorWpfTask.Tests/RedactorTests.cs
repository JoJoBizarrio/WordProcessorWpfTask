using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;

namespace WordProcessorWpfTask.Tests
{
	public class RedactorTests
	{
		private IRedactor _redactor;

		[SetUp]
		public void Setup()
		{
			_redactor = new Redactor();
		}

		#region tests of RemoveMarksInSeveralTextFiles
		[TestCase(".,!?:()\\\";-", "")]
		[Category("RemoveMarksInSeveralTextFiles")]
		async public Task RemoveMarks_ExpectedStringEmpty(string marks, string expected)
		{
			// assing
			var textFiles = new TextFile[2];
			textFiles[0] = new TextFile() { Text = marks };
			textFiles[1] = new TextFile() { Text = marks };

			_redactor.Add(textFiles[0]);
			_redactor.Add(textFiles[1]);

			// act
			var actual = await _redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(textFiles.Select(textFile => textFile.Id));

			// assert
			Assert.IsTrue(actual.All(textFile => textFile.Text == expected));
		}

		[Test]
		[Category("RemoveMarksInSeveralTextFiles")]
		public void IdArrayIsNull_ThrowArgumentNullEception()
		{
			AsyncTestDelegate testDelegate = () => { return _redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(null); };

			Assert.ThrowsAsync<ArgumentNullException>(testDelegate);
		}

		[Test]
		[Category("RemoveMarksInSeveralTextFiles")]
		async public Task IdArrayIsEmpty_ExpectedEmptyArray()
		{
			var idEmptyArray = Enumerable.Empty<Guid>();
			var expected = Enumerable.Empty<TextFile>();

			var actual = await _redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(idEmptyArray);

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		[Category("RemoveMarksInSeveralTextFiles")]
		public void IdIncorrect_ThrowArgumentNullEception()
		{
			var guidArray = new Guid[] { new Guid() };

			AsyncTestDelegate testDelegate = () => { return _redactor.RemoveAllMarksInSeveralTextFilesParallelAsync(guidArray); };

			Assert.ThrowsAsync<ArgumentException>(testDelegate);
		}
		#endregion

		#region tests of RemoveWordsInSeveralTextFiles
		[Test]
		[Category("RemoveWordsInSeveralTextFiles")]
		public void IdArrayIsNull_ThrowArgumentNullException()
		{
			var minimalLetterCount = 1;
			AsyncTestDelegate testDelegate = () => { return _redactor.RemoveWordsInSeveralTextFilesParallelAsync(null, minimalLetterCount); };

			Assert.ThrowsAsync<ArgumentNullException>(testDelegate);
		}

		[Test]
		[Category("RemoveWordsInSeveralTextFiles")]
		public void LetterCountLessZero_ThrowArgumentException()
		{
			var illegalLetterCount = -1;
			var idArray = Enumerable.Empty<Guid>();

			AsyncTestDelegate testDelegate = () => { return _redactor.RemoveWordsInSeveralTextFilesParallelAsync(idArray, illegalLetterCount); };

			Assert.ThrowsAsync<ArgumentException>(testDelegate);
		}

		[Test]
		[Category("RemoveWordsInSeveralTextFiles")]
		async public Task LetterCountEqualToZero_ExpectedEmptyArray()
		{
			var letterCount = 0;
			var idArray = new Guid[] { Guid.NewGuid() };
			var expected = Enumerable.Empty<Guid>();

			var actual = await _redactor.RemoveWordsInSeveralTextFilesParallelAsync(idArray, letterCount);

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		[Category("RemoveWordsInSeveralTextFiles")]
		async public Task EmptyIdArray_ExpectedEmptyArray()
		{
			var letterCount = 1;
			var idArray = Enumerable.Empty<Guid>();
			var expected = Enumerable.Empty<Guid>();

			var actual = await _redactor.RemoveWordsInSeveralTextFilesParallelAsync(idArray, letterCount);

			Assert.That(actual, Is.EqualTo(expected));
		}
		#endregion
	}
}