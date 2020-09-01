using UnityEngine;
using UnityEngine.UI;

namespace DarkSeas
{
    public class Engine : ShipSystem
    {

        [SerializeField] private const float EVASION_RATIO = 2.5f;
        public float charge;
        [Space(10)]
        public float baseRechargeRate;
        [Space(10)]

        private const float MAX_CHARGE = 100f;

        private Button submarineButton;
        private TMPro.TextMeshProUGUI engineValue;

        private bool hasStarted;

        private void OnEnable() {

            if(!hasStarted)
                return;

            Start();

        }

        private void AutoPower() {

            // set integrity
            if (!IsPowered && (ship.reactor >= 1) && !Room.IsDamaged) {
                powerConsumption += 1;
                ship.reactor -= 1;
                charge = MAX_CHARGE;
            }

        }

        public override void Start()
        {
            hasStarted = true;

            base.Start();

            AutoPower();

            if (ship == GameManager.ship) {
                // assign submarineButton and set it to start at false
                submarineButton = battleUiManager.submarineButton.GetComponent<Button>();
                submarineButton.interactable = false;
                // get and assign engine value to battle UI
                engineValue = battleUiManager.engineValue.GetComponent<TMPro.TextMeshProUGUI>();
                engineValue.text = charge.ToString("F1");
            }

            // set engine charge to start at zero
            charge = 0f;

            // set ship's starting engine evasion
            //RefreshEvasion();

        }

        public override void Update() {

            if (IsPowered) {
                // if not in battle, the sub can immediatly jump
                if (!GameManager.InBattle) {
                    charge = MAX_CHARGE;
                    // enable submarine (engage) button
                    if(submarineButton != null)
                        submarineButton.interactable = true;
                } else {
                    // recharge based on dedicated power and crewmember skill level
                    float finalRechargeRate = (baseRechargeRate + ship.shipEffects.EngineRR) * (1 + ship.shipEffects.EngineRR_P);
                    charge += finalRechargeRate * (crewmemberSkillLevel + powerConsumption) * Time.deltaTime;
                }

                if (charge >= MAX_CHARGE) {
                    charge = MAX_CHARGE;
                    // if fully charged, enable sub button
                    if (submarineButton != null)
                        submarineButton.interactable = true;
                } else {
                    // if not fully charged, disable sub button
                    if (submarineButton != null)
                        submarineButton.interactable = false;
                }
            } else {
                // reset handler skill level
                crewmemberSkillLevel = 0;
                // disable submarine (engage) button
                if (submarineButton != null)
                    submarineButton.interactable = false;
            }

            if (ship == GameManager.ship)
                if (charge >= MAX_CHARGE || charge <= 0f)
                    engineValue.text = charge.ToString("F0");
                else
                    engineValue.text = charge.ToString("F1");


        }

        public override void RefreshToUI() {

            RefreshEvasion();

        }

        public void RefreshEvasion()
        {

            if (handler != null && IsPowered)
                crewmemberSkillLevel = handler.crewmemberStats.engineerLevel;
            else
                crewmemberSkillLevel = skillInput;

            ship.engineEV = EVASION_RATIO * (powerConsumption + crewmemberSkillLevel);

            battleUiManager.RefreshEvasion();

        }

    }

}
