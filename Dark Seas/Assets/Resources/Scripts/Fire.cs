using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {

    public class Fire : MonoBehaviour {

        private LayerMask tileLayerMask;

        [SerializeField] private List<Transform> tilesToSpread;
        [SerializeField] private List<Transform> neighbourTiles = new List<Transform>();

        private float spreadTime;

        private const float DAMAGE = 10f;
        private const float DISTANCE_BETWEEN_TILES = 1f;
        private const float RAY_SIZE = 1f;
        private const float TIME_TO_SPREAD = 5f;

        private Room room;

        private void Start() {

            // set time spread time
            spreadTime = TIME_TO_SPREAD;

            // set layer mask
            tileLayerMask = LayerMask.GetMask("Walkable");

            // check and set neighbour tiles
            CheckNeighbourTiles();

            // get room from the first neighbour tile
            room = neighbourTiles[0].GetComponent<FloorTile>().Room;

            // copy neighbour tiles to tiles to spread
            tilesToSpread = new List<Transform>(neighbourTiles);

            // start coroutine
            StartCoroutine(Spread());

        }

        private void CheckNeighbourTiles() {

            neighbourTiles.Clear();

            Vector2 pos = this.transform.position;

            Vector2 offset1 = new Vector2(pos.x - DISTANCE_BETWEEN_TILES, pos.y);
            Vector2 offset2 = new Vector2(pos.x + DISTANCE_BETWEEN_TILES, pos.y);
            Vector2 offset3 = new Vector2(pos.x, pos.y - DISTANCE_BETWEEN_TILES);
            Vector2 offset4 = new Vector2(pos.x, pos.y + DISTANCE_BETWEEN_TILES);

            RaycastHit[] hit1 = Physics.RaycastAll(offset1, Vector3.forward, RAY_SIZE, tileLayerMask);
            RaycastHit[] hit2 = Physics.RaycastAll(offset2, Vector3.forward, RAY_SIZE, tileLayerMask);
            RaycastHit[] hit3 = Physics.RaycastAll(offset3, Vector3.forward, RAY_SIZE, tileLayerMask);
            RaycastHit[] hit4 = Physics.RaycastAll(offset4, Vector3.forward, RAY_SIZE, tileLayerMask);

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

        private FloorTile TileToSpread() {

            //("start: tiles to spread to: " + tilesToSpread.Count + " Neighbours: " + neighbourTiles.Count);
            tilesToSpread.Clear();

            foreach (Transform neighbourTile in neighbourTiles) {
                FloorTile neighbourTileCheck = neighbourTile.GetComponent<FloorTile>();
                if (!neighbourTileCheck.IsOnFire)
                    tilesToSpread.Add(neighbourTile);
            }

            // return null if there are no tiles around available to spread to
            if (tilesToSpread.Count < 1)
                return null;

            int randomTileIndex = Random.Range(0, tilesToSpread.Count);

            FloorTile tile = tilesToSpread[randomTileIndex].GetComponent<FloorTile>();

            return tile;

        }

        IEnumerator Spread() {

            while (true) {

                spreadTime -= Time.deltaTime;

                if (spreadTime <= 0) {

                    spreadTime = TIME_TO_SPREAD;

                    // apply DAMAGE
                    room.TakeDamage(DAMAGE);

                    // get tile to spread
                    FloorTile tileToSpread = TileToSpread();

                    // add fire to it, if there's any tile to spread to
                    if (tileToSpread != null)
                        tileToSpread.AddFire();

                }

                yield return 0;

            }

        }

        private void OnDestroy() {
            StopCoroutine("Spread");
        }

    }

}