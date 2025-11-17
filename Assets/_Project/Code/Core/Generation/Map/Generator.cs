using System.Collections.Generic;
using System.Linq;
using _Project.Code.Core.Map;
using AccidentalNoise;
using Code.Core.Chunks;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class Generator : MonoBehaviour
{
    private int _seed;

    // Adjustable variables for Unity Inspector
    [FormerlySerializedAs("Width")]
    [Header("Generator Values")]
    [SerializeField]
    protected int width = 512;
    public int Width => width;

    [FormerlySerializedAs("Height")]
    [SerializeField]
    protected int height = 512;
    public int Height => height;

    [FormerlySerializedAs("Chunk size")]
    [SerializeField]
    protected int chunkSize = 16;

    [FormerlySerializedAs("TerrainOctaves")]
    [Header("Height Map")]
    [SerializeField]
    protected int terrainOctaves = 6;

    [FormerlySerializedAs("TerrainFrequency")]
    [SerializeField]
    protected double terrainFrequency = 1.25;

    [FormerlySerializedAs("DeepWater")]
    [SerializeField]
    protected float deepWater = 0.2f;

    [FormerlySerializedAs("ShallowWater")]
    [SerializeField]
    protected float shallowWater = 0.4f;

    [FormerlySerializedAs("Sand")]
    [SerializeField]
    protected float sand = 0.5f;

    [FormerlySerializedAs("Grass")]
    [SerializeField]
    protected float grass = 0.7f;

    [FormerlySerializedAs("Forest")]
    [SerializeField]
    protected float forest = 0.8f;

    [FormerlySerializedAs("Rock")]
    [SerializeField]
    protected float rock = 0.9f;

    [FormerlySerializedAs("HeatOctaves")]
    [Header("Heat Map")]
    [SerializeField]
    protected int heatOctaves = 4;

    [FormerlySerializedAs("HeatFrequency")]
    [SerializeField]
    protected double heatFrequency = 3.0;

    [FormerlySerializedAs("ColdestValue")]
    [SerializeField]
    protected float coldestValue = 0.05f;
    public float ColdestValue => coldestValue;

    [FormerlySerializedAs("ColderValue")]
    [SerializeField]
    protected float colderValue = 0.18f;
    public float ColderValue => colderValue;

    [FormerlySerializedAs("ColdValue")]
    [SerializeField]
    protected float coldValue = 0.4f;
    public float ColdValue => coldValue;

    [FormerlySerializedAs("WarmValue")]
    [SerializeField]
    protected float warmValue = 0.6f;

    [FormerlySerializedAs("WarmerValue")]
    [SerializeField]
    protected float warmerValue = 0.8f;

    [FormerlySerializedAs("MoistureOctaves")]
    [Header("Moisture Map")]
    [SerializeField]
    protected int moistureOctaves = 4;

    [FormerlySerializedAs("MoistureFrequency")]
    [SerializeField]
    protected double moistureFrequency = 3.0;

    [FormerlySerializedAs("DryerValue")]
    [SerializeField]
    protected float dryerValue = 0.27f;

    [FormerlySerializedAs("DryValue")]
    [SerializeField]
    protected float dryValue = 0.4f;

    [FormerlySerializedAs("WetValue")]
    [SerializeField]
    protected float wetValue = 0.6f;

    [FormerlySerializedAs("WetterValue")]
    [SerializeField]
    protected float wetterValue = 0.8f;

    [FormerlySerializedAs("WettestValue")]
    [SerializeField]
    protected float wettestValue = 0.9f;

    [FormerlySerializedAs("RiverCount")]
    [Header("Rivers")]
    [SerializeField]
    protected int riverCount = 40;

    [FormerlySerializedAs("MinRiverHeight")]
    [SerializeField]
    protected float minRiverHeight = 0.6f;

    [FormerlySerializedAs("MaxRiverAttempts")]
    [SerializeField]
    protected int maxRiverAttempts = 1000;

    [FormerlySerializedAs("MinRiverTurns")]
    [SerializeField]
    protected int minRiverTurns = 18;

    [FormerlySerializedAs("MinRiverLength")]
    [SerializeField]
    protected int minRiverLength = 20;

    [FormerlySerializedAs("MaxRiverIntersections")]
    [SerializeField]
    protected int maxRiverIntersections = 2;

    [FormerlySerializedAs("HeightMapRenderer")]
    [Header("Maps renderers")]
    [SerializeField]
    private MeshRenderer heightMapRenderer;

    [FormerlySerializedAs("HeatMapRenderer")]
    [SerializeField]
    private MeshRenderer heatMapRenderer;

    [FormerlySerializedAs("MoistureMapRenderer")]
    [SerializeField]
    private MeshRenderer moistureMapRenderer;

    [FormerlySerializedAs("BiomeMapRenderer")]
    [SerializeField]
    private MeshRenderer biomeMapRenderer;

    // private variables
    private ImplicitFractal _heightMap;
    private ImplicitCombiner _heatMap;
    private ImplicitFractal _moistureMap;

    private MapData _heightData;
    private MapData _heatData;
    private MapData _moistureData;

    private Tile[,] _tiles;
    private Dictionary<Vector2Int, Chunk> _chunks = new();

    private readonly List<TileGroup> _waters = new();
    private readonly List<TileGroup> _lands = new();

    private List<River> _rivers = new();
    private readonly List<RiverGroup> _riverGroups = new();

    protected readonly BiomeType[,] BiomeTable =
    {
        {
            BiomeType.Ice,
            BiomeType.Tundra,
            BiomeType.Grassland,
            BiomeType.Desert,
            BiomeType.Desert,
            BiomeType.Desert,
        },
        {
            BiomeType.Ice,
            BiomeType.Tundra,
            BiomeType.Grassland,
            BiomeType.Desert,
            BiomeType.Desert,
            BiomeType.Desert,
        },
        {
            BiomeType.Ice,
            BiomeType.Tundra,
            BiomeType.Woodland,
            BiomeType.Woodland,
            BiomeType.Savanna,
            BiomeType.Savanna,
        },
        {
            BiomeType.Ice,
            BiomeType.Tundra,
            BiomeType.BorealForest,
            BiomeType.Woodland,
            BiomeType.Savanna,
            BiomeType.Savanna,
        },
        {
            BiomeType.Ice,
            BiomeType.Tundra,
            BiomeType.BorealForest,
            BiomeType.SeasonalForest,
            BiomeType.TropicalRainforest,
            BiomeType.TropicalRainforest,
        },
        {
            BiomeType.Ice,
            BiomeType.Tundra,
            BiomeType.BorealForest,
            BiomeType.TemperateRainforest,
            BiomeType.TropicalRainforest,
            BiomeType.TropicalRainforest,
        },
    };

    //Public accessors for external scripts
    public Tile[,] GetTiles()
    {
        return _tiles;
    }

    public Dictionary<Vector2Int, Chunk> GetChunks()
    {
        return _chunks;
    }

    public int GetChunkSize()
    {
        return chunkSize;
    }

    private void Start()
    {

        _seed = Random.Range(0, int.MaxValue);

        Initialize();
        GetData();
        LoadTiles();

        UpdateNeighbors();

        GenerateRivers();
        BuildRiverGroups();
        DigRiverGroups();
        AdjustMoistureMap();

        UpdateBitmasks();
        FloodFill();

        GenerateBiomeMap();
        UpdateBiomeBitmask();

        heightMapRenderer.materials[0].mainTexture = TextureGenerator.GetHeightMapTexture(
            width,
            height,
            _tiles
        );
        heatMapRenderer.materials[0].mainTexture = TextureGenerator.GetHeatMapTexture(
            width,
            height,
            _tiles
        );
        moistureMapRenderer.materials[0].mainTexture = TextureGenerator.GetMoistureMapTexture(
            width,
            height,
            _tiles
        );
        biomeMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(
            width,
            height,
            _tiles,
            coldestValue,
            colderValue,
            coldValue
        );
    }

    private void Initialize()
    {
        // Initialize the HeightMap Generator
        _heightMap = new ImplicitFractal(
            FractalType.MULTI,
            BasisType.SIMPLEX,
            InterpolationType.QUINTIC,
            terrainOctaves,
            terrainFrequency,
            _seed
        );

        // Initialize the Heat map
        var gradient = new ImplicitGradient(1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1);
        var heatFractal = new ImplicitFractal(
            FractalType.MULTI,
            BasisType.SIMPLEX,
            InterpolationType.QUINTIC,
            heatOctaves,
            heatFrequency,
            _seed
        );

        _heatMap = new ImplicitCombiner(CombinerType.MULTIPLY);
        _heatMap.AddSource(gradient);
        _heatMap.AddSource(heatFractal);

        //moisture map
        _moistureMap = new ImplicitFractal(
            FractalType.MULTI,
            BasisType.SIMPLEX,
            InterpolationType.QUINTIC,
            moistureOctaves,
            moistureFrequency,
            _seed
        );
    }

    private void UpdateBiomeBitmask()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _tiles[x, y].UpdateBiomeBitmask();
            }
        }
    }

    private BiomeType GetBiomeType(Tile tile)
    {
        return BiomeTable[(int)tile.MoistureType, (int)tile.HeatType];
    }

    private void GenerateBiomeMap()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (!_tiles[x, y].Collidable)
                    continue;

                var t = _tiles[x, y];
                t.BiomeType = GetBiomeType(t);
            }
        }
    }

    private void AddMoisture(Tile t, int radius)
    {
        var center = new Vector2(t.X, t.Y);
        var curr = radius;

        while (curr > 0)
        {
            var x1 = MathHelper.Mod(t.X - curr, width);
            var x2 = MathHelper.Mod(t.X + curr, width);
            var y = t.Y;

            AddMoisture(_tiles[x1, y], 0.025f / (center - new Vector2(x1, y)).magnitude);

            for (var i = 0; i < curr; i++)
            {
                AddMoisture(
                    _tiles[x1, MathHelper.Mod(y + i + 1, height)],
                    0.025f / (center - new Vector2(x1, MathHelper.Mod(y + i + 1, height))).magnitude
                );
                AddMoisture(
                    _tiles[x1, MathHelper.Mod(y - (i + 1), height)],
                    0.025f
                        / (center - new Vector2(x1, MathHelper.Mod(y - (i + 1), height))).magnitude
                );

                AddMoisture(
                    _tiles[x2, MathHelper.Mod(y + i + 1, height)],
                    0.025f / (center - new Vector2(x2, MathHelper.Mod(y + i + 1, height))).magnitude
                );
                AddMoisture(
                    _tiles[x2, MathHelper.Mod(y - (i + 1), height)],
                    0.025f
                        / (center - new Vector2(x2, MathHelper.Mod(y - (i + 1), height))).magnitude
                );
            }
            curr--;
        }
    }

    private void AddMoisture(Tile t, float amount)
    {
        _moistureData.Data[t.X, t.Y] += amount;
        t.MoistureValue += amount;
        if (t.MoistureValue > 1)
            t.MoistureValue = 1;

        //set moisture type
        if (t.MoistureValue < dryerValue)
            t.MoistureType = MoistureType.Dryest;
        else if (t.MoistureValue < dryValue)
            t.MoistureType = MoistureType.Dryer;
        else if (t.MoistureValue < wetValue)
            t.MoistureType = MoistureType.Dry;
        else if (t.MoistureValue < wetterValue)
            t.MoistureType = MoistureType.Wet;
        else if (t.MoistureValue < wettestValue)
            t.MoistureType = MoistureType.Wetter;
        else
            t.MoistureType = MoistureType.Wettest;
    }

    private void AdjustMoistureMap()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var t = _tiles[x, y];
                if (t.HeightType == HeightType.River)
                {
                    AddMoisture(t, 60);
                }
            }
        }
    }

    private void DigRiverGroups()
    {
        foreach (var group in _riverGroups)
        {
            River longest = null;

            //Find longest river in this group
            foreach (
                var river in group.Rivers.Where(river =>
                    longest == null || longest.Tiles.Count < river.Tiles.Count
                )
            )
            {
                longest = river;
            }

            if (longest == null)
                continue;
            {
                //Dig out longest path first
                DigRiver(longest);

                foreach (var river in group.Rivers.Where(river => river != longest))
                {
                    DigRiver(river, longest);
                }
            }
        }
    }

    private void BuildRiverGroups()
    {
        //loop each tile, checking if it belongs to multiple rivers
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var t = _tiles[x, y];

                if (t.Rivers.Count <= 1)
                    continue;
                // multiple rivers == intersection
                RiverGroup group = null;

                // Does a rivergroup already exist for this group?
                foreach (var tileRiver in t.Rivers)
                {
                    foreach (var riverGroup in _riverGroups)
                    {
                        foreach (var river in riverGroup.Rivers)
                        {
                            if (river.ID == tileRiver.ID)
                            {
                                group = riverGroup;
                            }
                            if (group != null)
                                break;
                        }

                        if (group != null)
                            break;
                    }

                    if (group != null)
                        break;
                }

                // existing group found -- add to it
                if (group != null)
                {
                    foreach (var river in t.Rivers.Where(t1 => !group.Rivers.Contains(t1)))
                    {
                        group.Rivers.Add(river);
                    }
                }
                else //No existing group found - create a new one
                {
                    group = new RiverGroup();
                    foreach (var river in t.Rivers)
                    {
                        group.Rivers.Add(river);
                    }
                    _riverGroups.Add(group);
                }
            }
        }
    }

    private void GenerateRivers()
    {
        var attempts = 0;
        var localRiverCount = riverCount;
        _rivers = new List<River>();

        // Generate some rivers
        while (localRiverCount > 0 && attempts < maxRiverAttempts)
        {
            // Get a random tile
            var x = Random.Range(0, width);
            var y = Random.Range(0, height);
            var tile = _tiles[x, y];

            // validate the tile
            if (!tile.Collidable)
                continue;
            if (tile.Rivers.Count > 0)
                continue;

            if (tile.HeightValue > minRiverHeight)
            {
                // Tile is good to start river from
                var river = new River(localRiverCount)
                {
                    // Figure out the direction this river will try to flow
                    CurrentDirection = tile.GetLowestNeighbor(this),
                };

                // Recursively find a path to water
                FindPathToWater(tile, river.CurrentDirection, ref river);

                // Validate the generated river
                if (
                    river.TurnCount < minRiverTurns
                    || river.Tiles.Count < minRiverLength
                    || river.Intersections > maxRiverIntersections
                )
                {
                    //Validation failed - remove this river
                    foreach (var t in river.Tiles)
                    {
                        t.Rivers.Remove(river);
                    }
                }
                else if (river.Tiles.Count >= minRiverLength)
                {
                    //Validation passed - Add river to list
                    _rivers.Add(river);
                    tile.Rivers.Add(river);
                    localRiverCount--;
                }
            }
            attempts++;
        }
    }

    // Dig river based on a parent river vein
    private static void DigRiver(River river, River parent)
    {
        var intersectionID = 0;
        var intersectionSize = 0;

        // determine point of intersection
        for (var i = 0; i < river.Tiles.Count; i++)
        {
            var t1 = river.Tiles[i];
            foreach (var t2 in parent.Tiles.Where(t2 => t1 == t2))
            {
                intersectionID = i;
                intersectionSize = t2.RiverSize;
            }
        }

        var counter = 0;
        var intersectionCount = river.Tiles.Count - intersectionID;
        var size = Random.Range(intersectionSize, 5);
        river.Length = river.Tiles.Count;

        // randomize size change
        var two = river.Length / 2;
        var three = two / 2;
        var four = three / 2;
        var five = four / 2;

        var twomin = two / 3;
        var threemin = three / 3;
        var fourmin = four / 3;
        var fivemin = five / 3;

        // randomize length of each size
        var count1 = Random.Range(fivemin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        var count2 = count1 + Random.Range(fourmin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        var count3 = count2 + Random.Range(threemin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        var count4 = count3 + Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            var extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0)
                {
                    count1--;
                    count2--;
                    count3--;
                    count4--;
                    extra--;
                }
                else if (count2 > 0)
                {
                    count2--;
                    count3--;
                    count4--;
                    extra--;
                }
                else if (count3 > 0)
                {
                    count3--;
                    count4--;
                    extra--;
                }
                else if (count4 > 0)
                {
                    count4--;
                    extra--;
                }
            }
        }

        switch (intersectionSize)
        {
            // adjust size of river at intersection point
            case 1:
                count4 = intersectionCount;
                count1 = 0;
                count2 = 0;
                count3 = 0;
                break;
            case 2:
                count3 = intersectionCount;
                count1 = 0;
                count2 = 0;
                break;
            case 3:
                count2 = intersectionCount;
                count1 = 0;
                break;
            case 4:
                count1 = intersectionCount;
                break;
            default:
                count1 = 0;
                count2 = 0;
                count3 = 0;
                count4 = 0;
                break;
        }

        // dig out the river
        for (var i = river.Tiles.Count - 1; i >= 0; i--)
        {
            var t = river.Tiles[i];

            if (counter < count1)
            {
                t.DigRiver(river, 4);
            }
            else if (counter < count2)
            {
                t.DigRiver(river, 3);
            }
            else if (counter < count3)
            {
                t.DigRiver(river, 2);
            }
            else if (counter < count4)
            {
                t.DigRiver(river, 1);
            }
            else
            {
                t.DigRiver(river, 0);
            }
            counter++;
        }
    }

    // Dig river
    private void DigRiver(River river)
    {
        var counter = 0;

        // How wide are we digging this river?
        var size = Random.Range(1, 5);
        river.Length = river.Tiles.Count;

        // randomize size change
        var two = river.Length / 2;
        var three = two / 2;
        var four = three / 2;
        var five = four / 2;

        var twoMin = two / 3;
        var threeMin = three / 3;
        var fourMin = four / 3;
        var fiveMin = five / 3;

        // randomize lenght of each size
        var count1 = Random.Range(fiveMin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        var count2 = count1 + Random.Range(fourMin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        var count3 = count2 + Random.Range(threeMin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        var count4 = count3 + Random.Range(twoMin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            var extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0)
                {
                    count1--;
                    count2--;
                    count3--;
                    count4--;
                    extra--;
                }
                else if (count2 > 0)
                {
                    count2--;
                    count3--;
                    count4--;
                    extra--;
                }
                else if (count3 > 0)
                {
                    count3--;
                    count4--;
                    extra--;
                }
                else if (count4 > 0)
                {
                    count4--;
                    extra--;
                }
            }
        }

        // Dig it out
        for (var i = river.Tiles.Count - 1; i >= 0; i--)
        {
            var t = river.Tiles[i];

            if (counter < count1)
            {
                t.DigRiver(river, 4);
            }
            else if (counter < count2)
            {
                t.DigRiver(river, 3);
            }
            else if (counter < count3)
            {
                t.DigRiver(river, 2);
            }
            else if (counter < count4)
            {
                t.DigRiver(river, 1);
            }
            else
            {
                t.DigRiver(river, 0);
            }
            counter++;
        }
    }

    private void FindPathToWater(Tile tile, Direction direction, ref River river)
    {
        while (true)
        {
            if (tile.Rivers.Contains(river))
                return;

            // check if there is already a river on this tile
            if (tile.Rivers.Count > 0)
                river.Intersections++;

            river.AddTile(tile);

            // get neighbors
            var left = GetLeft(tile);
            var right = GetRight(tile);
            var top = GetTop(tile);
            var bottom = GetBottom(tile);

            float leftValue = int.MaxValue;
            float rightValue = int.MaxValue;
            float topValue = int.MaxValue;
            float bottomValue = int.MaxValue;

            // query height values of neighbors
            if (left.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left))
                leftValue = left.HeightValue;
            if (right.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(right))
                rightValue = right.HeightValue;
            if (top.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(top))
                topValue = top.HeightValue;
            if (bottom.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottom))
                bottomValue = bottom.HeightValue;

            // if neighbor is existing river that is not this one, flow into it
            if (bottom.Rivers.Count == 0 && !bottom.Collidable)
                bottomValue = 0;
            if (top.Rivers.Count == 0 && !top.Collidable)
                topValue = 0;
            if (left.Rivers.Count == 0 && !left.Collidable)
                leftValue = 0;
            if (right.Rivers.Count == 0 && !right.Collidable)
                rightValue = 0;

            switch (direction)
            {
                // override flow direction if a tile is significantly lower
                case Direction.Left:
                {
                    if (Mathf.Abs(rightValue - leftValue) < 0.1f)
                        rightValue = int.MaxValue;
                    break;
                }
                case Direction.Right:
                {
                    if (Mathf.Abs(rightValue - leftValue) < 0.1f)
                        leftValue = int.MaxValue;
                    break;
                }
                case Direction.Top:
                {
                    if (Mathf.Abs(topValue - bottomValue) < 0.1f)
                        bottomValue = int.MaxValue;
                    break;
                }
                case Direction.Bottom:
                {
                    if (Mathf.Abs(topValue - bottomValue) < 0.1f)
                        topValue = int.MaxValue;
                    break;
                }
            }

            // find mininum
            var min = Mathf.Min(Mathf.Min(Mathf.Min(leftValue, rightValue), topValue), bottomValue);

            // if no minimum found - exit
            if (Mathf.Approximately(min, int.MaxValue))
                return;

            //Move to next neighbor
            if (Mathf.Approximately(min, leftValue))
            {
                if (!left.Collidable)
                    return;
                if (river.CurrentDirection != Direction.Left)
                {
                    river.TurnCount++;
                    river.CurrentDirection = Direction.Left;
                }

                tile = left;
                continue;
            }

            if (Mathf.Approximately(min, rightValue))
            {
                if (right.Collidable)
                {
                    if (river.CurrentDirection != Direction.Right)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Right;
                    }

                    tile = right;
                    continue;
                }
            }
            else if (Mathf.Approximately(min, bottomValue))
            {
                if (bottom.Collidable)
                {
                    if (river.CurrentDirection != Direction.Bottom)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Bottom;
                    }

                    tile = bottom;
                    continue;
                }
            }
            else if (Mathf.Approximately(min, topValue))
            {
                if (top.Collidable)
                {
                    if (river.CurrentDirection != Direction.Top)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = Direction.Top;
                    }

                    tile = top;
                    continue;
                }
            }

            break;
        }
    }

    // Extract data from a noise module
    private void GetData()
    {
        _heightData = new MapData(width, height);
        _heatData = new MapData(width, height);
        _moistureData = new MapData(width, height);

        // loop through each x,y point - get height value
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                // WRAP ON BOTH AXIS
                // Noise range
                const float x1 = 0;
                const float x2 = 2;
                const float y1 = 0;
                const float y2 = 2;
                const float dx = x2 - x1;
                const float dy = y2 - y1;

                // Sample noise at smaller intervals
                var s = x / (float)width;
                var t = y / (float)height;

                // Calculate our 4D coordinates
                var nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                var ny = y1 + Mathf.Cos(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);
                var nz = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                var nw = y1 + Mathf.Sin(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);

                var heightValue = (float)_heightMap.Get(nx, ny, nz, nw);
                var heatValue = (float)_heatMap.Get(nx, ny, nz, nw);
                var moistureValue = (float)_moistureMap.Get(nx, ny, nz, nw);

                // keep track of the max and min values found
                if (heightValue > _heightData.Max)
                    _heightData.Max = heightValue;
                if (heightValue < _heightData.Min)
                    _heightData.Min = heightValue;

                if (heatValue > _heatData.Max)
                    _heatData.Max = heatValue;
                if (heatValue < _heatData.Min)
                    _heatData.Min = heatValue;

                if (moistureValue > _moistureData.Max)
                    _moistureData.Max = moistureValue;
                if (moistureValue < _moistureData.Min)
                    _moistureData.Min = moistureValue;

                _heightData.Data[x, y] = heightValue;
                _heatData.Data[x, y] = heatValue;
                _moistureData.Data[x, y] = moistureValue;
            }
        }
    }

    // Build a Tile array from our data
    private void LoadTiles()
    {
        _tiles = new Tile[width, height];
        _chunks.Clear();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var t = new Tile { X = x, Y = y };

                //set heightmap value
                var heightValue = _heightData.Data[x, y];
                heightValue = (heightValue - _heightData.Min) / (_heightData.Max - _heightData.Min);
                t.HeightValue = heightValue;

                if (heightValue < deepWater)
                {
                    t.HeightType = HeightType.DeepWater;
                    t.Collidable = false;
                }
                else if (heightValue < shallowWater)
                {
                    t.HeightType = HeightType.ShallowWater;
                    t.Collidable = false;
                }
                else if (heightValue < sand)
                {
                    t.HeightType = HeightType.Sand;
                    t.Collidable = true;
                }
                else if (heightValue < grass)
                {
                    t.HeightType = HeightType.Grass;
                    t.Collidable = true;
                }
                else if (heightValue < forest)
                {
                    t.HeightType = HeightType.Forest;
                    t.Collidable = true;
                }
                else if (heightValue < rock)
                {
                    t.HeightType = HeightType.Rock;
                    t.Collidable = true;
                }
                else
                {
                    t.HeightType = HeightType.Snow;
                    t.Collidable = true;
                }

                switch (t.HeightType)
                {
                    //adjust moisture based on height
                    case HeightType.DeepWater:
                        _moistureData.Data[t.X, t.Y] += 8f * t.HeightValue;
                        break;
                    case HeightType.ShallowWater:
                        _moistureData.Data[t.X, t.Y] += 3f * t.HeightValue;
                        break;
                    case HeightType.Shore:
                        _moistureData.Data[t.X, t.Y] += 1f * t.HeightValue;
                        break;
                    case HeightType.Sand:
                        _moistureData.Data[t.X, t.Y] += 0.2f * t.HeightValue;
                        break;
                }

                //Moisture Map Analyze
                var moistureValue = _moistureData.Data[x, y];
                moistureValue =
                    (moistureValue - _moistureData.Min) / (_moistureData.Max - _moistureData.Min);
                t.MoistureValue = moistureValue;

                //set moisture type
                if (moistureValue < dryerValue)
                    t.MoistureType = MoistureType.Dryest;
                else if (moistureValue < dryValue)
                    t.MoistureType = MoistureType.Dryer;
                else if (moistureValue < wetValue)
                    t.MoistureType = MoistureType.Dry;
                else if (moistureValue < wetterValue)
                    t.MoistureType = MoistureType.Wet;
                else if (moistureValue < wettestValue)
                    t.MoistureType = MoistureType.Wetter;
                else
                    t.MoistureType = MoistureType.Wettest;

                switch (t.HeightType)
                {
                    // Adjust Heat Map based on Height - Higher == colder
                    case HeightType.Forest:
                        _heatData.Data[t.X, t.Y] -= 0.1f * t.HeightValue;
                        break;
                    case HeightType.Rock:
                        _heatData.Data[t.X, t.Y] -= 0.25f * t.HeightValue;
                        break;
                    case HeightType.Snow:
                        _heatData.Data[t.X, t.Y] -= 0.4f * t.HeightValue;
                        break;
                    default:
                        _heatData.Data[t.X, t.Y] += 0.01f * t.HeightValue;
                        break;
                }

                // Set heat value
                var heatValue = _heatData.Data[x, y];
                heatValue = (heatValue - _heatData.Min) / (_heatData.Max - _heatData.Min);
                t.HeatValue = heatValue;

                // set heat type
                if (heatValue < coldestValue)
                    t.HeatType = HeatType.Coldest;
                else if (heatValue < colderValue)
                    t.HeatType = HeatType.Colder;
                else if (heatValue < coldValue)
                    t.HeatType = HeatType.Cold;
                else if (heatValue < warmValue)
                    t.HeatType = HeatType.Warm;
                else if (heatValue < warmerValue)
                    t.HeatType = HeatType.Warmer;
                else
                    t.HeatType = HeatType.Warmest;

                _tiles[x, y] = t;

                // Define chunk coordinates

                int chunkX = x / chunkSize;
                int chunkY = y / chunkSize;
                Vector2Int index = new Vector2Int(chunkX, chunkY);

                // Create chunk
                if (!_chunks.ContainsKey(index))
                    _chunks[index] = new Chunk(new ChunkIndex(chunkX, chunkY));

                _chunks[index].AddTile(t);
            }
        }
        Debug.Log($"World divided into {_chunks.Count} chunks:");
        foreach (var kvp in _chunks)
        {
            var chunk = kvp.Value;
            Debug.Log($"Chunk {chunk.Index.X},{chunk.Index.Y} contains {chunk.Tiles.Count} tiles");

            for (int i = 0; i < chunk.Tiles.Count; i++)
            {
                var t = chunk.Tiles[i];
                Debug.Log($"Tile ({t.X},{t.Y}) Height: {t.HeightValue:F2}, Moisture: {t.MoistureValue:F2}");
            }
        }

    }

    private void UpdateNeighbors()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var t = _tiles[x, y];

                t.Top = GetTop(t);
                t.Bottom = GetBottom(t);
                t.Left = GetLeft(t);
                t.Right = GetRight(t);
            }
        }
    }

    private void UpdateBitmasks()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _tiles[x, y].UpdateBitmask();
            }
        }
    }

    private void FloodFill()
    {
        // Use a stack instead of recursion
        var stack = new Stack<Tile>();

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var t = _tiles[x, y];

                //Tile already flood filled, skip
                if (t.FloodFilled)
                    continue;

                // Land
                if (t.Collidable)
                {
                    var group = new TileGroup { Type = TileGroupType.Land };
                    stack.Push(t);

                    while (stack.Count > 0)
                    {
                        FloodFill(stack.Pop(), ref group, ref stack);
                    }

                    if (group.Tiles.Count > 0)
                        _lands.Add(group);
                }
                // Water
                else
                {
                    var group = new TileGroup { Type = TileGroupType.Water };
                    stack.Push(t);

                    while (stack.Count > 0)
                    {
                        FloodFill(stack.Pop(), ref group, ref stack);
                    }

                    if (group.Tiles.Count > 0)
                        _waters.Add(group);
                }
            }
        }
    }

    private void FloodFill(Tile tile, ref TileGroup tiles, ref Stack<Tile> stack)
    {
        // Validate
        if (tile.FloodFilled)
            return;
        switch (tiles.Type)
        {
            case TileGroupType.Land when !tile.Collidable:
            case TileGroupType.Water when tile.Collidable:
                return;
        }

        // Add to TileGroup
        tiles.Tiles.Add(tile);
        tile.FloodFilled = true;

        // floodfill into neighbors
        var t = GetTop(tile);
        if (!t.FloodFilled && tile.Collidable == t.Collidable)
            stack.Push(t);
        t = GetBottom(tile);
        if (!t.FloodFilled && tile.Collidable == t.Collidable)
            stack.Push(t);
        t = GetLeft(tile);
        if (!t.FloodFilled && tile.Collidable == t.Collidable)
            stack.Push(t);
        t = GetRight(tile);
        if (!t.FloodFilled && tile.Collidable == t.Collidable)
            stack.Push(t);
    }

    private Tile GetTop(Tile t)
    {
        return _tiles[t.X, MathHelper.Mod(t.Y - 1, height)];
    }

    private Tile GetBottom(Tile t)
    {
        return _tiles[t.X, MathHelper.Mod(t.Y + 1, height)];
    }

    private Tile GetLeft(Tile t)
    {
        return _tiles[MathHelper.Mod(t.X - 1, width), t.Y];
    }

    private Tile GetRight(Tile t)
    {
        return _tiles[MathHelper.Mod(t.X + 1, width), t.Y];
    }

    public float GetHeightValue(Tile tile)
    {
        if (tile == null)
            return int.MaxValue;
        return tile.HeightValue;
    }

    public class GeneratorBaker : Baker<Generator>
    {
        public override void Bake(Generator authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GeneratorComponentData());
        }
    }
}

public struct GeneratorComponentData : IComponentData { }
