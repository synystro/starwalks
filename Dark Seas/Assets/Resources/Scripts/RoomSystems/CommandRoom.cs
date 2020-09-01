using UnityEngine;

namespace DarkSeas
{

    public class CommandRoom : Room
    {

        private Command command;

        public override void Start()
        {
            base.Start();

            // set room maxHP based on shield power consumption
            //command = shipManager.command.GetComponent<Command>();
            command = SystemGO.GetComponent<Command>();
            maxHP += command.powerConsumption;

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
            if ((currentHP <= maxHP) && (command.powerConsumption > 0))
            {

                // cap damage
                if (roomDamage > command.powerConsumption)
                    roomDamage = command.powerConsumption;

                // return power to the ship
                shipManager.reactor += roomDamage;

                // apply damage to shield
                command.powerConsumption -= roomDamage;

            }

            // refresh command's evasion
            command.RefreshEvasion();

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
