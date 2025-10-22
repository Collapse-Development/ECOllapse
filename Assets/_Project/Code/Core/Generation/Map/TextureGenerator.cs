using UnityEngine;

public static class TextureGenerator
{
    // Height Map Colors
    private static readonly Color DeepColor = new(15 / 255f, 30 / 255f, 80 / 255f, 1);
    private static readonly Color ShallowColor = new(15 / 255f, 40 / 255f, 90 / 255f, 1);
    private static readonly Color RiverColor = new(30 / 255f, 120 / 255f, 200 / 255f, 1);
    private static readonly Color SandColor = new(198 / 255f, 190 / 255f, 31 / 255f, 1);
    private static readonly Color GrassColor = new(50 / 255f, 220 / 255f, 20 / 255f, 1);
    private static readonly Color ForestColor = new(16 / 255f, 160 / 255f, 0, 1);
    private static readonly Color RockColor = new(0.5f, 0.5f, 0.5f, 1);
    private static readonly Color SnowColor = new(1, 1, 1, 1);

    private static readonly Color IceWater = new(210 / 255f, 255 / 255f, 252 / 255f, 1);
    private static readonly Color ColdWater = new(119 / 255f, 156 / 255f, 213 / 255f, 1);
    private static readonly Color RiverWater = new(65 / 255f, 110 / 255f, 179 / 255f, 1);

    // Height Map Colors
    private static readonly Color Coldest = new(0, 1, 1, 1);
    private static readonly Color Colder = new(170 / 255f, 1, 1, 1);
    private static readonly Color Cold = new(0, 229 / 255f, 133 / 255f, 1);
    private static readonly Color Warm = new(1, 1, 100 / 255f, 1);
    private static readonly Color Warmer = new(1, 100 / 255f, 0, 1);
    private static readonly Color Warmest = new(241 / 255f, 12 / 255f, 0, 1);

    //Moisture map
    private static readonly Color Dryest = new(255 / 255f, 139 / 255f, 17 / 255f, 1);
    private static readonly Color Dryer = new(245 / 255f, 245 / 255f, 23 / 255f, 1);
    private static readonly Color Dry = new(80 / 255f, 255 / 255f, 0 / 255f, 1);
    private static readonly Color Wet = new(85 / 255f, 255 / 255f, 255 / 255f, 1);
    private static readonly Color Wetter = new(20 / 255f, 70 / 255f, 255 / 255f, 1);
    private static readonly Color Wettest = new(0 / 255f, 0 / 255f, 100 / 255f, 1);

    //biome map
    private static readonly Color Ice = Color.white;
    private static readonly Color Desert = new(238 / 255f, 218 / 255f, 130 / 255f, 1);
    private static readonly Color Savanna = new(177 / 255f, 209 / 255f, 110 / 255f, 1);
    private static readonly Color TropicalRainforest = new(66 / 255f, 123 / 255f, 25 / 255f, 1);
    private static readonly Color Tundra = new(96 / 255f, 131 / 255f, 112 / 255f, 1);
    private static readonly Color TemperateRainforest = new(29 / 255f, 73 / 255f, 40 / 255f, 1);
    private static readonly Color Grassland = new(164 / 255f, 225 / 255f, 99 / 255f, 1);
    private static readonly Color SeasonalForest = new(73 / 255f, 100 / 255f, 35 / 255f, 1);
    private static readonly Color BorealForest = new(95 / 255f, 115 / 255f, 62 / 255f, 1);
    private static readonly Color Woodland = new(139 / 255f, 175 / 255f, 90 / 255f, 1);

    public static Texture2D CalculateNormalMap(Texture2D source, float strength)
    {
        var pixels = new Color[source.width * source.height];
        strength = Mathf.Clamp(strength, 0.0F, 10.0F);
        var result = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);

        for (var by = 0; by < result.height; by++)
        {
            for (var bx = 0; bx < result.width; bx++)
            {
                var xLeft = source.GetPixel(bx - 1, by).grayscale * strength;
                var xRight = source.GetPixel(bx + 1, by).grayscale * strength;
                var yUp = source.GetPixel(bx, by - 1).grayscale * strength;
                var yDown = source.GetPixel(bx, by + 1).grayscale * strength;
                var xDelta = (xLeft - xRight + 1) * 0.5f;
                var yDelta = (yUp - yDown + 1) * 0.5f;

                pixels[bx + by * source.width] = new Color(xDelta, yDelta, 1.0f, yDelta);
            }
        }

