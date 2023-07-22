using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordProcessingWpfTask.Abstract
{
	internal interface ICommand
	{
		Action action { get; set; }

		bool CanExecute();
	}
}
