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

		[TestCase(".,!?:()\\\";-")]
		public void CheckRemoveAllMarks(string marks)
		{
			// assing
			var textFile = new TextFile();
			textFile.Text = marks;
			_redactor.Add(textFile);

			var expected = "";

			// act
			var actual = _redactor.RemoveAllMarksParallelAsync(textFile.Id).Result;

			// assert
			Assert.That(actual.Text, Is.EqualTo(expected));
		}
	}
}