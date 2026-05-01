using System;
using System.Collections.Generic;
using System.Linq;
using CMAESnet;
using Domain.Models;
using MapElites.Models;
using MathNet.Numerics.LinearAlgebra;
using Pokémon.Emitters.Scorers;

namespace Pokémon.Emitters 
{
    public struct EmitterBufferEntry
    {
        public double Score { get; }
        public Vector<double> Weights { get; }
        
        public EmitterBufferEntry(Individual individual, double score)
        {
            Weights = Vector<double>.Build.DenseOfEnumerable(individual.Weights.Select(w => w.Weight));
            Score = score;
        }
    }
    
    public class Emitter
    {
        private readonly List<TileType> _tileTypes; 
        private readonly List<EmitterBufferEntry> _buffer = new List<EmitterBufferEntry>();
        private CMA _cma;
        private IScorer _scorer;

        public Emitter(ConstrainedEntry<Individual, Behavior> meanEntry, double startingStepSize, IScorer scorer)
        {
            var bounds = Matrix<double>.Build.Dense(meanEntry.Individual.Weights.Count, 2);
            for (int i = 0; i < meanEntry.Individual.Weights.Count; i++)
            {
                bounds[i, 0] = 0.0; // Lower
                bounds[i, 1] = 1.0; // Upper
            }

            _tileTypes = meanEntry.Individual.Weights.Select(tw => tw.TileType).ToList();
            
            List<double> meanWeights = meanEntry.Individual.Weights.Select(w => w.Weight).ToList();
            _cma = new CMA(meanWeights, startingStepSize, bounds: bounds);
            
            _scorer = scorer;
            _scorer.Initialize(meanEntry);
        }

        public Individual Ask()
        {
            var newWeights = _cma.Ask();

            var newTileWeights = 
                newWeights.Zip(_tileTypes, (w, t) => new TileWeight(t, w)).ToList();

            return new Individual(newTileWeights);
        }

        public void Tell(ConstrainedEntry<Individual, Behavior> entry)
        {
            var bufferEntry = new EmitterBufferEntry(entry.Individual, _scorer.GetScore(entry));
            _buffer.Add(bufferEntry);
            if (_buffer.Count >= _cma.PopulationSize)
            {
                var solutions = 
                    _buffer.Select(e => new Tuple<Vector<double>, double>(e.Weights, e.Score)).ToList();
                _cma.Tell(solutions);
            }
        }
    }
}