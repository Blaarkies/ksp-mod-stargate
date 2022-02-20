namespace Stargate.Utilities
{
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
