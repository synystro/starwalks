using UnityEngine;
using System.Collections.Generic;
using System;

namespace DarkSeas {

    [System.Serializable]
    public class ItemsForSale {

        public string type;
        public List<Item> items = new List<Item>();

    }

    public class StoreInventory : MonoBehaviour {

        public static StoreInventory i;
        public Store store;

        public delegate void OnItemChanged();
        public OnItemChanged onItemChangedCallback;

        public StoreSlot selectedItemSlot;

        public List<ItemsForSale> itemsForSale = new List<ItemsForSale>();
        public int sellValueDivisor = 2;

        private const int STARTING_ITEM_TYPE_INDEX = 3;
        private const int MAX_FIXED_ITEM_AMOUNT = 10;
        private const int MAX_ITEMS_PER_TYPE = 3;

        private StoreInventory() {
            if(i == null)
                i = this;
        }

        private void Start() {

            #region fixed items (ammo, components and fuel)

            ItemsForSale currentFixedItemsForSale = new ItemsForSale();
            itemsForSale.Add(currentFixedItemsForSale);
            currentFixedItemsForSale.type = "Supplies";

            Item[] fixedItems = Resources.LoadAll<Item>("In-Game/Items/_supplies");
            for(int i = 0; i < MAX_ITEMS_PER_TYPE; i++) {
                int randomAmount = UnityEngine.Random.Range(1, MAX_FIXED_ITEM_AMOUNT);
                Item item = Instantiate(fixedItems[i]);
                item.name = fixedItems[i].name;
                item.amount = randomAmount;
                currentFixedItemsForSale.items.Add(item);
            }

            #endregion

            #region varied items

            for (int i = STARTING_ITEM_TYPE_INDEX; i < Enum.GetNames(typeof(Item.Type)).Length; i++) {

                ItemsForSale currentItemsForSale = new ItemsForSale();
                currentItemsForSale.type = ((Item.Type)i).ToString();
                itemsForSale.Add(currentItemsForSale);

                for (int j = 0; j < MAX_ITEMS_PER_TYPE; j++) {

                    Item[] possibleItems = Resources.LoadAll<Item>("In-Game/Items/" + currentItemsForSale.type);
                    
                    int n = UnityEngine.Random.Range(0, possibleItems.Length);

                    while(currentItemsForSale.items.Contains(possibleItems[n])) {
                        n = UnityEngine.Random.Range(0, possibleItems.Length);
                    }

                    Item item = Instantiate(possibleItems[n]);
                    item.name = possibleItems[n].name;
                    item.amount = possibleItems[n].amount;
                    currentItemsForSale.items.Add(item);

                }

            }

            #endregion

        }

        private void Add(int pageIndex, Item item) {

            if (itemsForSale[pageIndex].items.Count < 3) {
                itemsForSale[pageIndex].items.Add(item);
                onItemChangedCallback?.Invoke();
            }

        }

        public void Buy(int pageIndex, Item item) {
            
            if(item.amount <= 0) {
                print(item.name + " is out stock");
                return;
            }

            // check if player has enough scraps to pay
            if(GameManager.ship.scraps >= item.value)
                GameManager.ship.scraps -= item.value;
            else {
                Debug.Log("not enough scraps boi");
                return;
            }

            item.amount -= 1;

            GameManager.ship.ReceiveItem(item);

            onItemChangedCallback?.Invoke();

        }

        public bool Sell(int pageIndex, Item item) {

            Add(pageIndex, item);

            GameManager.ship.scraps += item.value / sellValueDivisor;

            onItemChangedCallback?.Invoke();
            return true;

        }

    }

}
