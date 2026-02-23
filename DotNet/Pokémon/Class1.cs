using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Domain;

namespace Pokémon
{
    public class Class1
    {
        public void Run()
        {
            TileType water = new TileType
            {
                Type = "Water"
            };

            Vector[] positions = new Vector[1];

            HashSet<TileOption> options = new HashSet<TileOption>
            {
                new TileOption
                {
                    TileType = water,
                    Weight = 1
                }
            };

            ReadOnlyDictionary<TileType, TileRules> rules = new ReadOnlyDictionary<TileType, TileRules>(
                new Dictionary<TileType, TileRules>()
                {
                    {
                        water, new TileRules()
                    }
                });

            Level level = WFC.WaveFunctionCollapse.Generate(positions, options, rules);
        }
    }
}