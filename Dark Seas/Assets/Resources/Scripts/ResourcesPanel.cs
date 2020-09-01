using UnityEngine;

namespace DarkSeas {
    public class ResourcesPanel : MonoBehaviour {
        public GameObject fuelTxt;
        public GameObject ammoTxt;
        public GameObject scrapsTxt;

        private TMPro.TextMeshProUGUI fuel;
        private TMPro.TextMeshProUGUI ammo;
        private TMPro.TextMeshProUGUI scraps;

        private Ship playerShip;

        private void Start() {
            playerShip = GameManager.ship;
            fuel = fuelTxt.GetComponent<TMPro.TextMeshProUGUI>();
            ammo = ammoTxt.GetComponent<TMPro.TextMeshProUGUI>();
            scraps = scrapsTxt.GetComponent<TMPro.TextMeshProUGUI>();
        }

        void Update() {
            fuel.text = playerShip.fuel.ToString("F0");
            ammo.text = playerShip.ammo.ToString();
            scraps.text = playerShip.scraps.ToString();

        }

    }
}
