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

    public abstract class Item : ScriptableObject
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

        public Item(Type type)
        {
            Type = type;
        }
    }
}