using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    public class GalaxyManager : MonoBehaviour
    {
        private const int DISTANCE_BETWEEN_SECTORS_X = 3;
        private const int DISTANCE_BETWEEN_SECTORS_Y = 2;

        public Transform ship;
        public Transform sectorsGroup;
        
        public GameObject sectorPrefab;

        [SerializeField]
        private List<Transform> sectors = new List<Transform>();

        void Start()
        {

            AudioManager.i.PlayMenuTheme();

            GameManager.gameStart = GameManager.GetNetTime();

            // scan sectors.
            ScanSectors();

            // sort sectors.
            SortSectors();

            // TEMPORARY
            GameManager.LocationManager.gameObject.SetActive(true);
            GameManager.ship.gameObject.SetActive(true);
            GameManager.ship.gameObject.SetActive(false);
            GameManager.LocationManager.gameObject.SetActive(false);

        }

        private void ScanSectors()
        {

            Vector2 pos = ship.position;

            Vector2 offset1 = new Vector2(pos.x + DISTANCE_BETWEEN_SECTORS_X, pos.y + DISTANCE_BETWEEN_SECTORS_Y);
            Vector2 offset2 = new Vector2(pos.x + DISTANCE_BETWEEN_SECTORS_X, pos.y - DISTANCE_BETWEEN_SECTORS_Y);
            Vector2 offset3 = new Vector2(pos.x - DISTANCE_BETWEEN_SECTORS_X, pos.y - DISTANCE_BETWEEN_SECTORS_Y);
            Vector2 offset4 = new Vector2(pos.x - DISTANCE_BETWEEN_SECTORS_X, pos.y + DISTANCE_BETWEEN_SECTORS_Y);

            RaycastHit2D hit1 = Physics2D.Raycast(offset1, -Vector2.up);
            RaycastHit2D hit2 = Physics2D.Raycast(offset2, -Vector2.up);
            RaycastHit2D hit3 = Physics2D.Raycast(offset3, -Vector2.up);
            RaycastHit2D hit4 = Physics2D.Raycast(offset4, -Vector2.up);

            if (hit1.collider == null)
                SpawnSector(offset1);

            if (hit2.collider == null)
                SpawnSector(offset2);

            if (hit3.collider == null)
                SpawnSector(offset3);

            if(hit4.collider == null)
                SpawnSector(offset4);

        }

        private void SpawnSector(Vector2 position)
        {

            GameObject sector = Instantiate(sectorPrefab, position, Quaternion.identity);
            sector.name = "X" + position.x.ToString() + "Y" + position.y.ToString();
            sector.transform.SetParent(sectorsGroup);

        }

        private void SortSectors()
        {
            foreach (Transform go in sectorsGroup)
            {
                sectors.Add(go);
            }
        }

    }
}
