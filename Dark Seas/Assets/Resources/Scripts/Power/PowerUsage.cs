using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas {
    public class PowerUsage : MonoBehaviour {

        [SerializeField] private Weaponry weapon;
        [SerializeField] private ShipSystem system;
        [SerializeField] private Image cooldownProgress;
        [SerializeField] private GameObject cooldownBG_GO;
        public GameObject iconGO;
        [Space(10)]
        public int maxUsage;
        [Space(10)]
        public int usage;
        [Space(10)]
        public GameObject powerBarsParent;
        [Space(10)]
        public Sprite powerBarOn;
        public Sprite powerBarOff;

        public bool IsPowered { get; set; }

        private Color NORMAL_COLOR = new Color(1f, 1f, 1f, 1f);
        public Color DAMAGED_COLOR = new Color(1f, 0f, 0f, 1f);

        [SerializeField] private List<GameObject> powerBars = new List<GameObject>();

        void Awake() {

            AddPowerBars();

        }

        private void Start() {
            RefreshMaxPowerUsageUI();
        }

        private void Update() {

            if(weapon != null) {
                if(cooldownBG_GO.activeSelf == false)
                    cooldownBG_GO.SetActive(true);
                if(weapon.IsPowered) {
                    cooldownProgress.fillAmount = 1f - (weapon.CurrentCooldownTime / weapon.weapon.cooldown);
                } else {
                    cooldownProgress.fillAmount = 0f;
                }
            } else if (cooldownBG_GO != null) {
                if(cooldownBG_GO.activeSelf == true)
                    cooldownBG_GO.SetActive(false);
            }

        }

        private void AddPowerBars() {

            for (int i = 0; i < powerBarsParent.transform.childCount; i++) {
                powerBars.Add(powerBarsParent.transform.GetChild(i).gameObject);
                //Image powerBarImg = powerBars[i].GetComponent<Image>();
                //powerBarImg.DisableSpriteOptimizations();
            }

        }

        public void SetWeapon(Weaponry weapon) {
            this.weapon = weapon;
        }

        public void RefreshMaxPowerUsageUI() {

            for (int i = 0; i < powerBars.Count; i++) {
                if (i < maxUsage)
                    powerBars[i].SetActive(true);
                else
                    powerBars[i].SetActive(false);
            }

        }

        public void RefreshPowerUsageUI(ShipSystem shipSystem) {

            RefreshMaxPowerUsageUI();

            if (IsPowered)
                for (int i = 0; i < powerBars.Count; i++) {
                    Image powerBarImg = powerBars[i].GetComponent<Image>();
                    if (i < usage) {
                        powerBarImg.sprite = powerBarOn;
                        powerBarImg.color = NORMAL_COLOR;

                    } else {

                        powerBarImg.sprite = powerBarOff;

                        // had to use this due to a unity bug (in order to refresh the changed color)
                        powerBarImg.DisableSpriteOptimizations();

                        int systemHP;

                        if (shipSystem.Room == null)
                            systemHP = shipSystem.GetComponent<Weaponry>().hp;
                        else
                            systemHP = shipSystem.Room.CurrentHP;

                        if (i >= systemHP)
                            powerBarImg.color = DAMAGED_COLOR;
                        else
                            powerBarImg.color = NORMAL_COLOR;

                    }

                }
            else
                for (int i = 0; i < powerBars.Count; i++) {

                    Image powerBarImg = powerBars[i].GetComponent<Image>();

                    powerBarImg.sprite = powerBarOff;

                    // had to use this due to a unity bug (in order to refresh the changed color)
                    powerBarImg.DisableSpriteOptimizations();

                    int systemHP;

                    if (shipSystem.Room == null)
                        systemHP = shipSystem.GetComponent<Weaponry>().hp;
                    else
                        systemHP = shipSystem.Room.CurrentHP;

                    if (i >= systemHP) {
                        powerBarImg.color = DAMAGED_COLOR;
                    } else
                        powerBarImg.color = NORMAL_COLOR;

                }

        }

    }
}
