using System.Collections.Generic;
using UnityEngine;

public class PointGenerator : MonoBehaviour {

    public GameObject smGO;

    [SerializeField]
    private GameManager gm;
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
    private float minX = -8f;
    private float maxY = 4f;
    private float minY = -4f;

    void Start() {

        // find game manager script.
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        // find sector manager script.
        sm = smGO.GetComponent<SectorManager>();

        // generate points.
        for (int i = 0; i < sm.pointsAmount; i++) {
            SpawnPoint();
        }

        // connect points.
        ConnectPoints();
        
        // define exit and entrance.
        DefineEntranceExit();

        // TODO: set points with sector manager?

        gm.SpawnShipSilhouette(entrance.transform.position);
        gm.shipSilhoette.transform.SetParent(this.transform.parent);

    }

    void SpawnPoint() {

        Vector3 currentPointLocation = Vector3.zero;
        bool correctPos = false;

        while (!correctPos) {

            correctPos = true;

            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            currentPointLocation = new Vector3(randomX, randomY, 1);

            foreach (GameObject p in points) {

                if (Vector2.Distance(p.transform.position, currentPointLocation) < gapBetweenPoints)
                    correctPos = false;

            }

        }

        GameObject pointGO = Instantiate(pointPrefab, currentPointLocation, Quaternion.identity);
        pointGO.transform.SetParent(this.transform);

        points.Add(pointGO);

    }

    void ConnectPoints() {

        GameObject currentPoint = null ;

        foreach (GameObject p in points) {

            currentPoint = p;
            Point point = p.GetComponent<Point>();

            foreach (GameObject np in points) {

                if (np != p) {

                    if (Vector2.Distance(np.transform.position, p.transform.position) < connectionMaxDistance) {

                        point.neighbourPoints.Add(np);

                    }

                }

            }

            point.ConnectToNeighbours();

        }

    }

    void DefineEntranceExit() {

        // decide entrance.
        int randomEnt = Random.Range(0, points.Count - 1);
        entrance = points[randomEnt];
        GameObject entrancePoint = entrance;
        entrance.name = "Entrance";
        // send entrance to sector manager.
        sm.entrance = entrancePoint;
        // set entrance as current location and as visited;
        sm.currentPoint = entrancePoint;
        entrancePoint.GetComponent<Point>().wasVisited = true;

        entrancePoint.GetComponent<SpriteRenderer>().sprite = entranceSprite;

        // decide exit.
        int randomExt = Random.Range(0, points.Count - 1);
        while (randomExt == randomEnt) {
            randomExt = Random.Range(0, points.Count - 1);
        }
        exit = points[randomExt];
        GameObject exitPoint = exit;
        exit.name = "Exit";
        // send exit to sector manager.
        sm.exit = exitPoint;

        exitPoint.GetComponent<SpriteRenderer>().sprite = exitSprite;

    }
    
}
