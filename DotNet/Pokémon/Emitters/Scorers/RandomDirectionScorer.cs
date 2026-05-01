using System;
using MapElites.Models;
using MathNet.Numerics.LinearAlgebra;

namespace Pokémon.Emitters.Scorers
{
    public class RandomDirectionScorer : IScorer
    {
        private Vector<double> _randomDirection;
        private Vector<double> _meanBehavior;
        
        public void Initialize(ConstrainedEntry<Individual, Behavior> meanEntry)
        {
            Random random = new Random();
         
            double theta = random.NextDouble() * 2 * Math.PI;

            double x = Math.Cos(theta);
            double y = Math.Sin(theta);

            _randomDirection = Vector<double>.Build.Dense(new[] { x, y });
            _meanBehavior = GetVectorFromBehavior(meanEntry.Behavior);
        }

        public double GetScore(ConstrainedEntry<Individual, Behavior> entry)
        {
            Vector<double> newBehavior = GetVectorFromBehavior(entry.Behavior);
            Vector<double> behaviorDirection = newBehavior - _meanBehavior;
            return _randomDirection.DotProduct(behaviorDirection);
        }

        private Vector<double> GetVectorFromBehavior(Behavior behavior)
        {
            return Vector<double>.Build.Dense(new double[]
            {
                behavior.FlowerPercentage, 
                behavior.Variation
            });
        }
    }
}