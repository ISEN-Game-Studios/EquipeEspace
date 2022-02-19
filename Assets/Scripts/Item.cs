using UnityEngine;
using SpaceTeam;

public class Item
{
    public ItemData Data;

    public Vector2Int Size => Data.Size;
    public Vector2Int Position { get; private set; }

    public Item(ItemData data, Vector2Int position)
    {
        this.Data = data;

        Position = position;
    }
}
