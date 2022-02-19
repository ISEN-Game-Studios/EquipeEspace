using UnityEngine;
using SpaceTeam;

[System.Serializable]
public class Item
{
    public ItemData data;

    public Vector2Int Position { get; private set; }

    public ItemData debug_data;
    public Vector2Int debug_position;
    public Vector2Int debug_size;

    public Item(ItemData data, Vector2Int position)
    {
        this.data = data;

        Position = position;

        debug_data = data;
        debug_position = position;
    }
}
