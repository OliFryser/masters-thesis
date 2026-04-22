namespace Pokémon
{
    public class Behavior
    {
        public Behavior(float flowerPercentage, float doorPercentage, float tileTypesUsedPercentage)
        {
            FlowerPercentage = flowerPercentage;
            DoorPercentage = doorPercentage;
            TileTypesUsedPercentage = tileTypesUsedPercentage;
        }

        public static int BehaviorCount => 3;
        public float FlowerPercentage { get; }
        public float DoorPercentage { get; }
        public float TileTypesUsedPercentage { get; }
    }
}