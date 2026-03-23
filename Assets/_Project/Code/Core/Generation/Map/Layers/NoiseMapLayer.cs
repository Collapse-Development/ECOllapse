using _Project.Code.Core.Map;
using AccidentalNoise;
using UnityEngine;

namespace _Project.Code.Core.Generation.Map
{
    public abstract class NoiseMapLayer : IMapLayer
    {
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public MapData Data { get; private set; }

        protected NoiseMapLayer(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
        }

        public void Generate(int seed)
        {
            Data = new MapData(Width, Height);
            var module = CreateModule(seed);

            for (var x = 0; x < Width; x++)
            {
                for (var y = 0; y < Height; y++)
                {
                    var value = Sample(module, x, y);
                    Data.Data[x, y] = value;

                    if (value > Data.Max)
                        Data.Max = value;
                    if (value < Data.Min)
                        Data.Min = value;
                }
            }
        }

        protected virtual float Sample(ImplicitModuleBase module, int x, int y)
        {
            // WRAP ON BOTH AXIS
            const float x1 = 0;
            const float x2 = 2;
            const float y1 = 0;
            const float y2 = 2;
            const float dx = x2 - x1;
            const float dy = y2 - y1;

            // Sample noise at smaller intervals
            var s = x / (float)Width;
            var t = y / (float)Height;

            // Calculate our 4D coordinates
            var nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
            var ny = y1 + Mathf.Cos(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);
            var nz = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
            var nw = y1 + Mathf.Sin(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);

            return (float)module.Get(nx, ny, nz, nw);
        }

        public float GetNormalizedValue(int x, int y)
        {
            if (Data == null)
                return 0f;

            var range = Mathf.Max(0.000001f, Data.Max - Data.Min);
            return (Data.Data[x, y] - Data.Min) / range;
        }

        protected abstract ImplicitModuleBase CreateModule(int seed);

        protected void OffsetValue(int x, int y, float amount)
        {
            if (Data == null)
                return;

            Data.Data[x, y] += amount;
        }
    }
}
