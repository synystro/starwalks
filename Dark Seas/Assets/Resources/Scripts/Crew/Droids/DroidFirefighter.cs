using UnityEngine;

namespace DarkSeas {

    public class DroidFirefighter : Droid {

        public override void Start() {

            base.Start();

            onPeriodicCheck += CheckForRoomsOnFire;
            onEnterRoomCallback += CheckRoomStatus;
            //onLeftRoomCallback += StopFireExtinguishing;

        }

        public override void Update() {

            base.Update();

            // if not in a room, skip
            if (!currentRoom)
                return;

            // if working and in a room on fire
            if (state == State.Working && currentRoom.IsOnFire && !currentRoom.firefighters.Contains(this.gameObject))
                ExtinguishFire();
            // if working but not in a room on fire
            else if (state == State.Working && !currentRoom.IsOnFire)
                StopFireExtinguishing();

            // check for rooms on fire
            //CheckForRoomsOnFire();

            // make it go back to robotics room when work is finished

        }

        private void CheckRoomStatus() {

            // if not in a room, return
            if(!isOnRoom)
                return;

            // if the current room is on fire and it's the higher priority room, extinguish it
            if (currentRoom.IsOnFire && currentRoom == roomToGo)
                state = State.Working;
            else if (state == State.Working && !currentRoom.IsOnFire) {
                // if not, stop the process and go back to robotics room
                StopFireExtinguishing();
            }

        }

        private void CheckForRoomsOnFire() {

            // reset room to fix
            roomToGo = null;

            // iterate through all of the ship's rooms
            for (int i = 0; i < ship.rooms.Count; i++) {

                Room room = ship.rooms[i].GetComponent<Room>();

                // if this room is not on fire, continue
                if (!room.IsOnFire)
                    continue;

                // if there is no room on fire or this room has a higher priority, extinguish fire in this room first
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

                GameObject tileOnFire = null;

                // set first fire to put out
                if (roomToGo.TilesOnFire[0] != null)
                    tileOnFire = roomToGo.TilesOnFire[0];
                else
                    tileOnFire = null;

                // go to that room
                if (tileOnFire != null && path.targetTransform != tileOnFire.transform) {
                    ClearDestination();
                    path.targetTransform = tileOnFire.transform;
                } else if(tileOnFire != null)
                    path.targetTransform = tileOnFire.transform;

                // check if inside a damaged room
                CheckRoomStatus();

            }

        }

    }

}