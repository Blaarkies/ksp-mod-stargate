namespace Stargate.Utilities
{
    /// <summary>
    /// Keeps track of an index point inside a set range. The index point can be shifted forwards/backwards and will
    /// wrap around the range instead of going out of bounds.
    /// </summary>
    public class Cycler
    {
        public int Index { get; private set; }

        private readonly int _maxIndex;
        private readonly int _indexOffset;

        public Cycler(int maxIndex, int indexOffset = 0, int? startingIndex = null)
        {
            _maxIndex = maxIndex;
            _indexOffset = indexOffset;
            Index = startingIndex ?? indexOffset;
        }

        /// <summary>
        /// Once off method for use when a <b>Cycler</b> instance can't be re-used. This finds the adjacent index given
        /// these parameters
        /// </summary>
        /// <param name="maxIndex">The size of the range to wrap the index around</param>
        /// <param name="shift">The distance used to find the new index position</param>
        /// <param name="startingIndex">The initial index position</param>
        /// <returns>Position of index at <b>shift</b> units away from <b>startingIndex</b></returns>
        public static int GetShiftFrom(int maxIndex, int shift = 1, int startingIndex = 0)
        {
            return (startingIndex + shift) % maxIndex;
        }

        private int Shift(int shift = 1)
        {
            Index = (-_indexOffset + Index + shift) % _maxIndex + _indexOffset;
            return Index;
        }

        public int Next()
        {
            return Shift(1);
        }

        public int Previous()
        {
            return Shift(-1);
        }

    }
}
