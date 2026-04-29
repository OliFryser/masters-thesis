using System;

namespace Pokémon
{
    // Inspired by https://stackoverflow.com/questions/218060/random-gaussian-variables
    public class NormalSampler
    {
        private Random Random { get; } = new Random();

        public double Sample(double mean, double sigma)
        {
            double u1 = 1.0 - Random.NextDouble();
            double u2 = 1.0 - Random.NextDouble();
            
            double standardNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

            return mean + sigma * standardNormal;
        }
    }
}