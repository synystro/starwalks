using UnityEngine;
using System.Collections.Generic;

namespace DarkSeas {

    public class CargoManager : MonoBehaviour {

        [SerializeField] private List<Item> items;        

        private Ship ship;

        public Inventory GetInventory { get { return ship.Cargo; } }

        private void Start() {     
            ship = GetComponentInParent<Ship>();       
            items = ship.Cargo.GetItemList();
        }

    }
}
