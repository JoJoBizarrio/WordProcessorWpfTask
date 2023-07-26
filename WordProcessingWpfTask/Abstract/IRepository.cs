using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Model;

namespace WordProcessingWpfTask.Abstract
{
	internal interface IRepository
	{
		Task SaveFileAsync(string path, string text);

		Task<TextFile> OpenFileAsync(string path);
	}
}
