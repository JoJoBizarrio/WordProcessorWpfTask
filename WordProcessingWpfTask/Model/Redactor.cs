using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
	internal class Redactor : IRedactor
	{
		public Redactor()
		{
			IdKeyTextFileValueDictionary = new Dictionary<Guid, TextFile>();
		}

		public IDictionary<Guid, TextFile> IdKeyTextFileValueDictionary { get; private set; }

		async public Task RemoveAllMarksInsideSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray)
		{
			if (idArray == null)
			{
				throw new ArgumentNullException("idArray is null.");
			}

			if (idArray.Count() == 0)
			{
				return;
			}

			var tasks = idArray.Select(id => RemoveAllMarksParallelAsync(id));

			await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
		}

		async public Task<string> RemoveAllMarksParallelAsync(Guid id)
		{
			CheckId(id);

			return await Task.Run(() =>
			{
				var textFile = IdKeyTextFileValueDictionary[id];
				var textChars = textFile.Text.ToCharArray();
				var filter = textChars.AsParallel().AsOrdered().Where(symbol => !Char.IsPunctuation(symbol));
				var result = new String(filter.ToArray());

				IdKeyTextFileValueDictionary[id].Text = result;
				return result;
			});
		}

		async public Task RemoveWordsInsideSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray, int lettersCount)
		{
			if (idArray == null)
			{
				throw new ArgumentNullException("idArray is null.");
			}

			if (lettersCount <= 0)
			{
				throw new ArgumentException($"Letters count is equal to {lettersCount} less zero.", "lettersCount");
			}

			if (idArray.Count() == 0)
			{
				return;
			}

			var tasks = idArray.Select(id => RemoveWordsParallelAsync(id, lettersCount));

			await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
		}

		async public Task<string> RemoveWordsParallelAsync(Guid id, int lettersCount)
		{
			if (lettersCount <= 0)
			{
				throw new ArgumentException($"Letters count is equal to {lettersCount} less zero.", "letterCount");
			}

			CheckId(id);

			return await Task.Run(() =>
			{
				var textFile = IdKeyTextFileValueDictionary[id];
				var stringBuilder = new StringBuilder(textFile.Text);
				var i = 0;
				var wordLength = 1;

				while (i < stringBuilder.Length)
				{
					if (!char.IsLetter(stringBuilder[i]))
					{
						i++;
					}
					else
					{
						for (int j = 0; i + j + 1 < stringBuilder.Length && char.IsLetter(stringBuilder[i + j + 1]); j++)
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

				var result = stringBuilder.ToString();
				textFile.Text = result;
				return result;
			});
		}

		async public Task SaveFileAsync(Guid id, string path)
		{
			CheckId(id);
			CheckPath(path);

			using (var writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
			{
				var textFile = IdKeyTextFileValueDictionary[id];
				await writer.WriteAsync(textFile.Text);

				textFile.FilePath = path;
				textFile.Title = Path.GetFileNameWithoutExtension(path); //path.Remove(path.LastIndexOf('.')).Substring(path.LastIndexOf('\\') + 1);
			}
		}

		async public Task<TextFile> OpenFileAsync(string path) //TODO: TextFileDTO ?
		{
			CheckPath(path);

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File isnt exists by path.", Path.GetFileName(path));
			}

			using (StreamReader streamReader = new StreamReader(path))
			{
				var temp = await streamReader.ReadToEndAsync();
				var newTextFile = new TextFile()
				{
					Title = Path.GetFileNameWithoutExtension(path),
					Text = temp,
					FilePath = path
				};

				IdKeyTextFileValueDictionary.Add(newTextFile.Id, newTextFile);
				return newTextFile;
			}
		}

		public bool Remove(Guid id)
		{
			return IdKeyTextFileValueDictionary.Remove(id);
		}

		private void CheckId(Guid id)
		{
			if (!IdKeyTextFileValueDictionary.TryGetValue(id, out var _))
			{
				throw new ArgumentException($"Dictionary hasnt item by id = {id}.", "id");
			}
		}

		private void CheckPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			if (!Path.IsPathRooted(path))
			{
				throw new ArgumentException($"Invalid path = {path}.", "path");
			}
		}
	}
}