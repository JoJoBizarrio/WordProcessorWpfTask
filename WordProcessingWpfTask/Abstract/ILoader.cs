using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Abstract
{
    interface ILoader
    {
        Task<string> OpenAsync(string path);
    }
}
