using UnityEngine;

namespace Code.Features.Chunks
{
    public static class WorldMath
    {
        public static int FloorToInt(float v)
        {
            int i = (int)Mathf.Floor(v);
            return i;
        }

        // модуль для индексов (wrap по X)
        public static int Wrap(int a, int m)
        {
            int r = a % m;
            if (r < 0) r += m;
            return r;
        }

        public static ChunkIndex WorldToChunkIndex(WorldPos wp, WorldConfig cfg)
        {
            int cx = FloorToInt(wp.x / cfg.chunkWidth);
            int cy = FloorToInt(wp.y / cfg.chunkHeight);
            cx = Wrap(cx, cfg.chunksX);            // кольцевание по X
            cy = Mathf.Clamp(cy, 0, cfg.chunksY - 1); // clamp по Y
            return new ChunkIndex(cx, cy);
        }

        public static LocalPos WorldToLocal(WorldPos wp, WorldConfig cfg)
        {
            // compute chunk indices without wrap used for local
            int cxRaw = FloorToInt(wp.x / cfg.chunkWidth);
            float lx = wp.x - cxRaw * cfg.chunkWidth;
            // normalize to [0..W)
            lx = lx % cfg.chunkWidth;
            if (lx < 0) lx += cfg.chunkWidth;

            int cyRaw = FloorToInt(wp.y / cfg.chunkHeight);
            float ly = wp.y - cyRaw * cfg.chunkHeight;
            // ly normally in [0,H) if world.y >=0 else can clamp/handle
            // clamp to [0,H)
            ly = Mathf.Clamp(ly, 0f, cfg.chunkHeight);

            return new LocalPos(lx, ly);
        }

        public static Vector2 ChunkOriginWorld(ChunkIndex ci, WorldConfig cfg)
        {
            // origin (bottom-left) world coordinate of chunk ci (with wrapped cx)
            return new Vector2(ci.x * cfg.chunkWidth, ci.y * cfg.chunkHeight);
        }

        public static Rect ChunkAABB(ChunkIndex ci, WorldConfig cfg)
        {
            Vector2 origin = ChunkOriginWorld(ci, cfg);
            return new Rect(origin.x, origin.y, cfg.chunkWidth, cfg.chunkHeight);
        }

        public static int ToLinear(ChunkIndex ci, WorldConfig cfg)
        {
            return ci.y * cfg.chunksX + ci.x;
        }
    }
}