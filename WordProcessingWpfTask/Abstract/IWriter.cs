using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Abstract
{
	internal interface IWriter
	{
		Task WriteAsync(string path, string value);

		void Write(string path, string text);
	}
}
