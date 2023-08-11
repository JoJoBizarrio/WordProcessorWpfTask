using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
    class DataAccessLayer : IDataAccessLayer
    {
        async public Task<string> OpenAsync(string path)
        {
            using (var reader = new StreamReader(new BufferedStream(new FileStream(path, FileMode.Open, FileAccess.Read), 4096)))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public Task SaveAsync(string path, string value)
        {
            return null;
        }

        async public Task<string> ReadAsync(string path, int startPos, int count)
        {
            using (var reader = new StreamReader(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read)))
            {
                var result = new char[startPos-count];
                reader.ReadAsync(result, startPos, 50);
                return result.ToString();
            }
        }
    }
}