using System.Collections.Generic;
using System.Linq;
using Code.Core.Chunks;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Code.Core.Generation.Map
{
    public class Generator : MonoBehaviour
    {
        private int _seed;

        // Adjustable variables for Unity Inspector
        [FormerlySerializedAs("Width")]
        [Header("Generator Values")]
        [SerializeField]
        protected int width = 512;

        [FormerlySerializedAs("Height")]
        [SerializeField]
        protected int height = 512;
        [FormerlySerializedAs("Chunk size")]
        [SerializeField]
        protected int chunkSize = 16;
        public int ChunkSize => chunkSize;

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

        [FormerlySerializedAs("ColderValue")]
        [SerializeField]
        protected float colderValue = 0.18f;

        [FormerlySerializedAs("ColdValue")]
        [SerializeField]
        protected float coldValue = 0.4f;

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

        // Map layers
        private HeightMapLayer _heightMapLayer;
        private HeatMapLayer _heatMapLayer;
        private MoistureMapLayer _moistureMapLayer;
        private readonly List<IMapLayer> _mapLayers = new();

        public Tile[,] Tiles;
        public Dictionary<Vector2Int, Chunk> Chunks = new();

    private readonly List<TileGroup> _waters = new();
    private readonly List<TileGroup> _lands = new();

    private List<River> _rivers = new();
    private readonly List<RiverGroup> _riverGroups = new();
    private bool _generated;
    public bool IsGenerated => _generated;

        private readonly BiomeType[,] _biomeTable =
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

        private void Start()
        {

            _seed = Random.Range(0, int.MaxValue);

            ConfigureMaps();
            GenerateMaps();
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
                Tiles
            );
            heatMapRenderer.materials[0].mainTexture = TextureGenerator.GetHeatMapTexture(
                width,
                height,
                Tiles
            );
            moistureMapRenderer.materials[0].mainTexture = TextureGenerator.GetMoistureMapTexture(
                width,
                height,
                Tiles
            );
        biomeMapRenderer.materials[0].mainTexture = TextureGenerator.GetBiomeMapTexture(
            width,
            height,
            Tiles,
            coldestValue,
            colderValue,
            coldValue
        );

        _generated = true;
    }

        private void ConfigureMaps()
        {
            _mapLayers.Clear();

            _heightMapLayer = new HeightMapLayer(
                width,
                height,
                terrainOctaves,
                terrainFrequency,
                deepWater,
                shallowWater,
                sand,
                grass,
                forest,
                rock
            );

            _heatMapLayer = new HeatMapLayer(
                width,
                height,
                heatOctaves,
                heatFrequency,
                coldestValue,
                colderValue,
                coldValue,
                warmValue,
                warmerValue
            );

            _moistureMapLayer = new MoistureMapLayer(
                width,
                height,
                moistureOctaves,
                moistureFrequency,
                dryerValue,
                dryValue,
                wetValue,
                wetterValue,
                wettestValue
            );

            RegisterMap(_heightMapLayer);
            RegisterMap(_heatMapLayer);
            RegisterMap(_moistureMapLayer);
        }

        private void GenerateMaps()
        {
            foreach (var map in _mapLayers)
            {
                map.Generate(_seed);
            }
        }

        private void RegisterMap(IMapLayer map)
        {
            _mapLayers.Add(map);
        }

        private void UpdateBiomeBitmask()
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    Tiles[x, y].UpdateBiomeBitmask();
                }
            }
        }

        private BiomeType GetBiomeType(Tile tile)
        {
            return _biomeTable[(int)tile.MoistureType, (int)tile.HeatType];
        }

        private void GenerateBiomeMap()
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (!Tiles[x, y].Collidable)
                        continue;

                    var t = Tiles[x, y];
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

                AddMoisture(Tiles[x1, y], 0.025f / (center - new Vector2(x1, y)).magnitude);

                for (var i = 0; i < curr; i++)
                {
                    AddMoisture(
                        Tiles[x1, MathHelper.Mod(y + i + 1, height)],
                        0.025f / (center - new Vector2(x1, MathHelper.Mod(y + i + 1, height))).magnitude
                    );
                    AddMoisture(
                        Tiles[x1, MathHelper.Mod(y - (i + 1), height)],
                        0.025f
                        / (center - new Vector2(x1, MathHelper.Mod(y - (i + 1), height))).magnitude
                    );

                    AddMoisture(
                        Tiles[x2, MathHelper.Mod(y + i + 1, height)],
                        0.025f / (center - new Vector2(x2, MathHelper.Mod(y + i + 1, height))).magnitude
                    );
                    AddMoisture(
                        Tiles[x2, MathHelper.Mod(y - (i + 1), height)],
                        0.025f
                        / (center - new Vector2(x2, MathHelper.Mod(y - (i + 1), height))).magnitude
                    );
                }
                curr--;
            }
        }

        private void AddMoisture(Tile t, float amount)
        {
            _moistureMapLayer.AddMoisture(t, amount);
        }

        private void AdjustMoistureMap()
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var t = Tiles[x, y];
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
                    var t = Tiles[x, y];

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
                var tile = Tiles[x, y];

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


        // Build a Tile array from our data
        private void LoadTiles()
        {
            Tiles = new Tile[width, height];
            Chunks.Clear();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var t = new Tile { X = x, Y = y };

                    _heightMapLayer.Apply(t);
                    _moistureMapLayer.ApplyHeightInfluence(t);
                    _heatMapLayer.ApplyHeightInfluence(t);

                    _moistureMapLayer.Apply(t);
                    _heatMapLayer.Apply(t);

                    Tiles[x, y] = t;

                    var chunkX = x / chunkSize;
                    var chunkY = y / chunkSize;
                    var index = new Vector2Int(chunkX, chunkY);

                    if (!Chunks.ContainsKey(index))
                        Chunks[index] = new Chunk(new ChunkIndex(chunkX, chunkY));

                    Chunks[index].AddTile(t);
                }
            }
            Debug.Log($"World divided into {Chunks.Count} chunks:");
            foreach (var kvp in Chunks)
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
                    var t = Tiles[x, y];

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
                    Tiles[x, y].UpdateBitmask();
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
                    var t = Tiles[x, y];

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
            return Tiles[t.X, MathHelper.Mod(t.Y - 1, height)];
        }

        private Tile GetBottom(Tile t)
        {
            return Tiles[t.X, MathHelper.Mod(t.Y + 1, height)];
        }

        private Tile GetLeft(Tile t)
        {
            return Tiles[MathHelper.Mod(t.X - 1, width), t.Y];
        }

        private Tile GetRight(Tile t)
        {
            return Tiles[MathHelper.Mod(t.X + 1, width), t.Y];
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
}
