using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Model;
using WordProcessingWpfTask.ViewModel;

namespace WordProcessorWpfTask.Tests
{
	[TestFixture]
	[TestOf(typeof(TextFile))]
	public class TextFileTests
	{
		[Test]
		public void GetHashCode_ExpectedMore50percentUniqueValues()
		{
			var lengthSample = (long)Math.Pow(10, 7);
			var textFilesArray = new TextFile[lengthSample];
			var hashArray = new int[lengthSample];

			for (int i = 0; i < lengthSample; i++)
			{
				textFilesArray[i] = new TextFile();
				hashArray[i] = textFilesArray[i].GetHashCode();
			}

			Array.Sort(hashArray);

			int differentValues = 0;
			int currentValue = hashArray[0];

			for (int i = 1; i < lengthSample; i++)
			{
				if (hashArray[i] != currentValue)
				{
					differentValues++;
					currentValue = hashArray[i];
				}
			}

			Assert.That(differentValues, Is.GreaterThan(lengthSample / 2)); // on 10^4 => 1times, on 10^5 => 3times, on 10^6 => 126times
		}

		[Test]
		public void Equals_ItemsIsEqual_ExpectedTrue()
		{
			var textFile1 = new TextFile();
			var textFile2 = textFile1;

			var actual = textFile1.Equals(textFile2);

			Assert.That(actual, Is.True);
		}

		[Test]
		public void Equals_ObjIsNull_ExpectedFalse()
		{
			var textFile = new TextFile();

			var actual = textFile.Equals(null);

			Assert.That(actual, Is.False);
		}

		[Test]
		public void Equals_ObjHasAnotherType_ExpectedFalse()
		{
			var textFile = new TextFile();

			var actual = textFile.Equals(new object());

			Assert.That(actual, Is.False);
		}

		[Test]
		public void Equals_DifferentItem_ExpectedFalse()
		{
			var textFile1 = new TextFile();
			var textFile2 = new TextFile();

			var actual = textFile1.Equals(textFile2);

			Assert.That(actual, Is.False);
		}
	}
}
