using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Model
{
	public interface IRedactor
	{
		Task<string> RemoveWordsParallelAsync(string text, int letterCount);

		Task<string> RemoveAllSeparatorsParallelAsync(string text);
	}
}