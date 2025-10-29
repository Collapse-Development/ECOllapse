using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Code.Core.Chunks
{
    public class ChunkManager : MonoBehaviour
    {
        [SerializeField] private WorldConfig worldConfig;
        [SerializeField] private ChunkViewPool viewPool;
        [SerializeField] private int maxConcurrentJobs = 4;

        private Dictionary<ChunkIndex, Chunk> chunks = new Dictionary<ChunkIndex, Chunk>();
        private Queue<Chunk> generationQueue = new Queue<Chunk>();
        private int runningJobs = 0;

        private void Update()
        {
            ProcessGenerationQueue();
        }

        public void RequestChunk(ChunkIndex index)
        {
            if (!chunks.TryGetValue(index, out var chunk))
            {
                chunk = new Chunk(index);
                chunks.Add(index, chunk);
            }

            if (chunk.State == ChunkState.Unloaded)
            {
                chunk.State = ChunkState.Generating;
                generationQueue.Enqueue(chunk);
            }

            if (chunk.State == ChunkState.Ready)
            {
                AssignView(chunk);
                chunk.Show();
            }
        }

        private void ProcessGenerationQueue()
        {
            while (runningJobs < maxConcurrentJobs && generationQueue.Count > 0)
            {
                var chunk = generationQueue.Dequeue();
                GenerateChunkAsync(chunk);
            }
        }

        private async void GenerateChunkAsync(Chunk chunk)
        {
            runningJobs++;

            // —имул€ци€ фоновой генерации
            await Task.Run(() =>
            {
                // “ут можно генерить данные карты, шум, тайлы и т.д.
                System.Threading.Thread.Sleep(50);
            });

            chunk.State = ChunkState.Ready;
            AssignView(chunk);

            runningJobs--;
        }

        private void AssignView(Chunk chunk)
        {
            if (chunk.View == null)
            {
                var view = viewPool.Get();
                view.transform.position = (Vector3)WorldMath.ChunkOriginWorld(chunk.Index, worldConfig);
                chunk.SetView(view);
            }
        }

        public void UnloadChunk(Chunk chunk)
        {
            chunk.Hide();
            if (chunk.View != null)
            {
                viewPool.Return(chunk.View);
                chunk.SetView(null);
            }
            chunk.State = ChunkState.Unloaded;
        }
    }
}
