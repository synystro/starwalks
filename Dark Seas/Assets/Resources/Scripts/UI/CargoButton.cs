using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    public class CargoButton : MonoBehaviour {

        private Button button;

        void Start() {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => GameManager.InventoryUI.ToggleDisplayInventory());
    
    }
    
    }

}