        result.SetPixels(pixels);
        result.wrapMode = TextureWrapMode.Clamp;
        result.Apply();
        return result;
    }

    public static Texture2D GetBiomePalette()
    {
        var texture = new Texture2D(128, 128);
        var pixels = new Color[128 * 128];

        for (var x = 0; x < 128; x++)
        {
            for (var y = 0; y < 128; y++)
            {
                if (x < 10)
                    pixels[x + y * 128] = Ice;
                else if (x < 20)
                    pixels[x + y * 128] = Desert;
                else if (x < 30)
                    pixels[x + y * 128] = Savanna;
                else if (x < 40)
                    pixels[x + y * 128] = TropicalRainforest;
                else if (x < 50)
                    pixels[x + y * 128] = Tundra;
                else if (x < 60)
                    pixels[x + y * 128] = TemperateRainforest;
                else if (x < 70)
                    pixels[x + y * 128] = Grassland;
                else if (x < 80)
                    pixels[x + y * 128] = SeasonalForest;
                else if (x < 90)
                    pixels[x + y * 128] = BorealForest;
                else if (x < 100)
                    pixels[x + y * 128] = Woodland;
            }
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Texture2D GetBumpMap(int width, int height, Tile[,] tiles)
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                pixels[x + y * width] = tiles[x, y].HeightType switch
                {
                    HeightType.DeepWater => new Color(0, 0, 0, 1),
                    HeightType.ShallowWater => new Color(0, 0, 0, 1),
                    HeightType.Sand => new Color(0.3f, 0.3f, 0.3f, 1),
                    HeightType.Grass => new Color(0.45f, 0.45f, 0.45f, 1),
                    HeightType.Forest => new Color(0.6f, 0.6f, 0.6f, 1),
                    HeightType.Rock => new Color(0.75f, 0.75f, 0.75f, 1),
                    HeightType.Snow => new Color(1, 1, 1, 1),
                    HeightType.River => new Color(0, 0, 0, 1),
                    _ => pixels[x + y * width],
                };

                if (!tiles[x, y].Collidable)
                {
                    pixels[x + y * width] = Color.Lerp(
                        Color.white,
                        Color.black,
                        tiles[x, y].HeightValue * 2
                    );
                }
            }
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Texture2D GetHeightMapTexture(int width, int height, Tile[,] tiles)
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                pixels[x + y * width] = tiles[x, y].HeightType switch
                {
                    HeightType.DeepWater => new Color(0, 0, 0, 1),
                    HeightType.ShallowWater => new Color(0, 0, 0, 1),
                    HeightType.Sand => new Color(0.3f, 0.3f, 0.3f, 1),
                    HeightType.Grass => new Color(0.45f, 0.45f, 0.45f, 1),
                    HeightType.Forest => new Color(0.6f, 0.6f, 0.6f, 1),
                    HeightType.Rock => new Color(0.75f, 0.75f, 0.75f, 1),
                    HeightType.Snow => new Color(1, 1, 1, 1),
                    HeightType.River => new Color(0, 0, 0, 1),
                    _ => pixels[x + y * width],
                };
            }
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Texture2D GetHeatMapTexture(int width, int height, Tile[,] tiles)
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                pixels[x + y * width] = tiles[x, y].HeatType switch
                {
                    HeatType.Coldest => Coldest,
                    HeatType.Colder => Colder,
                    HeatType.Cold => Cold,
                    HeatType.Warm => Warm,
                    HeatType.Warmer => Warmer,
                    HeatType.Warmest => Warmest,
                    _ => pixels[x + y * width],
                };

                //darken the color if a edge tile
                if ((int)tiles[x, y].HeightType > 2 && tiles[x, y].Bitmask != 15)
                    pixels[x + y * width] = Color.Lerp(pixels[x + y * width], Color.black, 0.4f);
            }
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Texture2D GetMoistureMapTexture(int width, int height, Tile[,] tiles)
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var t = tiles[x, y];

                pixels[x + y * width] = t.MoistureType switch
                {
                    MoistureType.Dryest => Dryest,
                    MoistureType.Dryer => Dryer,
                    MoistureType.Dry => Dry,
                    MoistureType.Wet => Wet,
                    MoistureType.Wetter => Wetter,
                    _ => Wettest,
                };
            }
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }

    public static Texture2D GetBiomeMapTexture(
        int width,
        int height,
        Tile[,] tiles,
        float coldest,
        float colder,
        float cold
    )
    {
        var texture = new Texture2D(width, height);
        var pixels = new Color[width * height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var value = tiles[x, y].BiomeType;

                pixels[x + y * width] = tiles[x, y].HeightType switch
                {
                    // Water tiles
                    HeightType.DeepWater => DeepColor,
                    HeightType.ShallowWater => ShallowColor,
                    _ => value switch
                    {
                        BiomeType.Ice => Ice,
                        BiomeType.BorealForest => BorealForest,
                        BiomeType.Desert => Desert,
                        BiomeType.Grassland => Grassland,
                        BiomeType.SeasonalForest => SeasonalForest,
                        BiomeType.Tundra => Tundra,
                        BiomeType.Savanna => Savanna,
                        BiomeType.TemperateRainforest => TemperateRainforest,
                        BiomeType.TropicalRainforest => TropicalRainforest,
                        BiomeType.Woodland => Woodland,
                        _ => pixels[x + y * width],
                    },
                };

                // draw rivers
                if (tiles[x, y].HeightType == HeightType.River)
                {
                    var heatValue = tiles[x, y].HeatValue;

                    pixels[x + y * width] = tiles[x, y].HeatType switch
                    {
                        HeatType.Coldest => Color.Lerp(IceWater, ColdWater, heatValue / coldest),
                        HeatType.Colder => Color.Lerp(
                            ColdWater,
                            RiverWater,
                            (heatValue - coldest) / (colder - coldest)
                        ),
                        HeatType.Cold => Color.Lerp(
                            RiverWater,
                            ShallowColor,
                            (heatValue - colder) / (cold - colder)
                        ),
                        _ => ShallowColor,
                    };
                }

                // add a outline
                if (
                    tiles[x, y].HeightType < HeightType.Shore
                    || tiles[x, y].HeightType == HeightType.River
                )
                    continue;
                if (tiles[x, y].BiomeBitmask != 15)
                    pixels[x + y * width] = Color.Lerp(pixels[x + y * width], Color.black, 0.35f);
            }
        }

        texture.SetPixels(pixels);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();
        return texture;
    }
}
