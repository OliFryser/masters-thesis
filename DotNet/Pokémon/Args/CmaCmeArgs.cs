using System;
using System.Text;
using MapElites.Args;

namespace Pokémon.Args
{
    public readonly struct EmitterConfiguration
    {
        public EmitterConfiguration(int optimizationEmitters, int feasibilityEmitters, int randomDirectionEmitters)
        {
            OptimizationEmitters = optimizationEmitters;
            FeasibilityEmitters = feasibilityEmitters;
            RandomDirectionEmitters = randomDirectionEmitters;
        }

        public int OptimizationEmitters { get; }
        public int FeasibilityEmitters { get; }
        public int RandomDirectionEmitters { get; }

        public override string ToString() => 
            $"Optimization emitters: {OptimizationEmitters}{Environment.NewLine}"
                + $"Feasibility emitters: {FeasibilityEmitters}{Environment.NewLine}"
                + $"Random Direction emitters: {RandomDirectionEmitters}{Environment.NewLine}";
    }
    
    public struct CmaCmeArgs
    {
        public MapElitesArgs MapElitesArgs { get; }
        public ConstrainedIndividualHandler ConstrainedIndividualHandler { get; }
        public EmitterConfiguration EmitterConfiguration { get; }
        public double StartingStepSize { get; }
        
        public CmaCmeArgs(
            MapElitesArgs mapElitesArgs, 
            ConstrainedIndividualHandler constrainedIndividualHandler, 
            EmitterConfiguration emitterConfiguration, double startingStepSize)
        {
            MapElitesArgs = mapElitesArgs;
            ConstrainedIndividualHandler = constrainedIndividualHandler;
            EmitterConfiguration = emitterConfiguration;
            StartingStepSize = startingStepSize;
        }
    }
}