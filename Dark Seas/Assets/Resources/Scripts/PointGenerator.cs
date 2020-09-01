using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    public class PointGenerator : MonoBehaviour
    {

        // set collider priority
        private const int colliderRayCastPriotity = 1;

        public GameObject smGO;

        [SerializeField]
        private SectorManager sm;

        public float gapBetweenPoints = 1f;
        public float connectionMaxDistance = 4f;

        public GameObject pointPrefab;

        [SerializeField]
        private GameObject entrance;
        public Sprite entranceSprite;
        [SerializeField]
        private GameObject exit;
        public Sprite exitSprite;

        private List<GameObject> points = new List<GameObject>();

        private float maxX = 8f;
        private float maxY = 4f;

        void Start()
        {

            // find sector manager script.
            sm = GameManager.SectorManager;

            // generate points.
            for (int i = 0; i < sm.pointsParent.transform.childCount; i++)
            {
                SpawnPoint();
            }

            // connect points.
            ConnectPoints();

            // define exit and entrance.
            DefineEntranceExit();

            // TODO: set points with sector manager?

            sm.shipSilhoette.transform.position = entrance.transform.position;
            sm.shipSilhoette.transform.SetParent(this.transform.parent);

        }

        void SpawnPoint()
        {

            Vector3 currentPointLocation = Vector3.zero;
            bool correctPos = false;

            while (!correctPos)
            {

                correctPos = true;

                float randomX = Random.Range(-maxX, maxX);
                float randomY = Random.Range(-maxY, maxY);

                currentPointLocation = new Vector3(randomX, randomY, colliderRayCastPriotity);

                foreach (GameObject p in points)
                {

                    if (Vector2.Distance(p.transform.position, currentPointLocation) < gapBetweenPoints)
                        correctPos = false;

                }

            }

            GameObject pointGO = Instantiate(pointPrefab, currentPointLocation, Quaternion.identity);
            pointGO.transform.SetParent(this.transform);

            AssignLocation(pointGO);

            points.Add(pointGO);

        }

        void ConnectPoints()
        {

            GameObject currentPoint = null;

            foreach (GameObject p in points)
            {

                currentPoint = p;
                Point point = p.GetComponent<Point>();

                foreach (GameObject np in points)
                {

                    if (np != p)
                    {

                        if (Vector2.Distance(np.transform.position, p.transform.position) < connectionMaxDistance)
                        {

                            point.neighbourPoints.Add(np);

                        }

                    }

                }

                point.ConnectToNeighbours();

            }

        }

        void DefineEntranceExit()
        {

            // decide entrance.
            int randomEnt = Random.Range(0, points.Count - 1);
            entrance = points[randomEnt];
            Point entrancePoint = entrance.GetComponent<Point>();
            entrance.name = "Entrance";
            // send entrance to sector manager.
            //sm.entrance = entrancePoint;
            // set entrance as current location and as visited;
            sm.currentPoint = entrancePoint;
            entrancePoint.IsVisited = true;

            entrancePoint.GetComponent<SpriteRenderer>().sprite = entranceSprite;

            // decide exit.
            int randomExt = Random.Range(0, points.Count - 1);
            while (randomExt == randomEnt)
            {
                randomExt = Random.Range(0, points.Count - 1);
            }
            exit = points[randomExt];
            GameObject exitPoint = exit;
            exit.name = "Exit";
            // send exit to sector manager.
            //sm.exit = exitPoint;

            exitPoint.GetComponent<SpriteRenderer>().sprite = exitSprite;

        }

        void AssignLocation(GameObject point)
        {

            int random = Random.Range(0, GameManager.currentZone.locations.Count);
            Location randomLocation = GameManager.currentZone.locations[random];

            point.GetComponent<Point>().location = randomLocation;

        }

    }
}
