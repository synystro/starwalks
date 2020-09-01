using UnityEngine;

namespace DarkSeas {

    public class Human : Crewmember {
        
        private Medbay medbay;

        public override void Awake() {
            base.Awake();
        }
        
        public override void Start() {

            base.Start();
            
            // this pawn is selectable and can be controlled
            isSelectable = true;

            onPeriodicCheck += CheckRoomStatus;
            onEnterRoomCallback += CheckRoomStatus;
            // onLeftRoomCallback += 

            // get infirmary's component
            medbay = ship.GetComponent<Medbay>();

        }
        
        public override void Update() {

            base.Update();

            HandleCheck();

            // upgrade crewmember specific skill
            crewmemberStats.UpgradeSkill();

        }

        private void CheckRoomStatus() {

            if(!isOnRoom)
                return;

            // check for fire
            if (currentRoom.IsOnFire && !currentRoom.firefighters.Contains(this.gameObject))
                ExtinguishFire();
            else if (state == State.Working && !currentRoom.IsOnFire) {
                StopFireExtinguishing();
            }
            // check for breaches repair
            if (currentRoom.IsBreached && !currentRoom.IsOnFire && !currentRoom.breachFixers.Contains(this.gameObject))
                FixBreach();
            else if (state == State.Working && !currentRoom.IsBreached)
                StopBreachFixProgress();
            // check for system repair
            else if (currentRoom.IsDamaged && !currentRoom.IsOnFire && !currentRoom.repairers.Contains(this.gameObject))
                SystemRepair();
            else if (state == State.Working && !currentRoom.IsDamaged)
                StopSystemRepairProgress();

            GainXP();

        }

        private void HandleCheck() {

            if(!roomSystem)
                return;

            if (currentRoom.System.IsPowered && roomSystem.handler == null && currentRoom.repairer != this.gameObject && !currentRoom.IsDamaged) {
                roomSystem.handler = this; // set this human as handler to the room
                roomSystem.RefreshToUI();
            } else if (roomSystem.handler == this && currentRoom.IsDamaged) {
                roomSystem.handler = null; // set room handler to null
                roomSystem.RefreshToUI();
            } else if (!currentRoom.System.IsPowered && roomSystem.handler != null) {
                roomSystem.handler = null; // set room handler to null
                roomSystem.RefreshToUI();
            }

        }

        private void RepairCheck() {

            if (currentRoom.repairer == null) {
                currentRoom.repairer = this.gameObject;
                roomSystem.RefreshToUI();
            } else if (currentRoom.repairer == this.gameObject && !currentRoom.IsDamaged) {
                currentRoom.repairer = null;
                roomSystem.RefreshToUI();
            }
            if (!isOnRoom || currentRoom.IsOnFire) {
                if (currentRoom.repairer == this.gameObject)
                    currentRoom.repairer = null;
                roomSystem.RefreshToUI();
            }

            if (!currentRoom.repairers.Contains(this.gameObject))
                SystemRepair();
            else if (!currentRoom.IsDamaged || !roomSystem)
                if (currentRoom.repairer == this.gameObject)
                    StopSystemRepairProgress();

        }

        private void GainXP() {

            #region Command Room

            if (currentRoom.name == "Command") // check if is inside command currentRoom
            {
                if (GameManager.InBattle)
                    crewmemberStats.pilotXP += Time.deltaTime; //give xp to helmsman
                else
                    crewmemberStats.pilotXP *= Time.deltaTime;
            }

            #endregion

            #region Engine Room

            if (currentRoom.name == "Engine") // check if is inside engine currentRoom
            {
                if (GameManager.InBattle)
                    crewmemberStats.engineerXP += Time.deltaTime; //give xp to engineer
            }

            #endregion

        }        

    }

}