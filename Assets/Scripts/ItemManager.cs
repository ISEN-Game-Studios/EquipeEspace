using System.Collections.Generic;
using UnityEngine;
using SpaceTeam;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemData[] items;

    [SerializeField]
    private List<ItemData>[] filteredItems;

    private List<int> usedIDs;
    private List<int> ownedIDs;

    private void Start()
    {
        for (int i = 0; i < items.Length; ++i)
            items[i].ID = i;

        filteredItems = new List<ItemData>[4];

        for (int i = 0; i < filteredItems.Length; ++i)
            filteredItems[i] = new List<ItemData>();
    }

    public void UpdateIDs(int[] ids)
    {
        usedIDs = new List<int>(ids);

        foreach (var items in filteredItems)
            items.Clear();

        foreach (var item in items)
            if (!usedIDs.Exists(id => id == item.ID))
                filteredItems[(int)item.Shape].Add(item);
    }

    public List<Item> Generate(Board board)
    {
        List<Item> items = new List<Item>();

        ownedIDs = new List<int>();

        foreach (var item in board)
        {
            ItemData data = GetRandomData(item.shape);

            items.Add(new Item(data, item.position));
        }

        return items;
    }

    private ItemData GetRandomData(Shape shape)
    {
        int shapeIndex = (int)shape;

        int itemIndex = Random.Range(0, filteredItems[shapeIndex].Count);

        int itemID = filteredItems[shapeIndex][itemIndex].ID;

        usedIDs.Add(itemID);
        ownedIDs.Add(itemID);

        filteredItems[shapeIndex].RemoveAt(itemIndex);

        return items[itemID];
    }

    public int[] GetOwnedIDs()
    {
        return ownedIDs.ToArray();
    }
}
