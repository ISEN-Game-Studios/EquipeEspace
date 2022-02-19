using UnityEngine;
using SpaceTeam;

public class Item
{
    private ItemData data;

    public Vector2Int Size => data.Size;
    public Vector2Int Position { get; private set; }

    public Item(ItemData data, Vector2Int position)
    {
        this.data = data;

        Position = position;
    }
}
