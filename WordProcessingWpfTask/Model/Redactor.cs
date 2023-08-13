﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
	public class Redactor : IRedactor
	{
		private readonly IDataAccessLayer _dataAccessLayer = new DataAccessLayer();

		public Redactor() { }

		#region operations of remove
		#region marks
		async public Task RemoveAllMarksParallelAsync(IEnumerable<TextFile> textFiles)
		{
			if (Equals(textFiles, null))
			{
				throw new ArgumentNullException(nameof(textFiles), "textFiles is null.");
			}

			if (!textFiles.Any())
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

				File.Delete(textFile.TempFilePath);
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

		//async public Task RemoveWordsParallelAsync(TextFile textFile, int lettersCount)
		//{
		//	if (Equals(textFile, null))
		//	{
		//		throw new ArgumentNullException(nameof(textFile), "textFile is null.");
		//	}

		//	CheckLettersCount(lettersCount);
		//	var newTempPath = GetTempPath(textFile.TempFilePath);
		//	File.Copy(textFile.TempFilePath, newTempPath, true);

		//	await Task.Run(() =>
		//	{
		//		using var reader = new StreamReader(textFile.TempFilePath);
		//		using var writer = new StreamWriter(newTempPath);

		//		var currentLine = "";

		//		while ((currentLine = reader.ReadLine()) != null)
		//		{
		//			writer.WriteLine(RemoveWords(currentLine, lettersCount));
		//		}
		//	});

		//	File.Delete(textFile.TempFilePath);
		//	textFile.TempFilePath = newTempPath;
		//}

		//async public Task RemoveWordsParallelAsync(TextFile textFile, int lettersCount)
		//{
		//	if (Equals(textFile, null))
		//	{
		//		throw new ArgumentNullException(nameof(textFile), "textFile is null.");
		//	}

		//	CheckLettersCount(lettersCount);
		//	var newTempPath = GetTempPath(textFile.TempFilePath);
		//	File.Copy(textFile.TempFilePath, newTempPath, true);

		//	await Task.Run(async () =>
		//	{
		//		var position = 0;
		//		var bytesReadCount = 0;
		//		var bytesRead = new byte[2048];
		//		var editedText = "";

		//		while ((bytesReadCount = await _dataAccessLayer.ReadAsync(textFile.TempFilePath, bytesRead, position, 2048)) > 0)
		//		{
		//			position += bytesReadCount;
		//			editedText = RemoveWords(ASCIIEncoding.ASCII.GetString(bytesRead), lettersCount);
		//			await _dataAccessLayer.WriteAsync(newTempPath, editedText);
		//		}
		//	});

		//	File.Delete(textFile.TempFilePath);
		//	textFile.TempFilePath = newTempPath;
		//}

		async public Task RemoveWordsParallelAsync(TextFile textFile, int lettersCount)
		{
			if (Equals(textFile, null))
			{
				throw new ArgumentNullException(nameof(textFile), "textFile is null.");
			}

			CheckLettersCount(lettersCount);
			var newTempPath = GetTempPath(textFile.TempFilePath);
			var fs = File.Create(newTempPath);
			fs.Dispose();

			await Task.Run(() =>
			{
				var lines = _dataAccessLayer.ReadLines(textFile.TempFilePath);
				var result = new List<string>();

				foreach (var line in lines)
				{
					_dataAccessLayer.Write(newTempPath, RemoveWords(line, lettersCount));
				}
			});

			File.Delete(textFile.TempFilePath);
			textFile.TempFilePath = newTempPath;
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