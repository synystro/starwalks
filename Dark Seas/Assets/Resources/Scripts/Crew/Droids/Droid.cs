using UnityEngine;

namespace DarkSeas {

    public class Droid : Crewmember {

        private Robotics robotics;

        public override void Awake() {
            base.Awake();
        }

        public override void Start() {

            base.Start();

            isMachine = true;

            // periodic check
            //onPeriodicCheck += CheckIfInsideRoboticsRoom;
            // on job finished
            onJobFinished += BackToRoboticsRoom;

            // this pawn is unselectable and can NOT be controlled
            isSelectable = false;

            // get robotics
            robotics = ship.robotics.GetComponent<Robotics>();

            // go to robotics room
            BackToRoboticsRoom();

        }

        public override void Update() {

            base.Update();

        }

        // private void CheckIfInsideRoboticsRoom() {

        //     // if not in a room, return
        //     if(!currentRoom)
        //         return;

        //     if (currentRoom.gameObject.name == robotics.system.name)
        //         // room not damaged ? heal
        //         if (!robotics.Room.IsDamaged && (crewmemberStats.health < crewmemberStats.maxHealth))
        //             Heal((robotics.powerConsumption / healRatio) * Time.deltaTime);

        // }     

        protected void BackToRoboticsRoom() {

            path.targetTransform = robotics.Room.transform;

        }
        
    }

}