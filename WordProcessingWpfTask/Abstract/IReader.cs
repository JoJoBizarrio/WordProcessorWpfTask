using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Abstract
{
	internal interface IReader
	{
		Task<int> ReadAsync(string path, byte[] bytes, long position, int count);

		IEnumerable<string> ReadLines(string path);
	}
}
