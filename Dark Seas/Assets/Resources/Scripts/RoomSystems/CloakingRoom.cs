using UnityEngine;

namespace DarkSeas {

    public class CloakingRoom : Room {

        private Cloaking cloaking;

        public override void Start() {

            base.Start();

            // set room maxHP based on shield power consumption
            //cloaking = shipManager.cloaking.GetComponent<Cloaking>();
            cloaking = SystemGO.GetComponent<Cloaking>();
            maxHP += cloaking.powerConsumption;

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
            if ((currentHP <= maxHP) && (cloaking.powerConsumption > 0)) {

                // cap damage
                if (roomDamage > cloaking.powerConsumption)
                    roomDamage = cloaking.powerConsumption;

                // return power to the ship
                shipManager.reactor += roomDamage;

                // apply damage to shield
                cloaking.powerConsumption -= roomDamage;

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
