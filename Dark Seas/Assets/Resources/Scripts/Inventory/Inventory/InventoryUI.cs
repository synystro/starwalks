using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DarkSeas {

    public class InventoryUI : MonoBehaviour {

        private Inventory inventory;

        [SerializeField] private Transform inventoryPanel;
        public Transform slotContainer;
        public Transform slotPrefab;

        private List<GameObject> slotList;

        public int rowCap;
        public bool horizontal;

        private int nRows;
        private float slotCellSizeX;
        private float slotCellSizeY;
        private RectTransform slotContainerRectT;
        private float panelWidth;
        private float panelHeight;        

        private void Start() {
            slotContainerRectT = slotContainer.GetComponent<RectTransform>();
            RectTransform slotRectT = slotPrefab.GetComponent<RectTransform>();
            slotCellSizeX = slotRectT.sizeDelta.x;
            slotCellSizeY = slotRectT.sizeDelta.y;
        }

        public void SetInventory(Inventory inventory) {
            this.inventory = inventory;
            inventory.onItemChangedCallback += UpdateUI;
            UpdateUI();
        }

        public void ToggleDisplayInventory() {
            if(inventoryPanel == null)
                Debug.LogError("Inventory panel not set for the inventory UI!");
            else {
                inventoryPanel.gameObject.SetActive(!inventoryPanel.gameObject.activeSelf);
            }
        }

        private void UpdateUI() {

            foreach(Transform child in slotContainer)
                if(child == slotPrefab)
                    continue;
                else
                    Destroy(child.gameObject);

            int x = 0;
            int y = 0;
            nRows = 0;
            
            foreach(Item item in inventory.GetItemList()) {
                RectTransform slotRectTransform = Instantiate(slotPrefab, slotContainer).GetComponent<RectTransform>();
                slotRectTransform.gameObject.SetActive(true);
                slotRectTransform.anchoredPosition = new Vector2(
                    x * slotCellSizeX,
                    y * slotCellSizeY
                    );
                GameObject itemSlot = slotRectTransform.GetChild(0).gameObject;
                Image itemIcon = itemSlot.GetComponent<Image>();
                itemIcon.sprite = item.icon;

                #region button

                CustomButton itemButton = itemSlot.GetComponent<CustomButton>();
                itemButton.onTap.RemoveAllListeners();
                itemButton.onDoubleTap.RemoveAllListeners();

                itemButton.onTap.AddListener(() => DisplayItemInfo(item));
                itemButton.onDoubleTap.AddListener( () => EquipItem(item));

                #endregion

                x++;
                if (x >= rowCap && rowCap != 0) {
                    x = 0;
                    y--;
                    nRows = y * -1;
                } else {
                    nRows++;
                }
            }            

            AdjustPanelSize();    

        }

        private void DisplayItemInfo(Item item) {
            print("info of " + item.name);
        }

        private bool EquipItem(Item item) {

            bool equipped = false;

            switch (item.type) {
                case Item.Type.Weapon:
                    equipped = EquipmentManager.EquipWeapon(item.weapon);
                    break;
            }

            //switch itemtype?

            if(equipped) {
                inventory.Remove(item);
                print("ue");
            } else {
                print("uelse");
            }

            return equipped;

        }

        private void AdjustPanelSize() {

            if(horizontal) {
                panelWidth = slotCellSizeX * nRows;
                panelHeight = slotCellSizeY;
            }
            else {
                panelWidth = slotCellSizeX;
                panelHeight = slotCellSizeY * nRows;
            }

            slotContainerRectT.sizeDelta = new Vector2(panelWidth, panelHeight);

        }

    }
}
