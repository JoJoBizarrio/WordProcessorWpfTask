using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Model
{
	internal interface IRedactor
	{
		Task ReadFileAsync(string path);

		Task RemoveWordsAsync(int LetterCount);

		Task RemoveSeparatorsAsync();
	}
}