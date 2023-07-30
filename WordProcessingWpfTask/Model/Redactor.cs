using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
	public class Redactor : IRedactor
	{
		public Redactor() { }

		private readonly IDictionary<Guid, TextFile> _idKeyTextFileValueDictionary = new Dictionary<Guid, TextFile>();

		public IEnumerable<TextFile> TextFiles => _idKeyTextFileValueDictionary.Values.ToImmutableArray();

		#region operations of remove
		#region marks
		async public Task<IEnumerable<TextFile>> RemoveAllMarksInSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray)
		{
			if (Equals(idArray, null))
			{
				throw new ArgumentNullException(nameof(idArray), "idArray is null.");
			}

			if (idArray.Count() == 0)
			{
				return Enumerable.Empty<TextFile>();
			}

			var tasks = idArray.Select(id => RemoveAllMarksParallelAsync(id));

			return await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
		}

		async public Task<TextFile> RemoveAllMarksParallelAsync(Guid id)
		{
			CheckId(id);

			return await Task.Run(() =>
			{
				var textFile = _idKeyTextFileValueDictionary[id];
				var textChars = textFile.Text.ToCharArray();
				var filter = textChars.AsParallel().AsOrdered().Where(symbol => !Char.IsPunctuation(symbol));
				textFile.Text = new String(filter.ToArray());

				return textFile;
			});
		}
		#endregion

		#region words
		async public Task<IEnumerable<TextFile>> RemoveWordsInSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray, int lettersCount)
		{
			if (idArray == null)
			{
				throw new ArgumentNullException(nameof(idArray), "idArray is null.");
			}

			CheckLettersCount(lettersCount);

			if (lettersCount == 0 || idArray.Count() == 0)
			{
				return Enumerable.Empty<TextFile>();
			}

			var tasks = idArray.Select(id => RemoveWordsParallelAsync(id, lettersCount));

			return await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
		}

		async public Task<TextFile> RemoveWordsParallelAsync(Guid id, int lettersCount)
		{
			CheckLettersCount(lettersCount);
			CheckId(id);

			return await Task.Run(() =>
			{
				var textFile = _idKeyTextFileValueDictionary[id];
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

				textFile.Text = stringBuilder.ToString();
				return textFile;
			});
		}
		#endregion
		#endregion

		#region operation with files
		async public Task SaveFileAsync(Guid id, string path)
		{
			CheckId(id);
			CheckPath(path);

			using (var writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
			{
				var textFile = _idKeyTextFileValueDictionary[id];
				await writer.WriteAsync(textFile.Text);

				textFile.FilePath = path;
				textFile.Title = Path.GetFileNameWithoutExtension(path);
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

				_idKeyTextFileValueDictionary.Add(newTextFile.Id, newTextFile);
				return newTextFile;
			}
		}
		#endregion

		#region validation
		private void CheckLettersCount(int lettersCount)
		{
			if (lettersCount < 0)
			{
				throw new ArgumentException($"Letters count is less zero and equal to {lettersCount}.", nameof(lettersCount));
			}
		}

		private void CheckId(Guid id)
		{
			if (!_idKeyTextFileValueDictionary.TryGetValue(id, out var _))
			{
				throw new ArgumentException($"Dictionary hasnt item by id = {id}.", nameof(id));
			}
		}

		private void CheckPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException(nameof(path));
			}

			if (!Path.IsPathRooted(path))
			{
				throw new ArgumentException($"Invalid path = {path}.", nameof(path));
			}
		}
		#endregion

		#region suppporting
		public void Add(TextFile textFile)
		{
			if (textFile == null)
			{
				throw new ArgumentNullException(nameof(textFile), "TextFile is null.");
			}

			if (!_idKeyTextFileValueDictionary.TryAdd(textFile.Id, textFile))
			{
				throw new ArgumentException("Same element already exists in redactor.", nameof(textFile));
			}
		}

		public bool Remove(Guid id)
		{
			return _idKeyTextFileValueDictionary.Remove(id);
		}
		#endregion
	}
}