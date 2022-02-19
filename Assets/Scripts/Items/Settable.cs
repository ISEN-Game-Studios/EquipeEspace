using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceTeam
{
    [CreateAssetMenu(fileName = "New Settable", menuName = "Items/Settable")]
    public class Settable : ItemData
    {
        public Settable() : base(Type.OnState)
        {

        }
    }
}
