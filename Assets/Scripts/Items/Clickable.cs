using UnityEngine;

namespace SpaceTeam
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Clickable", menuName = "Items/Clickable")]
    public class Clickable : ItemData
    {
        public Clickable() : base(Type.OnClick)
        {

        }
    }
}
