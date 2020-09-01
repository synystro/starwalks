using UnityEngine;

namespace DarkSeas {
    public class ShipSystem : MonoBehaviour {
        public Crewmember handler;
        [Space(10)]
        public RoomSystem system;
        [Space(10)]
        public int powerConsumption;
        [Space(10)]
        public int powerUpgrade;
        [Space(10)]
        public int powerCap;
        [Space(10)]
        public int crewmemberSkillLevel;
        public int skillInput = 0;
        public bool IsDamaged { get { return Room.IsDamaged; } }
        [Space(10)]
        public Sprite icon;
        [Space(10)]
        protected Ship ship;

        public bool IsPowered { get { return powerConsumption > 0 ? true : false; } }

        public Room Room { get; set; }
        protected BattleUIManager battleUiManager;

        private void Awake() {

            // set power cap and system's icon
            if (system != null) {
                powerCap = system.basePowerCap + powerUpgrade;
                icon = system.icon;
            }

        }

        public virtual void Start() {
            // set power cap and system's icon
            if (system != null && system.name != "Weapons") {
                powerCap = system.basePowerCap + powerUpgrade;
                icon = system.icon;
                Room.MaxHP = powerCap;
                
            }

            // get ship
            ship = GetComponentInParent<Ship>();

            // get battle ui manager
            battleUiManager = GameManager.BattleUiManager;

        }

        public virtual void Update() {

        }

        public virtual void RefreshToUI() {

        }

        public virtual void ToggleSelectSystem() {


        }

        public void AddFullPower() {

            int barsUntilFullPower;
            int maxConsumption;

            if(powerCap < Room.CurrentHP) {
                maxConsumption = powerCap;
                barsUntilFullPower = powerCap - powerConsumption;
            }
            else {
                maxConsumption = Room.CurrentHP;
                barsUntilFullPower = Room.CurrentHP - powerConsumption;
            }

            if(powerConsumption < maxConsumption) {                
                if(ship.reactor > barsUntilFullPower) {
                    powerConsumption += barsUntilFullPower;
                    ship.reactor -= barsUntilFullPower;
                } else {
                    Debug.Log("your ship has not enough power to max this system's power");
                }
            }

            battleUiManager.RefreshPowerUsage();
        }

        public void RemoveAllPower() {
            if(powerConsumption > 0) {
                ship.reactor += powerConsumption;
                powerConsumption -= powerConsumption;
            }
            battleUiManager.RefreshPowerUsage();
        }

        public void AddOnePowerBar() {
            if (powerConsumption < Room.CurrentHP)
                if(ship.reactor > 0) {
                ship.reactor -= 1;
                powerConsumption += 1;
                } else {
                    Debug.Log("your ship has not enough power");
                }
            
            battleUiManager.RefreshSystemsPowerUsage();
            RefreshToUI();
        }

        public void RemoveOnePowerBar() {
            if(powerConsumption > 0) {
                ship.reactor += 1;
                powerConsumption -= 1;
            }

            battleUiManager.RefreshSystemsPowerUsage();
            RefreshToUI();
        }

        public void OpenSystemPowerUI() {

            SystemPowerUI systemPowerUI = battleUiManager.systemPowerUI;
            systemPowerUI.gameObject.SetActive(true);
            systemPowerUI.system = this;
            //systemPowerUI.on

        }

        // public void TogglePower() {

        //     if (powerConsumption < Room.CurrentHP && ship.reactor > 0) {
        //         ship.reactor -= 1;
        //         powerConsumption += 1;
        //     } else {
        //         ship.reactor += Room.CurrentHP;
        //         powerConsumption = 0;
        //     }

        //     // set weapon power usage to UI

        //     if (ship.isPlayer) {

        //         // refresh power usage
        //         battleUiManager.RefreshPowerUsage();
        //         // refresh to UI
        //         RefreshToUI();

        //     }

        // }

    }
}
