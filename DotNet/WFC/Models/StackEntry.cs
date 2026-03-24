namespace WFC.Models
{
    public struct StackEntry
    {
        public StackEntry(int fromCellIndex, int bannedTileIndex)
        {
            FromCellIndex = fromCellIndex;
            BannedTileIndex = bannedTileIndex;
        }

        public int FromCellIndex { get; }
        public int BannedTileIndex { get; }
    }
}