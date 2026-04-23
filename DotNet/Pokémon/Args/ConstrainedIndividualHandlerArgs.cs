namespace Pokémon.Args
{
    public readonly struct ConstrainedIndividualHandlerArgs
    {
        public ConstrainedIndividualHandlerArgs(IndividualHandlerArgs individualHandlerArgs, float feasibilityThreshold)
        {
            IndividualHandlerArgs = individualHandlerArgs;
            FeasibilityThreshold = feasibilityThreshold;
        }

        public IndividualHandlerArgs IndividualHandlerArgs { get; }
        public float FeasibilityThreshold { get; }
    }
}