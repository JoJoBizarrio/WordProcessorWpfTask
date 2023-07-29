using NUnit.Framework.Constraints;
using System.Security.Cryptography;
using WordProcessingWpfTask.Abstract;
using WordProcessingWpfTask.Model;
using WordProcessorWpfTask;

namespace WordProcessorWpfTask.Tests
{
	public class Tests
	{
		private IRedactor _redactor;

		[SetUp]
		public void Setup()
		{
			_redactor = new Redactor();
		}

		[TestCase('.')]
		[TestCase(',')]
		[TestCase('!')]
		[TestCase('?')]
		[TestCase(':')]
		[TestCase('(')]
		[TestCase(')')]
		[TestCase('\\')]
		[TestCase('"')]
		[TestCase(';')]
		[TestCase('-')]
		public void CheckRemoveAllMarks(char c)
		{
			// assing
			var textFile = new TextFile();
			textFile.Text = c.ToString();
			_redactor.IdKeyTextFileValueDictionary.Add(textFile.Id, textFile);

			// act
			var excepted = _redactor.RemoveAllMarksParallelAsync(textFile.Id).Result;

			// assert
			Assert.That(excepted, Is.EqualTo(string.Empty));
		}
	}
}