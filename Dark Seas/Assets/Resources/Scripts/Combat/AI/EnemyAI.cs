using UnityEngine;

namespace DarkSeas {
    [RequireComponent(typeof(Ship))]
    public class EnemyAI : MonoBehaviour {

        public bool gridGenerated;

        [SerializeField] private int ammoDivisor = 2;

        private Ship playerShip;
        private Ship AIship;

        void Start() {
            playerShip = GameManager.ship;
            AIship = this.GetComponent<Ship>();

        }

        void Update() {
            if (!gridGenerated) {
                // generate grid
                AIship.pathGrid.Generate(AIship);
                gridGenerated = true;
            }

            //check for hp.
            if (AIship.hull <= 0)
                Defeated();
            //check for fire
            //check for breaches
            //check for damaged systems
            CheckWeapons();

        }

        void CheckWeapons() {

            foreach (Transform weaponChild in AIship.arsenal.transform) {

                Weaponry weaponry = weaponChild.GetComponent<Weaponry>();

                // if on cooldown or damaged, skip.
                if (!weaponry.OnCooldown && !weaponry.IsDamaged && weaponry.IsPowered) {

                    // select weapon
                    weaponry.isSelected = true;

                    // check target systems with current weapon
                    CheckTargetSystems(weaponry);

                    // shoot with the current weapon
                    weaponry.Shoot();

                }

            }

        }

        private void CheckTargetSystems(Weaponry weaponry) {

            int highestTargetPriority = -1;
            Transform targetRoom = null;

            // loop through every room and target the one with the highest priority
            for (int i = 0; i < playerShip.rooms.Count; i++) {

                // set room
                Room room = playerShip.rooms[i].GetComponent<Room>();

                // if there is no room system, return
                if (room.system != null && room.CurrentHP > 0) {
                    // check room's target priority
                    if (room.system.targetPriority > highestTargetPriority) {
                        highestTargetPriority = room.system.targetPriority;
                        targetRoom = room.transform;
                    }
                }

            }

            if (targetRoom == null) {

                for (int i = 0; i < playerShip.rooms.Count; i++) {

                    // set room
                    Room room = playerShip.rooms[i].GetComponent<Room>();

                    // if there is no room system, return
                    if (room.system != null) {
                        // check room's target priority
                        if (room.system.targetPriority > highestTargetPriority) {
                            highestTargetPriority = room.system.targetPriority;
                            targetRoom = room.transform;
                        }
                    }

                }
            }

            // target a random tile in the room
            int randomTileIndex = Random.Range(0, targetRoom.GetComponent<Room>().Tiles.Count);
            GameObject randomTile = targetRoom.GetChild(randomTileIndex).gameObject;
            Transform target = randomTile.transform;
            weaponry.SetTarget(target);

        }

        void Defeated() {

            //TODO ship explosion

            // drop collectibles

            GameObject collectible;

            if (AIship.fuel > 0) {
                collectible = Instantiate(AIship.collectiblePrefab, transform.position + RandomOffset(), Quaternion.identity);
                collectible.GetComponent<Collectible>().fuel = AIship.fuel;
                collectible.transform.SetParent(GameManager.LocationManager.transform);
            }
            if (AIship.ammo > 0) {
                collectible = Instantiate(AIship.collectiblePrefab, transform.position + RandomOffset(), Quaternion.identity);
                int collectibleAmmo;
                collectibleAmmo = Mathf.RoundToInt((AIship.ammo / ammoDivisor));
                collectible.GetComponent<Collectible>().ammo = (collectibleAmmo <= 1) ? collectibleAmmo = 1 : collectibleAmmo;
                collectible.transform.SetParent(GameManager.LocationManager.transform);
            }
            if (AIship.scraps > 0) {
                collectible = Instantiate(AIship.collectiblePrefab, transform.position + RandomOffset(), Quaternion.identity);
                collectible.GetComponent<Collectible>().scraps = AIship.scraps;
                collectible.transform.SetParent(GameManager.LocationManager.transform);
            }

            GameManager.LocationManager.EnemyShips.Remove(this.gameObject);

            Destroy(this.gameObject);

            //TODO VICTORY ACHIEVED
            //GameManager.LocationManager.gameObject.SetActive(true);
            if (GameManager.LocationManager.EnemyShips.Count == 0)
                GameManager.LocationManager.BattleWon();

        }

        private Vector3 RandomOffset() {
            float randomX = Random.Range(transform.position.x, 1);
            float randomY = Random.Range(transform.position.y, 1);
            return new Vector3(randomX, randomY, 1);
        }

    }
}
