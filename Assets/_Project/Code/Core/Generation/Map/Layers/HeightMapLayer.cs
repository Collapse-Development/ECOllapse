using AccidentalNoise;

namespace _Project.Code.Core.Generation.Map
{
    public class HeightMapLayer : NoiseMapLayer
    {
        private readonly int _octaves;
        private readonly double _frequency;
        private readonly float _deepWater;
        private readonly float _shallowWater;
        private readonly float _sand;
        private readonly float _grass;
        private readonly float _forest;
        private readonly float _rock;

        public HeightMapLayer(
            int width,
            int height,
            int octaves,
            double frequency,
            float deepWater,
            float shallowWater,
            float sand,
            float grass,
            float forest,
            float rock
        )
            : base("Height", width, height)
        {
            _octaves = octaves;
            _frequency = frequency;
            _deepWater = deepWater;
            _shallowWater = shallowWater;
            _sand = sand;
            _grass = grass;
            _forest = forest;
            _rock = rock;
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
            var value = GetNormalizedValue(tile.X, tile.Y);
            tile.HeightValue = value;
            tile.HeightType = GetHeightType(value);
            tile.Collidable = IsCollidable(tile.HeightType);
        }

        public HeightType GetHeightType(float heightValue)
        {
            if (heightValue < _deepWater)
                return HeightType.DeepWater;
            if (heightValue < _shallowWater)
                return HeightType.ShallowWater;
            if (heightValue < _sand)
                return HeightType.Sand;
            if (heightValue < _grass)
                return HeightType.Grass;
            if (heightValue < _forest)
                return HeightType.Forest;
            if (heightValue < _rock)
                return HeightType.Rock;

            return HeightType.Snow;
        }

        private static bool IsCollidable(HeightType heightType)
        {
            return heightType != HeightType.DeepWater && heightType != HeightType.ShallowWater;
        }
    }
}
