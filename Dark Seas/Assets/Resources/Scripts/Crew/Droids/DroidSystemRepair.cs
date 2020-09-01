namespace DarkSeas {

    public class DroidSystemRepair : Droid {

        public override void Start() {

            base.Start();

            onPeriodicCheck += CheckForDamagedRooms;
            onEnterRoomCallback += CheckRoomStatus;
            //onLeftRoomCallback += StopSystemRepairProgress;

        }

        public override void Update() {

            base.Update();

            // if not in a room, skip
            if(!currentRoom)
                return;

            // if repairing and in a damaged room
            if (state == State.Working && currentRoom.IsDamaged && !currentRoom.IsOnFire && !currentRoom.repairers.Contains(this.gameObject))
                SystemRepair();
            // if repairing but not in a damaged room
            else if (state == State.Working && !currentRoom.IsDamaged)
                StopSystemRepairProgress();

        }

        private void CheckRoomStatus() {
            
            // if not in a room, return
            if(!isOnRoom)
                return;

            // if the current room is damaged, repair it
            if (currentRoom.IsDamaged && currentRoom == roomToGo)
                state = State.Working;
            else
            // if not, stop the process
                StopSystemRepairProgress();

        }

        private void CheckForDamagedRooms() {

            // reset room to fix
            roomToGo = null;

            // iterate through all of the ship's rooms
            for (int i = 0; i < ship.rooms.Count; i++) {

                Room room = ship.rooms[i].GetComponent<Room>();

                // if this room is not damaged, continue
                if (!room.IsDamaged || room.IsOnFire)
                    continue;

                // if there is no room to fix or this room has a higher priority, fix this room first
                if (roomToGo == null)
                    roomToGo = room;
                else if (room.system.targetPriority > roomToGo.system.targetPriority)
                    roomToGo = room;

            }

            // if there's a room to fix
            if (roomToGo != null) {
                // go to that room
                if (path.targetTransform != roomToGo.transform) {
                    ClearDestination();
                    path.targetTransform = roomToGo.transform;
                } else
                    path.targetTransform = roomToGo.transform;

                // check if inside a damaged room
                CheckRoomStatus();

            }

        }

    }

}