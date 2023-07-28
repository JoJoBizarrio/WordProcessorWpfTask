using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Model;

namespace WordProcessingWpfTask.Abstract
{
	internal interface IRedactor
	{
		Task<string> RemoveWordsParallelAsync(Guid id, int lettersCount);
		Task RemoveWordsInsideSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray, int lettersCount);

		Task<string> RemoveAllMarksParallelAsync(Guid id);
		Task RemoveAllMarksInsideSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray);

		Task<TextFile> OpenFileAsync(string path);

		Task SaveFileAsync(Guid id, string path);

		bool Remove(Guid id);
	}
}