using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
	internal class Repository : IRepository
	{
		async public Task<TextFile> OpenFileAsync(string path)
		{
			using (StreamReader streamReader = new StreamReader(path))
			{
				var temp = await streamReader.ReadToEndAsync();
				var newTextFile = new TextFile()
				{
					Title = path.Remove(path.LastIndexOf('.')).Substring(path.LastIndexOf('\\') + 1),
					Text = temp,
					FilePath = path
				};

				return newTextFile;
			}
		}

		async public Task SaveFileAsync(string path, string text)
		{
			using (var writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
			{
				await writer.WriteAsync(text);
			}
		}
	}
}