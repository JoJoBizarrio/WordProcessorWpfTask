using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
    public class Redactor : IRedactor
    {
        private readonly IFileSystem _fileSystem;

        public Redactor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        #region remove marks
        async public Task RemoveAllMarksParallelAsync(IEnumerable<TextFile> textFiles)
        {
            CheckTextFile(textFiles);

            if (!textFiles.Any())
            {
                return;
            }

            var tasks = textFiles.Select(item => RemoveAllMarksParallelAsync(item));

            await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
        }

        async public Task RemoveAllMarksParallelAsync(TextFile textFile)
        {
            CheckTextFile(textFile);

            await Task.Run(() =>
            {
                lock (textFile)
                {
                    using var fileStream = _fileSystem.FileStream.New(textFile.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

                    var readBytes = new byte[4096];
                    var bytesReadCount = 0;
                    var writerPosition = 0L;
                    var readerPosition = 0L;

                    while ((bytesReadCount = fileStream.Read(readBytes, 0, 4096)) > 0)
                    {
                        readerPosition = fileStream.Position;

                        var readBlock = Encoding.ASCII.GetChars(readBytes, 0, bytesReadCount);
                        var filter = readBlock.Where(symbol => !Char.IsPunctuation(symbol));
                        var editBlock = Encoding.ASCII.GetBytes(new String(filter.ToArray()));

                        fileStream.Position = writerPosition;

                        fileStream.Write(editBlock, 0, editBlock.Length);
                        writerPosition += editBlock.Length;

                        fileStream.Position = readerPosition;
                    }

                    fileStream.Position = writerPosition;

                    fileStream.Write(new byte[readerPosition - writerPosition]);
                }
            });
        }
        #endregion

        #region remove words
        async public Task RemoveWordsParallelAsync(IEnumerable<TextFile> textFiles, int lettersCount)
        {
            CheckTextFile(textFiles);
            CheckLettersCount(lettersCount);

            if (!textFiles.Any() || lettersCount == 0)
            {
                return;
            }

            var tasks = textFiles.Select(item => RemoveWordsParallelAsync(item, lettersCount));

            await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
        }

        async public Task RemoveWordsParallelAsync(TextFile textFile, int lettersCount)
        {
            CheckTextFile(textFile);
            CheckLettersCount(lettersCount);

            if (lettersCount == 0)
            {
                return;
            }

            await Task.Run(() =>
            {
                lock (textFile)
                {
                    var fileStream = textFile.FileStream;

                    var readBytes = new byte[4096];
                    var bytesReadCount = 0;
                    var writerPosition = 0L;
                    var readerPosition = 0L;

                    while ((bytesReadCount = fileStream.Read(readBytes, 0, 4096)) > 0)
                    {
                        readerPosition = fileStream.Position;

                        var readBlock = Encoding.ASCII.GetChars(readBytes, 0, bytesReadCount);
                        var result = RemoveWordsAlgorithmInternal(readBlock.ToList(), lettersCount);
                        var editBlock = Encoding.ASCII.GetBytes(new String(result));

                        fileStream.Position = writerPosition;

                        fileStream.Write(editBlock, 0, editBlock.Length);
                        writerPosition += editBlock.Length;

                        fileStream.Position = readerPosition;
                    }

                    fileStream.Position = writerPosition;

                    fileStream.Write(new byte[readerPosition - writerPosition]);
                }
            });
        }

        private char[] RemoveWordsAlgorithmInternal(List<char> chars, int lettersCount)
        {
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

            return chars.ToArray();
        }
        #endregion

        #region operation with files
        public void SaveFile(TextFile textFile, string path)
        {
            CheckPath(path);

            _fileSystem.File.Copy(textFile.TempFilePath, path, true);
        }

        public TextFile OpenFile(string path)
        {
            CheckPath(path);

            if (!_fileSystem.File.Exists(path))
            {
                throw new FileNotFoundException("File isnt exists by path.", Path.GetFileName(path));
            }

            var tempFilePath = GetTempPath(path);

            _fileSystem.File.Copy(path, tempFilePath, true);

            var newTextFile = new TextFile()
            {
                Title = Path.GetFileNameWithoutExtension(path),
                FilePath = path,
                TempFilePath = tempFilePath,
                FileStream = _fileSystem.FileStream.New(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)
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

        private void CheckTextFile(TextFile textFile)
        {
            if (Equals(textFile, null))
            {
                throw new ArgumentNullException(nameof(textFile), "textFile is null.");
            }
        }

        private void CheckTextFile(IEnumerable<TextFile> textFiles)
        {
            if (Equals(textFiles, null))
            {
                throw new ArgumentNullException(nameof(textFiles), "textFiles is null.");
            }
        }
        #endregion

        #region other
        private string GetTempPath(string path)
        {
            var extension = Path.GetExtension(path);
            return Path.GetTempFileName().Replace(".tmp", extension);
        }

        private string GetOutFilePath(string path)
        {
            var extencion = Path.GetExtension(path);
            return path.Replace(extencion, ".out" + extencion);
        }
        #endregion
    }
}