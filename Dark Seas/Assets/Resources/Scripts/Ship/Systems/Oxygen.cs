using UnityEngine;

namespace DarkSeas {
    public class Oxygen : ShipSystem {

        public float amount;
        [Space(10)]
        public float baseRechargeRate;
        [SerializeField] float depleteRate = 0.4f;

        [SerializeField] private TMPro.TextMeshProUGUI oxygenValue;

        private void AutoPower() {

            if (IsPowered)
                amount = 100f;
            else if (!Room.IsDamaged) {
                for (int i = 0; i < ship.reactor; i++) {
                    if (powerConsumption < powerCap) {
                        powerConsumption += 1;
                        ship.reactor -= 1;
                    }
                }
                amount = 100f;
            }

        }

        public override void Start() {

            base.Start();

            AutoPower();

            // set oxygen amount (in the ship) to start at 100%
            amount = 100f;

            if (ship == GameManager.ship) {
                // get and assign oxygen value to battle UI
                oxygenValue = battleUiManager.oxygenValue.GetComponent<TMPro.TextMeshProUGUI>();
                oxygenValue.text = amount.ToString("F1");
            }

        }

        public override void Update() {

            if (IsPowered && !ship.IsBreached) {

                if(ship == GameManager.ship)
                    oxygenValue.color = Color.white;

                // recharge based on dedicated power and crewmember skill level
                amount += baseRechargeRate * (crewmemberSkillLevel + powerConsumption) * Time.deltaTime;

                if (amount >= 100f)
                    amount = 100f;

            } else {

                if(ship == GameManager.ship)
                    oxygenValue.color = Color.yellow;

                amount -= depleteRate * Time.deltaTime;

                if (amount <= 0f)
                    amount = 0f;

            }

            if (ship == GameManager.ship)
                if (amount >= 100f || amount <= 0f)
                    oxygenValue.text = amount.ToString("F0");
                else
                    oxygenValue.text = amount.ToString("F0");

        }

    }

}
