using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {
    
    public class StoreSlot : MonoBehaviour {

        public Item item;
        //public int index;

        public bool IsTaken { get { return item != null; } }

        private Image slotBorderImg;
        private Image itemImg;
        private Button button;

        private StoreUI storeUI;

        private void Awake() {

            storeUI = GetComponentInParent<StoreUI>();

            slotBorderImg = this.GetComponent<Image>();

            itemImg = this.transform.GetChild(0).GetComponent<Image>();
        }

        private void SelectItem() {

            StoreInventory.i.selectedItemSlot = this;

            StoreInventory.i.Buy(storeUI.PageIndex, item);

        }

        public void Unselect() {

            StoreInventory.i.selectedItemSlot = null;
            
        }

        public void AddItem(Item _item) {
            // skip if there's an item already.
            if (IsTaken)
                return;

            // add item data.
            item = _item;

            // set itemImg sprite and visibility.
            itemImg.sprite = item.icon;
            itemImg.enabled = true;

        }

        public void RemoveItem() {
            // erase item data.
            itemImg.enabled = false;
            itemImg.sprite = null;
            item = null;
        }

    }
}
