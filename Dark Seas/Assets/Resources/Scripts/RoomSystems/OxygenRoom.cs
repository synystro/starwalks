using UnityEngine;

namespace DarkSeas {
    public class OxygenRoom : Room {

        private Oxygen oxygen;

        public override void Start() {

            base.Start();

            // set room maxHP based on oxygen systems power consumption
            //oxygen = shipManager.oxygen.GetComponent<Oxygen>();
            oxygen = SystemGO.GetComponent<Oxygen>();
            maxHP += oxygen.powerConsumption;

            currentHP = maxHP;
        }

        public override void Update()
        {
            base.Update();
        }

        // receive damage from one of this room's tile
        public override void TakeDamage(float damage) {

            base.TakeDamage(damage);

            int roomDamage = Mathf.RoundToInt(damage / 10);

            // if damage was taken and the shield is powered
            if ((currentHP <= maxHP) && (oxygen.powerConsumption > 0)) {

                // cap damage
                if (roomDamage > oxygen.powerConsumption)
                    roomDamage = oxygen.powerConsumption;

                // return power to the ship
                shipManager.reactor += roomDamage;

                // apply damage to shield
                oxygen.powerConsumption -= roomDamage;

            }

            // refresh power usage
            GameManager.BattleUiManager.RefreshPowerUsage();

        }

        // repair system with a crew member or robot
        public override void RepairOneBar() {
            base.RepairOneBar();
        }
    }
}
