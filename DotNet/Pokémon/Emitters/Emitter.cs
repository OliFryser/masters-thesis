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
        private readonly IScorer _scorer;
        private readonly Matrix<double>? _bounds;
        private readonly Func<IScorer> _createScorer;
        private readonly double _startingStepSize;
        
        private ConstrainedEntry<Individual, Behavior> _meanEntry;
        private CMA _cma;
        
        public int GeneratedSolutions { get; private set; }
        public bool IsConverged => _cma.IsConverged(); 
        
        public Emitter(ConstrainedEntry<Individual, Behavior> meanEntry, double startingStepSize, IScorer scorer)
        {
            _startingStepSize = startingStepSize;
            _scorer = scorer;
            _bounds = Matrix<double>.Build.Dense(meanEntry.Individual.Weights.Count, 2);
            for (int i = 0; i < meanEntry.Individual.Weights.Count; i++)
            {
                _bounds[i, 0] = 0.0; // Lower
                _bounds[i, 1] = 1.0; // Upper
            }

            _tileTypes = meanEntry.Individual.Weights.Select(tw => tw.TileType).ToList();
            
            Reset(meanEntry);
        }

        public void Reset(ConstrainedEntry<Individual, Behavior> meanEntry)
        {
            _meanEntry = meanEntry;
            List<double> meanWeights = meanEntry.Individual.Weights.Select(w => w.Weight).ToList();
            _cma = new CMA(meanWeights, _startingStepSize, bounds: _bounds);
            _scorer.Reset();
        }

        public Individual Ask()
        {
            Vector<double>? newWeights = _cma.Ask();

            List<TileWeight> newTileWeights = 
                newWeights.Zip(_tileTypes, (w, t) => new TileWeight(t, w)).ToList();

            return new Individual(newTileWeights);
        }

        public void Tell(ConstrainedEntry<Individual, Behavior> entry)
        {
            GeneratedSolutions++;
            
            double score = _scorer.GetScore(entry, _meanEntry);

            EmitterBufferEntry bufferEntry = new EmitterBufferEntry(entry.Individual, score);
            _buffer.Add(bufferEntry);
            if (_buffer.Count >= _cma.PopulationSize)
            {
                // The CMA.net code uses OrderBy, 
                // which assumes SMALLER values are BETTER (Minimization).
                List<Tuple<Vector<double>, double>> solutions = 
                    _buffer.Select(e => new Tuple<Vector<double>, double>(e.Weights, -e.Score)).ToList();
                _cma.Tell(solutions);
                _buffer.Clear();
            }
        }
    }
}