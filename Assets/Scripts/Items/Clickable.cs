using UnityEngine;

namespace SpaceTeam
{
    [CreateAssetMenu(fileName = "New Clickable", menuName = "Items/Clickable")]
    public class Clickable : Item
    {
        public Clickable() : base(Type.OnClick)
        {

        }
    }
}
