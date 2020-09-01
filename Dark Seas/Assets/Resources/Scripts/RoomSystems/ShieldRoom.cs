using UnityEngine;

namespace DarkSeas
{
    public class ShieldRoom : Room
    {

        private Shield shield;

        public override void Start()
        {

            base.Start();

            // set room maxHP based on shield power consumption
            //shield = shipManager.shield.GetComponent<Shield>();
            shield = SystemGO.GetComponent<Shield>();

            currentHP = maxHP;

        }

        public override void Update()
        {
            base.Update();
        }

        // receive damage from one of this room's tile
        public override void TakeDamage(float damage)
        {

            base.TakeDamage(damage);

            int roomDamage = Mathf.RoundToInt(damage / 10);

            // if damage was taken and the shield is powered
            if ((currentHP <= maxHP) && (shield.powerConsumption > 0))
            {
                // cap damage
                if (roomDamage > shield.powerConsumption)
                    roomDamage = shield.powerConsumption;

                // return power to the ship
                shipManager.reactor += roomDamage;

                // apply damage to shield
                shield.powerConsumption -= roomDamage;

            }

            // refresh power usage
            GameManager.BattleUiManager.RefreshPowerUsage();

        }

        // repair system with a crew member or robot
        public override void RepairOneBar()
        {
            base.RepairOneBar();
        }
    }
}
