using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Model;
using WordProcessingWpfTask.ViewModel;

namespace WordProcessorWpfTask.Tests
{
    [TestFixture]
    [TestOf(typeof(TextFile))]
    public class TextFileTests
    {
        [Test]
        public void GetHashCode_ExpectedMore50percent()
        {
            var lengthSample = (long)Math.Pow(10, 8);
            var textFiles = new TextFile[lengthSample];
            var hashList = new int[lengthSample];

            for (int i = 0; i < lengthSample; i++)
            {
                textFiles[i] = new TextFile();
                hashList[i] = textFiles[i].GetHashCode();
            }

            Array.Sort(hashList);
            //hashList.Sort();
            int differentValues = 0;
            int curValue = hashList[0];

            for (int i = 1; i < lengthSample; i++)
            {
                if (hashList[i] != curValue)
                {
                    differentValues++;
                    curValue = hashList[i];
                }
            }

            Assert.Greater(differentValues, lengthSample / 2); // on 10^4 => 1times, on 10^5 => 3times, on 10^6 => 126times
        }
    }
}
