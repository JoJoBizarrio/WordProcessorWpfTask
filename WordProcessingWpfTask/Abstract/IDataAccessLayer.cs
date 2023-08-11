using WordProcessingWpfTask.Abstract;

namespace WordProcessingWpfTask.Model
{
    internal interface IDataAccessLayer : ISaver, ILoader
    {
    }
}