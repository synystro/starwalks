using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {

    [CreateAssetMenu(fileName = "New Store", menuName = "Store")]
    public class Store : ScriptableObject {

        public new string name;
        public int id;
        [Space(20)]
        public int itemsForSaleAmount;
        [Space(10)]        
        public List<Item> possibleItems = new List<Item>();

    }

}
