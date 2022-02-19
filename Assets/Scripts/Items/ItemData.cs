using UnityEngine;

namespace SpaceTeam
{
    public enum Type
    {
        OnClick,
        OnState
    }

    public enum Shape
    {
        Horizontal,
        Vertical,
        Small,
        Big
    }

    public abstract class ItemData : ScriptableObject
    {
        public Type Type { get; protected set; }

        [Space(5)]
        public string Name;

        [Space(10)]
        public Shape Shape;
        public GameObject Prefab;

        [Space(10)]
        public string Instruction;

        [Space(5)]
        public string[] Values;

        [HideInInspector]
        public Vector2Int Size;

        [HideInInspector]
        public int ID;

        public ItemData(Type type)
        {
            Type = type;

            if (Shape == Shape.Small)
                Size = Vector2Int.one;
            else if (Shape == Shape.Big)
                Size = Vector2Int.right;
            else if (Shape == Shape.Big)
                Size = 2 * Vector2Int.one;
            else if (Shape == Shape.Vertical)
                Size = Vector2Int.up;
        }
    }
}