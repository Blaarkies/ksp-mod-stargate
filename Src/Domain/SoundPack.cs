using Stargate.Utilities;

namespace Stargate.Domain
{
    /// <summary>
    /// Describes a set of sound files. Used by <see cref="SoundEffect"/> to load multiple files, and play a random
    /// file to reduce repetition. <br/>
    /// All sound files must match <b>baseName</b> appended with a sequence number from <b>1</b> to <b>count</b>.
    /// </summary>
    public class SoundPack
    {
        public string BaseName { get; }
        public int Count { get; }
        public bool IsLoop { get; }

        public SoundPack(string baseName, int count, bool isLoop = false)
        {
            BaseName = baseName;
            Count = count;
            IsLoop = isLoop;
        }
    }
}
