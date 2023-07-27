using System;
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

        async public Task<string> RemoveAllMarksParallelAsync(Guid id)
        {
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

        async public Task<string> RemoveWordsParallelAsync(Guid id, int lettersCount)
        {
            return await Task.Run(() =>
            {
                var textFile = IdKeyTextFileValueDictionary[id];
                var stringBuilder = new StringBuilder(textFile.Text);
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

                var result = stringBuilder.ToString();
                textFile.Text = result;
                return result;
            });
        }

        async public Task RemoveWordsInsideSeveralTextFilesParallelAsync(IEnumerable<Guid> idArray, int letterCount)
        {
            var tasks = idArray.Select(id => RemoveWordsParallelAsync(id, letterCount));

            await Task.WhenAll(tasks.AsParallel().Select(async task => await task));
        }

        async public Task SaveFileAsync(Guid id, string path) // id?
        {
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
    }
}