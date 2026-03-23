using AccidentalNoise;

namespace _Project.Code.Core.Generation.Map
{
    public class HeatMapLayer : NoiseMapLayer
    {
        private readonly int _octaves;
        private readonly double _frequency;
        private readonly float _coldest;
        private readonly float _colder;
        private readonly float _cold;
        private readonly float _warm;
        private readonly float _warmer;

        public HeatMapLayer(
            int width,
            int height,
            int octaves,
            double frequency,
            float coldest,
            float colder,
            float cold,
            float warm,
            float warmer
        )
            : base("Heat", width, height)
        {
            _octaves = octaves;
            _frequency = frequency;
            _coldest = coldest;
            _colder = colder;
            _cold = cold;
            _warm = warm;
            _warmer = warmer;
        }

        protected override ImplicitModuleBase CreateModule(int seed)
        {
            var gradient = new ImplicitGradient(1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            var fractal = new ImplicitFractal(
                FractalType.MULTI,
                BasisType.SIMPLEX,
                InterpolationType.QUINTIC,
                _octaves,
                _frequency,
                seed
            );

            var combiner = new ImplicitCombiner(CombinerType.MULTIPLY);
            combiner.AddSource(gradient);
            combiner.AddSource(fractal);
            return combiner;
        }

        public void ApplyHeightInfluence(Tile tile)
        {
            switch (tile.HeightType)
            {
                case HeightType.Forest:
                    OffsetValue(tile.X, tile.Y, -0.1f * tile.HeightValue);
                    break;
                case HeightType.Rock:
                    OffsetValue(tile.X, tile.Y, -0.25f * tile.HeightValue);
                    break;
                case HeightType.Snow:
                    OffsetValue(tile.X, tile.Y, -0.4f * tile.HeightValue);
                    break;
                default:
                    OffsetValue(tile.X, tile.Y, 0.01f * tile.HeightValue);
                    break;
            }
        }

        public void Apply(Tile tile)
        {
            var heatValue = GetNormalizedValue(tile.X, tile.Y);
            tile.HeatValue = heatValue;
            tile.HeatType = GetHeatType(heatValue);
        }

        private HeatType GetHeatType(float heatValue)
        {
            if (heatValue < _coldest)
                return HeatType.Coldest;
            if (heatValue < _colder)
                return HeatType.Colder;
            if (heatValue < _cold)
                return HeatType.Cold;
            if (heatValue < _warm)
                return HeatType.Warm;
            if (heatValue < _warmer)
                return HeatType.Warmer;

            return HeatType.Warmest;
        }
    }
}
