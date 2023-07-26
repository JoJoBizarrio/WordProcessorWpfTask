using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
	internal interface IRedactor
	{
		Task<string> RemoveWordsParallelAsync(string text, int letterCount);

		Task<string> RemoveAllMarksParallelAsync(string text);

		Task<TextFile> OpenFileAsync(string path);

		Task SaveFileAsync(string path, string text);
	}
}