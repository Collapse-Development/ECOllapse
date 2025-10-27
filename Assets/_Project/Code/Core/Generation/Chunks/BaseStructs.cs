namespace Code.Core.Chunks
{
    // Structs & Config
    public struct ChunkIndex
    {
        public float X;
        public float Y;

        public ChunkIndex(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public struct LocalPos
    {
        public float X;
        public float Y;

        public LocalPos(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public struct WorldPos
    {
        public float X;
        public float Y;

        public WorldPos(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
