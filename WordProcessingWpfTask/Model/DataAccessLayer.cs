using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
	internal class DataAccessLayer : IDataAccessLayer
	{
		async public Task<int> ReadAsync(string path, byte[] bytes, long position, int count)
		{
			using var reader = new BufferedStream(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true));
			reader.Position = position;

			return await reader.ReadAsync(bytes, 0, count);
		}

		async public Task WriteAsync(string path, string value)
		{
			using var writer = new StreamWriter(path);
			await writer.WriteAsync(value);
		}

		public IEnumerable<string> ReadLines(string path)
		{
			return File.ReadLines(path);
		}

		public void Write(string path, string text)
		{
			File.AppendAllText(path, text);
		}
	}
}