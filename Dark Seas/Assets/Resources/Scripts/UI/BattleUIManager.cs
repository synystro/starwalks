using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {

    public class BattleUIManager : MonoBehaviour
    {
        //public delegate void OnUiChanged();
        //public event OnUiChanged onUiChangedCallback;

        [Header("Values")]
        public GameObject armorValue;
        [Space(10)]
        public GameObject evasionValue;
        [Space(10)]
        public GameObject oxygenValue;
        [Space(10)]
        public GameObject powerValue;
        [Space(10)]
        public GameObject engineValue;
        [Space(20)]
        public GameObject submarineButton;
        [Space(20)]
        public GameObject aimIconPrefab;
        [Space(10)]
        public GameObject firePrefab;
        [Space(10)]
        public GameObject breachPrefab;
        [Space(10)]
        public GameObject weapons;
        [Space(10)]
        public GameObject systemPanel;
        [Space(10)]
        public GameObject crewPanel;
        [Space(10)]
        public SystemPowerUI systemPowerUI;
        [Space(10)]
        public List<GameObject> systemPanels = new List<GameObject>();
        [Space(10)]
        //[SerializeField] private Transform crewAttributePanels;
        //[SerializeField] private GameObject crewmemberAttributePanelPrefab;
        //public List<GameObject> crewAttributesPanel = new List<GameObject>();
        [Space(10)]
        #region Canvas Group
        public GameObject battleCanvas;
        [Space(10)]
        public GameObject roomIconCanvas;
        [Space(10)]
        public GameObject crewmemberCanvas; // health and progress bars
        #endregion
        [Space(20)]
        public Color roomIconColor;
        [Space(10)]
        public Color damagedColor;

        // [Header("Bars")]
        // public Image hullBar;
        // [Space(10)]
        // public Image shieldBar;

        private Color NORMAL_COLOR = new Color(1f, 1f, 1f, 0.5f);
        private Color BAR_BG_COLOR = new Color(0.30f, 0.33f, 0.33f, 0.5f);

        private Ship playerShip;
        private Vector3 targetPosition;
        private TMPro.TextMeshProUGUI armorValueTxt;
        private TMPro.TextMeshProUGUI evasionValueTxt;
        private TMPro.TextMeshProUGUI powerValueTxt;

        void Setup() {

            GameManager.firstBattleStart = GameManager.GetNetTime();

            Debug.Log("Seconds since game started until first battle : " + GameManager.TimeDifference(GameManager.gameStart, GameManager.firstBattleStart).Seconds * -1);

            // subscribe to onplayerdefeat callback
            GameManager.onPlayerDefeatCallback += DisplayDefeatCanvas;

            // get player's ship
            if(playerShip == null)
                playerShip = GameManager.ship;

            // get armor and set it to UI
            RefreshArmor();

            // get evasion and set it to UI
            RefreshEvasion();

            // add listener to submarine button to exit location
            submarineButton.GetComponent<Button>().onClick.AddListener(delegate () { GameManager.LocationManager.ExitLocation(); });
            
            // refresh hull and shield integrity
            RefreshHullIntegrity();            

            // display battle canvas
            DisplayBattleCanvas();

        }

        // public void SetupCrew() {

        //     // clear previous crewattpanels
        //     foreach(Transform child in crewAttributePanels)
        //         Destroy(child.gameObject);

        //     int currentButtonIndex = 0;

        //     for (int i = 0; i < playerShip.crew.transform.childCount; i++) {

        //         GameObject crewmemberGO;
        //         crewmemberGO = playerShip.crew.transform.GetChild(i).gameObject;

        //         // if the crewmember's gameobject is inactive for some reason, skip to next
        //         if(crewmemberGO.activeSelf == false) {
        //             continue;
        //         }
                
        //         Crewmember crewmember = crewmemberGO.GetComponent<Crewmember>();

        //         print(crewmember.name);

        //         GameObject crewmemberButtonGO = crewPanel.transform.GetChild(currentButtonIndex).gameObject;
        //         // add to crew button index
        //         currentButtonIndex++;

        //         Image crewmemberIcon = crewmemberButtonGO.transform.GetChild(0).GetComponentInChildren<Image>(true);
        //         // enable crew icon
        //         crewmemberIcon.enabled = true;
        //         // set crew icon
        //         crewmemberIcon.sprite = crewmember.icon;
        //         print(crewmember.icon + " vs " + crewmemberIcon.sprite);
        //         crewmemberIcon.color = crewmember.GetComponent<SpriteRenderer>().color;

        //         RectTransform crewAttPanelRectT = crewmemberButtonGO.transform.Find("CrewAttributesPanel").GetComponent<RectTransform>();
        //         AttributesUIPanel attUI = crewAttPanelRectT.GetComponent<AttributesUIPanel>();
        //         attUI.SetCrewmember(crewmember);

        //         //crewAttPanelRectT.SetParent(crewAttributePanels.transform, true);

        //         // set double click
        //         CustomButton crewmemberButton = crewmemberButtonGO.GetComponent<CustomButton>();
        //         crewmemberButton.onDoubleTap.AddListener(
        //             () =>  crewAttPanelRectT.gameObject.SetActive(!crewAttPanelRectT.gameObject.activeSelf)
        //         );

        //         crewmemberButton.onDoubleTap?.Invoke();

        //     }

        //     AddListenerToShipCrew();

        // }

        public void SetupSystems() {

            for (int i = 0; i < playerShip.systems.transform.childCount; i++) {

                ShipSystem shipSystem = GameManager.shipSystems[i].GetComponent<ShipSystem>();
                // set system power usage to UI
                GameObject systemButton = systemPanel.transform.GetChild(i).gameObject;
                PowerUsage systemPowerUsage = systemButton.GetComponent<PowerUsage>();
                systemPowerUsage.maxUsage = shipSystem.powerCap;
                systemPowerUsage.RefreshMaxPowerUsageUI();
                Image systemIcon = systemButton.transform.GetChild(0).GetComponentInChildren<Image>();
                // enable system icon
                systemIcon.enabled = true;
                // set system icon
                systemIcon.sprite = shipSystem.system.icon;

            }

            AddListenerToShipSystems();
            RefreshSystemsPowerUsage();

        }

        public void SetupWeapons() {

            for (int i = 0; i < GameManager.shipWeapons.Count; i++) {

                GameObject weapon = weapons.transform.GetChild(i).gameObject;
                Weaponry shipWeapon = GameManager.shipWeapons[i].GetComponent<Weaponry>();

                // set weapon power usage to UI
                PowerUsage weaponPowerUsage = weapon.GetComponent<PowerUsage>();
                weaponPowerUsage.SetWeapon(shipWeapon);
                weaponPowerUsage.maxUsage = shipWeapon.powerCap;
                weaponPowerUsage.RefreshMaxPowerUsageUI();

                Image weaponIcon = weapon.transform.GetChild(0).GetComponentInChildren<Image>();
                // enable weapon icon
                weaponIcon.enabled = true;
                // set weapon icon
                weaponIcon.sprite = GameManager.ship.WeaponsRoom.weaponsEquipped[i].icon;
            }

            AddListenerToShipWeapons();

            RefreshWeaponsPowerUsage();

        }

        // public void AddListenerToShipCrew() {

        //     int nOfButtons = 0;

        //     for (int i = 0; i < playerShip.crew.transform.childCount; i++) {

        //         GameObject crewmemberGO = playerShip.crew.transform.GetChild(i).gameObject;

        //         // if the crewmember's gameobject is inactive for some reason, skip to next
        //         if(crewmemberGO.activeSelf == false) {
        //             continue;
        //         }

        //         GameObject crewButton = crewPanel.transform.GetChild(nOfButtons).gameObject;
        //         Crewmember crewmember = crewmemberGO.GetComponent<Crewmember>();

        //         UnityEngine.UI.Toggle crewToggle = crewButton.GetComponent<UnityEngine.UI.Toggle>();
        //         ToggleColor toggleColor = crewButton.GetComponent<ToggleColor>();
        //         // assign toggle to the crewmember's script
        //         crewmember.toggle = crewToggle;
        //         // remove all previous functionalities from crew toggle
        //         crewToggle.onValueChanged.RemoveAllListeners();
        //         // add functionality to crew toggle
        //         crewToggle.onValueChanged.AddListener(crewmember.ToggleSelection);
        //         crewToggle.onValueChanged.AddListener(toggleColor.OnToggleValueChanged);

        //         nOfButtons++;
        //     }

        // }

        public void AddListenerToShipWeapons() {

            for (int i = 0; i < GameManager.shipWeapons.Count; i++) {

                GameObject weapon = weapons.transform.GetChild(i).gameObject;
                Weaponry shipWeapon = GameManager.shipWeapons[i].GetComponent<Weaponry>();

                CustomButton weaponButton = weapon.GetComponent<CustomButton>();
                // remove all previous functionalities from weapon button
                weaponButton.onTap.RemoveAllListeners();
                weaponButton.onDoubleTap.RemoveAllListeners();
                weaponButton.onLongTap.RemoveAllListeners();
                // add functionality to weapon button
                weaponButton.onTap.AddListener(delegate () { shipWeapon.ToggleSelectWeapon(); });
                weaponButton.onDoubleTap.AddListener(delegate () { shipWeapon.AddFullPower(); });
                weaponButton.onLongTap.AddListener(delegate () { shipWeapon.RemoveAllPower(); });

            }

        }

        public void AddListenerToShipSystems() {

            for (int i = 0; i < GameManager.shipSystems.Count; i++) {

                ShipSystem shipSystem = GameManager.shipSystems[i].GetComponent<ShipSystem>();

                GameObject systemButtonGO = systemPanel.transform.GetChild(i).gameObject;

                CustomButton systemButton = systemButtonGO.GetComponent<CustomButton>();
                // remove all previous functionalities from weapon button
                systemButton.onTap.RemoveAllListeners();
                systemButton.onDoubleTap.RemoveAllListeners();
                systemButton.onLongTap.RemoveAllListeners();

                // add functionality to system's button (except for weapons system)
                if (!shipSystem.GetComponent<Weapons>()) {
                    systemButton.onTap.AddListener(delegate () {
                        shipSystem.ToggleSelectSystem();
                        });
                    // systemButton.onDoubleTap.AddListener(delegate () {
                    //     shipSystem.AddFullPower();
                    //     });
                    systemButton.onDoubleTap.AddListener(delegate () {
                        shipSystem.OpenSystemPowerUI();
                    });
                }

            }

        }

        public void RefreshSystemsPowerUsage() {

            for (int i = 0; i < GameManager.shipSystems.Count; i++) {

                int childCount = systemPanel.transform.childCount;

                ShipSystem shipSystem = GameManager.shipSystems[i].GetComponent<ShipSystem>();

                GameObject systemButton = systemPanel.transform.GetChild(i).gameObject;

                PowerUsage systemPowerUsage = systemButton.GetComponent<PowerUsage>();

                //if system is damaged, turn icon red
                if (shipSystem.IsDamaged)
                    systemPowerUsage.iconGO.GetComponent<Image>().color = systemButton.GetComponent<Image>().color = damagedColor;
                else
                    systemPowerUsage.iconGO.GetComponent<Image>().color = systemButton.GetComponent<Image>().color = NORMAL_COLOR;

                if (systemPowerUsage != null) {
                    systemPowerUsage.usage = shipSystem.powerConsumption;
                    systemPowerUsage.IsPowered = shipSystem.IsPowered;
                    systemPowerUsage.RefreshPowerUsageUI(shipSystem);
                } else {
                    Debug.LogError(shipSystem.name + " is missing the power usage component");
                }

            }

            // refresh power value
            powerValueTxt = powerValue.GetComponent<TMPro.TextMeshProUGUI>();
            powerValueTxt.text = playerShip.reactor.ToString();

        }

        public void RefreshWeaponsPowerUsage() {

            for (int i = 0; i < GameManager.shipWeapons.Count; i++) {

                GameObject weapon = weapons.transform.GetChild(i).gameObject;
                Weaponry shipWeapon = GameManager.shipWeapons[i].GetComponent<Weaponry>();
                PowerUsage weaponPowerUsage = weapon.GetComponent<PowerUsage>();

                // if weapon is damaged, turn icon red
                if (shipWeapon.IsDamaged)
                    weaponPowerUsage.iconGO.GetComponent<Image>().color = weapon.GetComponent<Image>().color = damagedColor;
                else
                    weaponPowerUsage.iconGO.GetComponent<Image>().color = weapon.GetComponent<Image>().color = NORMAL_COLOR;

                weaponPowerUsage.maxUsage = GameManager.ship.WeaponsRoom.weaponsEquipped[i].powerConsumption;
                weaponPowerUsage.usage = shipWeapon.powerConsumption;
                weaponPowerUsage.IsPowered = shipWeapon.IsPowered;
                weaponPowerUsage.RefreshPowerUsageUI(shipWeapon);
            }

            // refresh power value
            powerValueTxt = powerValue.GetComponent<TMPro.TextMeshProUGUI>();
            powerValueTxt.text = playerShip.reactor.ToString();

        }

        public void RefreshPowerUsage() {

            RefreshSystemsPowerUsage();

            RefreshWeaponsPowerUsage();

            // refresh power value
            powerValueTxt = powerValue.GetComponent<TMPro.TextMeshProUGUI>();
            powerValueTxt.text = playerShip.reactor.ToString();

        }

        public void RefreshArmor() {

            if (playerShip != null) {
                playerShip.RefreshShipArmor();

                armorValueTxt = armorValue.GetComponent<TMPro.TextMeshProUGUI>();
                armorValueTxt.text = playerShip.armor.ToString();
            }

        }

        public void RefreshEvasion() {

            if (playerShip != null) {
                playerShip.RefreshShipEvasion();

                evasionValueTxt = evasionValue.GetComponent<TMPro.TextMeshProUGUI>();
                evasionValueTxt.text = Mathf.RoundToInt(playerShip.evasion).ToString();
            }

        }

        public void RefreshHullIntegrity() {
            //control health bar
            if (playerShip == null)
                return;

            GameManager.TopBarManager.HullBar.fillAmount = playerShip.hull / playerShip.maxHull;
        }

        public void RefreshShieldlIntegrity() {

            //control shield bar
            if (playerShip == null)
                return;

            Shield playerShield = playerShip.shield.GetComponent<Shield>();

            GameManager.TopBarManager.ShieldBar.fillAmount = playerShield.integrity / playerShield.capacity;

            // get shield bar bg image component
            Image shieldBarBG = GameManager.TopBarManager.ShieldBar.transform.parent.GetComponent<Image>();

            if (playerShield.Room.CurrentHP <= 0)
                shieldBarBG.color = damagedColor;
            else
                shieldBarBG.color = BAR_BG_COLOR;

        }

        public void Refresh() { Setup(); }

        public void HideBattleCanvas() { battleCanvas.SetActive(false); if (!GameManager.InBattle) roomIconCanvas.SetActive(false); }

        public void DisplayBattleCanvas() { battleCanvas.SetActive(true); battleCanvas.GetComponent<Canvas>().sortingOrder = 2; if (!roomIconCanvas.activeSelf) roomIconCanvas.SetActive(true); }

        //public void HideInventoryCanvas() { inventoryCanvas.SetActive(false); }

        //public void DisplayInventoryCanvas() { inventoryCanvas.SetActive(true); } 

        public void DisplayDefeatCanvas() { print("defeat screen"); GameManager.onPlayerDefeatCallback -= DisplayDefeatCanvas; }
    
    }

}
