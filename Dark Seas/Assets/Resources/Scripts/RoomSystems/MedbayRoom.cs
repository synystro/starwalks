using UnityEngine;

namespace DarkSeas
{
    public class MedbayRoom : Room
    {
        private Medbay medbay;

        public override void Start()
        {

            base.Start();

            // get medBay component from this game object
            medbay = shipManager.medbay.GetComponent<Medbay>();


            // set room maxHP based on medBay power consumption
            maxHP += medbay.powerConsumption;

            currentHP = maxHP;

        }

        public override void Update()
        {
            base.Update();

            if(medbay.IsPowered)
                HealCrewmembers();

        }

        // receive damage from one of this room's tile
        public override void TakeDamage(float damage)
        {

            base.TakeDamage(damage);

            int roomDamage = Mathf.RoundToInt(damage / 10);

            // if damage was taken and the medBay is powered
            if ((currentHP <= maxHP) && (medbay.powerConsumption > 0))
            {

                // cap damage
                if (roomDamage > medbay.powerConsumption)
                    roomDamage = medbay.powerConsumption;

                // return power to the ship
                shipManager.reactor += roomDamage;

                // apply damage to shield
                medbay.powerConsumption -= roomDamage;

            }

            // refresh power usage
            GameManager.BattleUiManager.RefreshPowerUsage();

        }

        // repair system with a crew member or robot
        public override void RepairOneBar()
        {
            base.RepairOneBar();
        }

        private void HealCrewmembers() {

            for (int i = 0; i < crewmembers.Count; i++) {

                Crewmember crewmember = crewmembers[i].GetComponent<Crewmember>();

                if (!crewmember.IsMachine)
                    crewmember.Heal(medbay.powerConsumption * medbay.healRatio * Time.deltaTime);

            }

        }

    }
}
