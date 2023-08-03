using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WordProcessingWpfTask.Model;

namespace WordProcessingWpfTask.Abstract
{
	public interface IRedactor
	{
		IEnumerable<TextFile> TextFiles { get; }
		Task<TextFile> RemoveWordsParallelAsync(Guid id, int lettersCount);
		Task<IEnumerable<TextFile>> RemoveWordsInSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray, int lettersCount);

		Task<TextFile> RemoveAllMarksParallelAsync(Guid id);
		Task<IEnumerable<TextFile>> RemoveAllMarksInSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray);

		Task<TextFile> OpenFileAsync(string path);
		Task SaveFileAsync(Guid id, string path);

		void Add(TextFile textFile);
		bool Remove(Guid id);
	}
}