using UnityEngine;

namespace DarkSeas
{
    public class EngineRoom : Room
    {

        private Engine engine;

        public override void Start()
        {

            base.Start();

            // set room maxHP based on engine power consumption
            //engine = shipManager.engine.GetComponent<Engine>();
            engine = SystemGO.GetComponent<Engine>();
            maxHP += engine.powerConsumption;

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

            // if damage was taken and the engine is powered
            if ((currentHP <= maxHP) && (engine.powerConsumption > 0))
            {

                // cap damage
                if (roomDamage > engine.powerConsumption)
                    roomDamage = engine.powerConsumption;

                // return power to the ship
                shipManager.reactor += roomDamage;

                // apply damage to shield
                engine.powerConsumption -= roomDamage;

            }

            // refresh engine's evasion
            engine.RefreshEvasion();

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
