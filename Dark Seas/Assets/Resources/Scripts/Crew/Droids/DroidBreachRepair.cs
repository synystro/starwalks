using UnityEngine;

namespace DarkSeas {

    public class DroidBreachRepair : Droid {

        public override void Start() {

            base.Start();

            onEnterRoomCallback += CheckRoomStatus;
            //onLeftRoomCallback += StopBreachFixProgress;

        }

        public override void Update() {

            base.Update();

            // if not in a room, skip
            if (!currentRoom)
                return;

            // if working and in a room that has a breach
            if (state == State.Working && currentRoom.IsBreached && !currentRoom.IsOnFire && !currentRoom.breachFixers.Contains(this.gameObject))
                FixBreach();
            // if working but not in a room that has a breach
            else if (state == State.Working && !currentRoom.IsBreached)
                StopBreachFixProgress();

            // check for rooms on fire
            CheckForBreachedRooms();

        }

        private void CheckRoomStatus() {

            // if the current room is breached and it's the higher priority room, fix it
            if (currentRoom.IsBreached && currentRoom == roomToGo)
                state = State.Working;
            else
                // if not, stop the process
                StopBreachFixProgress();

        }

        private void CheckForBreachedRooms() {

            // reset room to fix
            roomToGo = null;

            // iterate through all of the ship's rooms
            for (int i = 0; i < ship.rooms.Count; i++) {

                Room room = ship.rooms[i].GetComponent<Room>();

                // if this room is not breached, continue
                if (!room.IsBreached)
                    continue;

                // if there is no room breached or this room has a higher priority, fix the breach in this room first
                if (roomToGo == null)
                    roomToGo = room;
                else if (room.system != null)
                    if (roomToGo.system == null)
                        roomToGo = room;
                    else if(room.system.targetPriority > roomToGo.system.targetPriority)
                        roomToGo = room;

            }

            // if there's a room on fire
            if (roomToGo != null) {

                GameObject tileBreached = null;

                // set first breach to fix
                if (roomToGo.TilesBreached[0] != null)
                    tileBreached = roomToGo.TilesBreached[0];
                else
                    tileBreached = null;

                // go to that room
                if (tileBreached != null && path.targetTransform != tileBreached.transform) {
                    ClearDestination();
                    path.targetTransform = tileBreached.transform;
                } else if(tileBreached != null)
                    path.targetTransform = tileBreached.transform;

                // check if inside a damaged room
                CheckRoomStatus();

            }

        }

    }

}