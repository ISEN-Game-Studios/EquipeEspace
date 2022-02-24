using UnityEngine;
using SpaceTeam;

[System.Serializable]
public class Item
{
    public ItemData Data;

    public Vector2Int Position { get; private set; }

    public int current = 0;

    public Item(ItemData data, Vector2Int position)
    {
        Data = data;

        Position = position;
    }

    public (int id, int index) GetAction()
    {
        int index = 0;

        if (Data.Values.Length > 1)
        {
            do
            {
                index = Random.Range(0, Data.Values.Length);
            } while (index == current);
        }

        return (Data.ID, index);
    }

    public string GetInstruction(int index)
    {
        return Data.Instruction.Replace("Name", Data.name).Replace("Value", Data.Values[index]);
    }
}
