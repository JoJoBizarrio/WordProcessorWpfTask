using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WordProcessingWpfTask.Model;

namespace WordProcessingWpfTask.Abstract
{
	public interface IRedactor
	{
		Task RemoveWordsParallelAsync(TextFile textFiles, int lettersCount);
		Task RemoveWordsParallelAsync(IEnumerable<TextFile> textFiles, int lettersCount);

		Task RemoveAllMarksParallelAsync(TextFile textFiles);
		Task RemoveAllMarksParallelAsync(IEnumerable<TextFile> textFiles);

		TextFile OpenFile(string path);
		void SaveFile(TextFile textFile, string path);

	}
}