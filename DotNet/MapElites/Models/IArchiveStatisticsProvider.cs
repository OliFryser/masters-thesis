namespace MapElites.Models
{
    public interface IArchiveStatisticsProvider
    {
        int Count { get; }
        int BucketCapacity { get; }
        float GetMaxFitness();
        float GetAverageFitness();
        int GetFeasiblePopulationSize();
        float GetReliability();
    }
}