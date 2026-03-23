using _Project.Code.Core.Map;

public interface IMapLayer
{
    string Name { get; }
    int Width { get; }
    int Height { get; }
    MapData Data { get; }

    void Generate(int seed);
    float GetNormalizedValue(int x, int y);
}
