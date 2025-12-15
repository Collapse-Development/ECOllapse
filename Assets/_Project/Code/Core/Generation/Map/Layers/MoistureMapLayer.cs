using AccidentalNoise;
using UnityEngine;

namespace _Project.Code.Core.Generation.Map
{
    public class MoistureMapLayer : NoiseMapLayer
    {
        private readonly int _octaves;
        private readonly double _frequency;
        private readonly float _dryer;
        private readonly float _dry;
        private readonly float _wet;
        private readonly float _wetter;
        private readonly float _wettest;

        public MoistureMapLayer(
            int width,
            int height,
            int octaves,
            double frequency,
            float dryer,
            float dry,
            float wet,
            float wetter,
            float wettest
        )
            : base("Moisture", width, height)
        {
            _octaves = octaves;
            _frequency = frequency;
            _dryer = dryer;
            _dry = dry;
            _wet = wet;
            _wetter = wetter;
            _wettest = wettest;
        }

        protected override ImplicitModuleBase CreateModule(int seed)
        {
            return new ImplicitFractal(
                FractalType.MULTI,
                BasisType.SIMPLEX,
                InterpolationType.QUINTIC,
                _octaves,
                _frequency,
                seed
            );
        }

        public void Apply(Tile tile)
        {
            var moistureValue = GetNormalizedValue(tile.X, tile.Y);
            tile.MoistureValue = moistureValue;
            tile.MoistureType = GetMoistureType(moistureValue);
        }

        public void ApplyHeightInfluence(Tile tile)
        {
            switch (tile.HeightType)
            {
                case HeightType.DeepWater:
                    OffsetValue(tile.X, tile.Y, 8f * tile.HeightValue);
                    break;
                case HeightType.ShallowWater:
                    OffsetValue(tile.X, tile.Y, 3f * tile.HeightValue);
                    break;
                case HeightType.Shore:
                    OffsetValue(tile.X, tile.Y, 1f * tile.HeightValue);
                    break;
                case HeightType.Sand:
                    OffsetValue(tile.X, tile.Y, 0.2f * tile.HeightValue);
                    break;
            }
        }

        public void AddMoisture(Tile tile, float amount)
        {
            OffsetValue(tile.X, tile.Y, amount);
            tile.MoistureValue = Mathf.Min(1f, tile.MoistureValue + amount);
            tile.MoistureType = GetMoistureType(tile.MoistureValue);
        }

        private MoistureType GetMoistureType(float moistureValue)
        {
            if (moistureValue < _dryer)
                return MoistureType.Dryest;
            if (moistureValue < _dry)
                return MoistureType.Dryer;
            if (moistureValue < _wet)
                return MoistureType.Dry;
            if (moistureValue < _wetter)
                return MoistureType.Wet;
            if (moistureValue < _wettest)
                return MoistureType.Wetter;

            return MoistureType.Wettest;
        }
    }
}
