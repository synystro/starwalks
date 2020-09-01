using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {
    public class FloorTile : MonoBehaviour {

        public float hp;

        public Sprite breachedTile;

        public Ship shipManager;

        //private SpriteRenderer sr;
        private Room room;
        private WeaponsRoom weaponsRoom;

        private GameObject fire;
        private GameObject breach;

        public bool IsOnFire { get; private set; }
        public bool IsBreached { get; private set; }
        public Room Room { get { return room; } }

        [SerializeField] private List<Transform> neighbourTiles = new List<Transform>();
        private LayerMask tileLayerMask;
        private const float distanceBetweenTiles = 1f;
        private const float raySize = 1f;

        private void Awake() {

            // set tile layer mask
            tileLayerMask = 1 << this.gameObject.layer;

            // get neighbour tiles
            CheckNeighbourTiles();

            // get this tile's room
            room = this.transform.parent.GetComponent<Room>();

        }

        private void Start() {

            CheckNeighbourRooms();

            weaponsRoom = this.transform.parent.GetComponent<WeaponsRoom>();

        }

        private void CheckNeighbourTiles() {

            neighbourTiles.Clear();

            Vector2 pos = this.transform.position;

            Vector2 offset1 = new Vector2(pos.x - distanceBetweenTiles, pos.y);
            Vector2 offset2 = new Vector2(pos.x + distanceBetweenTiles, pos.y);
            Vector2 offset3 = new Vector2(pos.x, pos.y - distanceBetweenTiles);
            Vector2 offset4 = new Vector2(pos.x, pos.y + distanceBetweenTiles);

            RaycastHit[] hit1 = Physics.RaycastAll(offset1, Vector3.forward, raySize, tileLayerMask);
            RaycastHit[] hit2 = Physics.RaycastAll(offset2, Vector3.forward, raySize, tileLayerMask);
            RaycastHit[] hit3 = Physics.RaycastAll(offset3, Vector3.forward, raySize, tileLayerMask);
            RaycastHit[] hit4 = Physics.RaycastAll(offset4, Vector3.forward, raySize, tileLayerMask);

            foreach (RaycastHit hit in hit1)
                neighbourTiles.Add(hit.transform);

            foreach (RaycastHit hit in hit2)
                neighbourTiles.Add(hit.transform);

            foreach (RaycastHit hit in hit3)
                neighbourTiles.Add(hit.transform);

            foreach (RaycastHit hit in hit4)
                neighbourTiles.Add(hit.transform);

            if (neighbourTiles.Count < 1)
                Debug.LogError("The tile " + transform.parent.name + " has no neighbour tiles at all?");

        }

        private void CheckNeighbourRooms(){

            foreach(Transform neighbour in neighbourTiles) {

                if(this.room == null){
                    Debug.LogError("this tile " + this.name + " doesn't have a room? : " + transform.parent.name);
                    return;
                }

                FloorTile neighbourTile = neighbour.GetComponent<FloorTile>();

                if(neighbourTile.room == null) {
                    Debug.LogError("neighbour tile " + neighbourTile.name + " doesn't have a room? : " + neighbourTile.transform.parent.name);
                    return;
                }

                if(this.room.gameObject.name != neighbourTile.room.gameObject.name)
                    this.room.neighbourRooms.Add(neighbourTile.room);

            }

        }

        public void TakeDamage(float damage, Weapon weapon) {

            float effectiveArmor = (shipManager.armor - weapon.pierce) / -100;

            float actualDamage = damage + (damage * effectiveArmor);

            print("The actual damage was " + actualDamage + "!");

            if(actualDamage < 0)
                actualDamage = 0;

            // damage tile
            hp -= actualDamage;

            // damage room (system)
            if (room != null)
                room.TakeDamage(actualDamage);
            else if (weaponsRoom != null)
                weaponsRoom.TakeDamage(actualDamage);

            // fire chance
            if (!IsOnFire) {
                // roll
                int randomDodgeNumber = Random.Range(0, 100);
                // success ? add fire
                if (randomDodgeNumber < weapon.fireChance)
                    AddFire();

            }

            // breach chance
            if (!IsBreached) {
                // roll
                int randomDodgeNumber = Random.Range(0, 100);
                // success ? add fire
                if (randomDodgeNumber < weapon.breachChance)
                    AddBreach();

            }


            // raycast to find pawns on this tile

            Vector2 tilePos = this.transform.position;
            RaycastHit2D[] hit = Physics2D.RaycastAll(tilePos, Vector2.zero);

            #region damage crew + droids on this tile

            for (int i = 0; i < hit.Length; i++) {

                GameObject hitGO = hit[i].transform.gameObject;

                if (hitGO.CompareTag("Crewmember"))
                    hitGO.GetComponent<Crewmember>().TakeDamage(damage * 2);
                else if (hitGO.CompareTag("Droid"))
                    hitGO.GetComponent<Droid>().TakeDamage(damage * 2);

            }

            #endregion

            // if hp reaches zero, change sprite to breached tile and end fire
            if (hp <= 0) {

                hp = 0;

                AddBreach();

                ExtinguishFire();

            }

            // damage ship hull
            shipManager.TakeDamage(actualDamage);

        }

        public void AddBreach() {

            breach = Instantiate(GameManager.BattleUiManager.breachPrefab, this.transform.position, Quaternion.identity);
            breach.transform.SetParent(this.transform);
            room.TilesBreached.Add(this.gameObject);

            IsBreached = true;
            shipManager.IsBreached = true;

        }

        public void RemoveBreach() {

            room.TilesBreached.Remove(this.gameObject);
            Destroy(breach);
            IsBreached = false;

        }

        public void AddFire() {

            fire = Instantiate(GameManager.BattleUiManager.firePrefab, this.transform.position, Quaternion.identity);
            fire.transform.SetParent(this.transform);
            room.TilesOnFire.Add(this.gameObject);
            IsOnFire = true;

        }

        public void RemoveFire() {

            room.TilesOnFire.Remove(this.gameObject);
            Destroy(fire);
            IsOnFire = false;

        }

        public void ExtinguishFire() {

            room.TilesOnFire.Remove(this.gameObject);
            Destroy(fire);
            IsOnFire = false;

        }

    }
}
