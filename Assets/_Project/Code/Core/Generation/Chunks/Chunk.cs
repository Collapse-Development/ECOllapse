using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Chunks
{
    public class Chunk
    {
        public ChunkIndex Index { get; private set; }
        public ChunkState State { get; set; }
        public GameObject View { get; private set; }

        public List<Tile> Tiles { get; private set; } = new List<Tile>();

        public Chunk(ChunkIndex index)
        {
            Index = index;
            State = ChunkState.Unloaded;
        }
        public void AddTile(Tile tile)
        {
            Tiles.Add(tile);
        }

        public void SetView(GameObject view)
        {
            View = view;
            if (View != null)
                View.SetActive(State == ChunkState.Visible);
        }

        public void Show()
        {
            State = ChunkState.Visible;
            if (View != null)
                View.SetActive(true);
        }

        public void Hide()
        {
            State = ChunkState.Ready;
            if (View != null)
                View.SetActive(false);
        }
    }
}
