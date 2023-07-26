using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
	internal class Redactor : IRedactor
	{
		public Redactor() { }

		public Task<string> RemoveAllMarksParallelAsync(string text)
		{
			return Task.Run(() =>
			{
				var textChars = text.ToCharArray();
				var filter = textChars.AsParallel().AsOrdered().Where(symbol => !Char.IsPunctuation(symbol));
				var result = new String(filter.ToArray());
				return result;
			});
		}

		public Task<string> RemoveWordsParallelAsync(string text, int lettersCount)
		{
			return Task.Run(() =>
			{
				var stringBuilder = new StringBuilder(text);
				var i = 0;
				var wordLength = 1;

				while (i < stringBuilder.Length)
				{
					if (!Char.IsLetter(stringBuilder[i]))
					{
						i++;
					}
					else
					{
						for (int j = 0; i + j + 1 < stringBuilder.Length && Char.IsLetter(stringBuilder[i + j + 1]); j++)
						{
							wordLength++;
						}

						if (wordLength <= lettersCount)
						{
							stringBuilder.Remove(i, wordLength);
						}
						else
						{
							i += wordLength;
						}

						wordLength = 1;
					}
				}

				return stringBuilder.ToString();
			});
		}

		async public Task SaveFileAsync(string path, string text)
		{
			using (var writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
			{
				await writer.WriteAsync(text);
			}
		}

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
	}
}