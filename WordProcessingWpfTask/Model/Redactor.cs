using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
	public class Redactor : IRedactor
	{
		public Redactor() { }

		#region operations of remove
		#region marks
		async public Task RemoveAllMarksParallelAsync(IEnumerable<TextFile> textFiles)
		{
			if (Equals(textFiles, null))
			{
				throw new ArgumentNullException(nameof(textFiles), "textFiles is null.");
			}

			if (textFiles.Count() == 0)
			{
				return;
			}

			var tasks = textFiles.Select(item => RemoveAllMarksParallelAsync(item));

			await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
		}

		async public Task RemoveAllMarksParallelAsync(TextFile textFile)
		{
			if (Equals(textFile, null))
			{
				throw new ArgumentNullException(nameof(textFile), "textFiles is null.");
			}

			var newTempPath = GetTempPath(textFile.TempFilePath);
			CreateTemp(textFile.TempFilePath);

			await Task.Run(async () =>
			{
				using var reader = new StreamReader(textFile.TempFilePath);
				using var writer = new StreamWriter(newTempPath);

				var currentLine = "";

				while ((currentLine = await reader.ReadLineAsync()) != null)
				{
					var filter = currentLine.ToCharArray().AsParallel().AsOrdered().Where(symbol => !Char.IsPunctuation(symbol));
					writer.WriteLine(new String(filter.ToArray()));
				}

				textFile.TempFilePath = newTempPath;
			});
		}
		#endregion

		#region words
		async public Task RemoveWordsParallelAsync(IEnumerable<TextFile> textFiles, int lettersCount)
		{
			if (Equals(textFiles, null))
			{
				throw new ArgumentNullException(nameof(textFiles), "textFiles is null.");
			}

			CheckLettersCount(lettersCount);

			var tasks = textFiles.Select(item => RemoveWordsParallelAsync(item, lettersCount));

			await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
		}

		async public Task RemoveWordsParallelAsync(TextFile textFile, int lettersCount)
		{
			if (Equals(textFile, null))
			{
				throw new ArgumentNullException(nameof(textFile), "textFile is null.");
			}

			CheckLettersCount(lettersCount);

			await Task.Run(() =>
			{
				var newTempPath = GetTempPath(textFile.TempFilePath);
				File.Copy(textFile.TempFilePath, newTempPath, true);

				using var reader = new StreamReader(textFile.TempFilePath);
				using var writer = new StreamWriter(newTempPath);

				var currentLine = "";

				while ((currentLine = reader.ReadLine()) != null)
				{
					writer.WriteLine(RemoveWords(currentLine, lettersCount));
				}

				textFile.TempFilePath = newTempPath;
			});
		}

		private string RemoveWords(string line, int lettersCount)
		{
			var chars = line.ToList();
			var i = 0;
			var wordLength = 1;

			while (i < chars.Count)
			{
				if (!char.IsLetter(chars[i]) && !char.IsNumber(chars[i]))
				{
					i++;
				}
				else
				{
					for (int j = 0; i + j + 1 < chars.Count && (char.IsLetter(chars[i + j + 1]) || char.IsNumber(chars[i + j + 1])); j++)
					{
						wordLength++;
					}

					if (wordLength <= lettersCount)
					{
						chars.RemoveRange(i, wordLength);
					}
					else
					{
						i += wordLength;
					}

					wordLength = 1;
				}
			}

			return new string(chars.ToArray());
		}
		#endregion
		#endregion

		#region operation with files
		public void SaveFile(TextFile textFile, string path)
		{
			CheckPath(path);

			File.Copy(textFile.TempFilePath, path, true);
			File.Delete(textFile.TempFilePath);
		}

		public TextFile OpenFile(string path)
		{
			CheckPath(path);

			if (!File.Exists(path))
			{
				throw new FileNotFoundException("File isnt exists by path.", Path.GetFileName(path));
			}

			var tempFilePath = GetTempPath(path);

			File.Copy(path, tempFilePath, true);

			var newTextFile = new TextFile()
			{
				Title = Path.GetFileNameWithoutExtension(path),
				FilePath = path,
				TempFilePath = tempFilePath,
				Extension = Path.GetExtension(path)
			};

			return newTextFile;
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

		#region other
		public string GetTempPath(string path)
		{
			var extension = Path.GetExtension(path);
			return Path.GetTempFileName().Replace(".tmp", extension);
		}

		public void CreateTemp(string path)
		{
			var newTempPath = GetTempPath(path);
			File.Copy(path, newTempPath, true);
		}
		#endregion
	}
}