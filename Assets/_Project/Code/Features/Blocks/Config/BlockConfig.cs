using UnityEngine;

[CreateAssetMenu(menuName = "Blocks/Block")]
public class BlockConfig : ScriptableObject
{
    public BlockType id;

    public Vector2Int idUV;
}
