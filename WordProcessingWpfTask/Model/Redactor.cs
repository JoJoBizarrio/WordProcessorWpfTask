using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Model
{
	internal class Redactor : IRedactor
	{
		public Redactor() { }

		public Task RemoveSeparatorsAsync(string text)
		{
			throw new NotImplementedException();
		}

		public Task RemoveWordsAsync(string text, int LetterCount)
		{
			throw new NotImplementedException();
		}
	}
}