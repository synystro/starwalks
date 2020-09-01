using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    public class StoreUI : MonoBehaviour {

        [SerializeField] private StoreSlot selectedItemSlot;

        public Button nextButton;
        public Button previousButton;
        public Button exitButton;
        public GameObject storeItemValues;
        public GameObject storeItemAmounts;
        public TMPro.TextMeshProUGUI storePageText;

        public int PageIndex { get  {return storePageIndex; } }

        [SerializeField] private int storePageIndex; 
        private int maxStorePageIndex;

        [SerializeField] StoreSlot[] slots;
        [SerializeField] TMPro.TextMeshProUGUI[] storeItemValuesTxt;
        [SerializeField] TMPro.TextMeshProUGUI[] storeItemAmountsTxt;

        private void OnDisable() {
            // unselect item
            if(StoreInventory.i)
                StoreInventory.i.selectedItemSlot = null;
        }

        private void Start() {

            // assign item type index and max item type index
            storePageIndex = 0;
            maxStorePageIndex = StoreInventory.i.itemsForSale.Count - 1;

            // get slots from this script's GO children (even if inventory's GO is disabled).
            slots = GetComponentsInChildren<StoreSlot>(true);

            // get item value txt components
            storeItemValuesTxt = storeItemValues.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);

            // get item amounts txt components
            storeItemAmountsTxt = storeItemAmounts.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);

            // get inventory instance and add updateUI to its on item change callback.
            StoreInventory.i.onItemChangedCallback += UpdateUI;

            UpdateUI();

            nextButton.onClick.AddListener(NextPage);
            previousButton.onClick.AddListener(PreviousPage);
            exitButton.onClick.AddListener(ExitStore);

        }

        private void Update() {
            selectedItemSlot = StoreInventory.i.selectedItemSlot;

        }

        private void UpdateUI() {

            // set item type text
            string itemType = StoreInventory.i.itemsForSale[storePageIndex].type;
            storePageText.text = itemType;

            // check all slots for itemsForSale and remove all of them
            for (int i = 0; i < slots.Length; i++) {
                if (slots[i].item != null) {
                    CustomButton slotButton = slots[i].GetComponent<CustomButton>();
                    slotButton.onTap.RemoveAllListeners();
                    slots[i].RemoveItem();
                }
            }

            /// fill item slots according to the current selected type
            for (int i = 0; i < slots.Length; i++) {
                Item item = StoreInventory.i.itemsForSale[storePageIndex].items[i];
                if(item != null) {
                    slots[i].AddItem(item);
                    CustomButton slotButton = slots[i].GetComponent<CustomButton>();
                    slotButton.onTap.AddListener(()=> { StoreInventory.i.Buy(storePageIndex, item); });
                    storeItemValuesTxt[i].text = item.value.ToString();
                    storeItemAmountsTxt[i].text = item.amount.ToString();
                }
                else
                    Debug.LogError("Fucking item is missing on the " + StoreInventory.i.store.name + " store's items for sale index " + i);
            }

        }

        private void NextPage() {

            // go to next item type
            if(storePageIndex < maxStorePageIndex)
                storePageIndex++;
            else
                storePageIndex = 0;

            StoreInventory.i.selectedItemSlot = null;
            StoreInventory.i.onItemChangedCallback?.Invoke();

        }

        private void PreviousPage() {
            
            // go to previous page
            if(storePageIndex > 0)
                storePageIndex--;
            else
                storePageIndex = maxStorePageIndex;

            StoreInventory.i.selectedItemSlot = null;
            StoreInventory.i.onItemChangedCallback?.Invoke();

        }

        private void ExitStore() {

            this.transform.parent.gameObject.SetActive(false);
            GameManager.InShop = false;
            GameManager.LocationManager.ExitLocation();

        }

    }
}
