using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Model
{
	internal class Redactor : IRedactor
	{
		public string Text { get; private set; }

		public Redactor() { }

		public Redactor(string text)
		{
			Text = text;
		}

		Task IRedactor.ReadFileAsync(string path)
		{
			throw new NotImplementedException();
		}

		Task IRedactor.RemoveSeparatorsAsync()
		{
			throw new NotImplementedException();
		}

		Task IRedactor.RemoveWordsAsync(int LetterCount)
		{
			throw new NotImplementedException();
		}
	}
}