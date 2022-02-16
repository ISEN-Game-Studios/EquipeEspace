using System.Collections.Generic;
using System;

namespace SpaceTeam
{
    class Items
    {
        public List<Item> binaryItems;
        public List<Item> ternaryItems;

        public Items()
        {
            binaryItems = new List<Item>();
            ternaryItems = new List<Item>();

            binaryItems.Add(new Button("Ordres directs", "Désobeir aux ordres directs", "Désobeir", 3, 3)); // Button 3 * 3
        }
    }
}
