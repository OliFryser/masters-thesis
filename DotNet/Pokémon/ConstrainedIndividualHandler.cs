using System;
using System.Linq;
using MapElites;
using MapElites.Models;
using Pokémon.Args;
using WFC;
using WFC.Args;
using WFC.Models;

namespace Pokémon
{
    public class ConstrainedIndividualHandler
        : IndividualHandler,
            IConstrainedIndividualHandler<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>
    {
        private float FeasibilityThreshold { get; }
        private float SmoothingFactor { get; }

        public ConstrainedIndividualHandler(ConstrainedIndividualHandlerArgs args) : base(args.IndividualHandlerArgs)
        {
            FeasibilityThreshold = args.FeasibilityThreshold;
            SmoothingFactor = args.SmoothingFactor;
        }


        public new ConstrainedEntry<Individual, Behavior> Evaluate(Individual individual)
        {
            int amountComplete = 0;
            Behavior[] behaviors = new Behavior[EvaluationIterations];

            for (int i = 0; i < EvaluationIterations; i++)
            {
                WfcArgs args = new WfcArgs(Coordinates, TileTypes, AdjacencyRules, individual.Weights, i);
                State state = WaveFunctionCollapse.Run(args);

                if (state.IsCollapsed)
                {
                    amountComplete++;
                }

                behaviors[i] = GetBehavior(state);
            }

            Behavior averageBehavior = GetAverageBehavior(behaviors);
            float fitness = GetFitness(behaviors, averageBehavior, SmoothingFactor);
            float feasibility = amountComplete / (float)EvaluationIterations;

            var entry = new ConstrainedEntry<Individual, Behavior>(
                individual,
                averageBehavior,
                fitness,
                feasibility,
                FeasibilityThreshold);

            return entry;
        }

        private static float GetFitness(Behavior[] behaviors, Behavior averageBehavior, float smoothingFactor)
        {
            float deviationSum = behaviors.Sum(behavior => behavior.GetDeviation(averageBehavior));
            float meanDeviation = deviationSum / behaviors.Length;

            return MathF.Exp(-smoothingFactor * meanDeviation);
        }
    }
}