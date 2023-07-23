using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace WordProcessingWpfTask.Model
{
	internal class Redactor : IRedactor
	{
		public Redactor() { }

		//private readonly char[] _punctuationMarks = new char[] { '.', ',', '!', '?', ':', '(', ')', '\'', '"', ';' };

		public Task<string> RemoveAllSeparatorsParallelAsync(string text)
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
						for (int j = 0; j < lettersCount && (i + j + 1) < stringBuilder.Length; j++)
						{
							if (Char.IsLetter(stringBuilder[i + j + 1]))
							{
								wordLength++;
							}
							else
							{
								break;
							}
						}

						if (wordLength < lettersCount)
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
	}
}