namespace Pokémon.Args
{
    public readonly struct ConstrainedIndividualHandlerArgs
    {
        public ConstrainedIndividualHandlerArgs(
            IndividualHandlerArgs individualHandlerArgs, 
            float feasibilityThreshold,
            float smoothingFactor)
        {
            IndividualHandlerArgs = individualHandlerArgs;
            FeasibilityThreshold = feasibilityThreshold;
            SmoothingFactor = smoothingFactor;
        }

        public IndividualHandlerArgs IndividualHandlerArgs { get; }
        public float FeasibilityThreshold { get; }
        public float SmoothingFactor { get; }
    }
}