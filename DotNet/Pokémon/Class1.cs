using System;
using MapElites;
using MapElites.Models;
using TilemapAnalysis;

namespace Pokémon
{
    public class Individual
    {
        
    }

    public class Behavior
    {
        
    }

    public class Key : IEquatable<Key>
    {
        public bool Equals(Key other)
        {
            throw new NotImplementedException();
        }
    }

    public class Entry : Entry<Individual, Behavior>
    {
        public Entry(Individual individual, Behavior behavior, float fitness) : base(individual, behavior, fitness)
        {
        }
    }

    public class PopulationManager : IPopulationManager<Key, Entry, Individual, Behavior>
    {
        public PopulationManager(string inputTilemapPath)
        {
            using TilemapAnalyzer tilemapAnalyzer = new TilemapAnalyzer(inputTilemapPath);
            int tileTypeCount = tilemapAnalyzer.TileTypeCount;
            
        }

        public Individual CreateRandom()
        {
            throw new NotImplementedException();
        }

        public Individual Mutate(Individual individual)
        {
            throw new NotImplementedException();
        }

        public Entry Evaluate(Individual individual)
        {
            throw new NotImplementedException();
        }

        public Key GetKey(Behavior behavior)
        {
            throw new NotImplementedException();
        }
    }
}