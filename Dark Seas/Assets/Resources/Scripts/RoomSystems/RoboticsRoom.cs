using UnityEngine;

namespace DarkSeas {

    public class RoboticsRoom : Room {

        private Robotics robotics;

        public override void Start() {

            base.Start();

            // get robotics component from this game object
            //robotics = shipManager.robotics.GetComponent<Robotics>();
            robotics = SystemGO.GetComponent<Robotics>();


            // set room maxHP based on robotics power consumption
            maxHP += robotics.powerConsumption;

            currentHP = maxHP;

        }

        public override void Update()
        {
            base.Update();

            if(robotics.IsPowered)
                RepairRobots();

        }

        // receive damage from one of this room's tile
        public override void TakeDamage(float damage) {

            base.TakeDamage(damage);

            int roomDamage = Mathf.RoundToInt(damage / 10);

            // if damage was taken and the robotics is powered
            if ((currentHP <= maxHP) && (robotics.powerConsumption > 0)) {

                // cap damage
                if (roomDamage > robotics.powerConsumption)
                    roomDamage = robotics.powerConsumption;

                // return power to the ship
                shipManager.reactor += roomDamage;

                // apply damage to shield
                robotics.powerConsumption -= roomDamage;

            }

            // refresh power usage
            GameManager.BattleUiManager.RefreshPowerUsage();

        }

        // repair system with a crew member or robot
        public override void RepairOneBar() {
            base.RepairOneBar();
        }

        private void RepairRobots() {

            for(int i = 0; i < crewmembers.Count; i++) {

                Crewmember crewmember = crewmembers[i].GetComponent<Crewmember>();

                if(crewmember.IsMachine)
                    crewmember.Heal(robotics.powerConsumption * robotics.repairRatio * Time.deltaTime);
                
            }

        }
        
    }

}
