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
        Task<string> RemoveWordsParallelAsync(Guid id, int letterCount);

        Task<string> RemoveAllMarksParallelAsync(Guid id);

        Task RemoveWordsInsideSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray, int letterCount);


        Task<TextFile> OpenFileAsync(string path);

        Task SaveFileAsync(Guid id, string path);

        bool Remove(Guid id);
    }
}