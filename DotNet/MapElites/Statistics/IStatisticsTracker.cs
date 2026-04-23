using MapElites.Models;

namespace MapElites.Statistics
{
    public interface IStatisticsTracker
    {
        void AddPoint(IArchiveStatisticsProvider archive);

        void SaveToFile(string outputPath);
    }
}