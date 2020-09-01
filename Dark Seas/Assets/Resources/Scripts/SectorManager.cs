using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {
    public class SectorManager : MonoBehaviour {

        private const float POINT_CONNECTION_MAX_DISTANCE = 3f;
        // max aras
        private const int MAX_AREAS = 5;
        private const int MAX_SAME_AREA = 2;
        // max n events
        private const int MAX_BOUNTIES = 3;
        private const int MAX_DISTRESSES = 3;
        private const int MAX_ELITES = 2;
        private const int MAX_MERCENARIES = 1;
        private const int MAX_PIRATES = 3;
        private const int MAX_QUESTS = 1;
        private const int MAX_STORES = 4;
        private const int MAX_VOYAGERS = 2;
        // min n events
        private const int MIN_BOUNTIES = 1;
        private const int MIN_DISTRESSES = 1;
        private const int MIN_ELITES = 1;
        private const int MIN_MERCENARIES = 0;
        private const int MIN_PIRATES = 1;
        private const int MIN_QUESTS = 0;
        private const int MIN_STORES = 1;
        private const int MIN_VOYAGERS = 0;

        private int nAreas;
        private int nBounties;
        private int nDistresses;
        private int nElites;
        private int nMercernaries;
        private int nPirates;
        private int nQuests;
        private int nStores;
        private int nVoyagers;

        [SerializeField] private List<GameObject> areas = new List<GameObject>();
        [SerializeField] private Zone zone;
        [Space(10)]
        public GameObject areaPrefab;
        public GameObject eventPawnPrefab;
        public GameObject shipSilhoette;
        [Space(10)]
        public Point currentPoint;
        public GameObject sector;
        [Space(10)]
        [Header("Points")]
        public GameObject pointsParent;
        public int NumberOfPoints { get { return pointsParent.transform.childCount; } }
        public List<GameObject> centerPoints = new List<GameObject>();
        public List<GameObject> borderPoints = new List<GameObject>();
        [Space(10)]
        [Header("Locations")]
        public List<Location> regularLocations = new List<Location>();
        public List<Location> redLocations = new List<Location>();
        public List<Location> yellowLocations = new List<Location>();
        public List<Location> blueLocations = new List<Location>();  

        [SerializeField] public GameObject Entrance { get; private set; }
        [SerializeField] public GameObject Exit { get; private set; }

        public List<Location> LocationsToSpawn { get => locationsToSpawn; }

        private List<Location> locationsToSpawn = new List<Location>();
        private Dictionary<EventType, int> eventAmount = new Dictionary<EventType, int>();

        private void Awake() {

            // get zone
            zone = GameManager.currentZone;            

            ConnectPoints();

            SetEntranceAndExit();

            SetAreas();

            SetEventsQuantity();

            SetupEventPawns();

            ShuffleZoneLocations();

            AssignLocationsToPoints();

        }

        private void ConnectPoints() {

            GameObject currentPoint = null;

            for(int i = 0; i < NumberOfPoints; i++) {
                currentPoint = pointsParent.transform.GetChild(i).gameObject;
                Point point = currentPoint.GetComponent<Point>();
                for(int j = 0; j < NumberOfPoints; j++) {
                    GameObject otherPoint = pointsParent.transform.GetChild(j).gameObject;
                    if(otherPoint != currentPoint) {
                        if(Vector2.Distance(otherPoint.transform.position, currentPoint.transform.position) < POINT_CONNECTION_MAX_DISTANCE)
                            point.neighbourPoints.Add(otherPoint);
                    }
                }
                //point.ConnectToNeighbours();
            }

        }

        private void SetEntranceAndExit() {

            #region randomize

            int random1 = Random.Range(0, borderPoints.Count);
            Entrance = borderPoints[GameManager.RGN(borderPoints.Count)];

            int random2 = Random.Range(0, centerPoints.Count);
            Exit = centerPoints[GameManager.RGN(centerPoints.Count)];

            #endregion

            shipSilhoette.transform.position = Entrance.transform.position;
            currentPoint = Entrance.GetComponent<Point>();
            currentPoint.IsOccupied = true;

            SpriteRenderer entranceSR = Entrance.GetComponent<SpriteRenderer>();
            SpriteRenderer exitSR = Exit.GetComponent<SpriteRenderer>();
            entranceSR.sprite = Resources.Load<Sprite>("Sprites/UI/entrance");
            exitSR.sprite = Resources.Load<Sprite>("Sprites/UI/exit");

        }

        private void SetAreas() {

            int numberTypesOfArea = System.Enum.GetValues(typeof(AreaType)).Length;

            for (; areas.Count < MAX_AREAS;) {

                int randomAreaTypeInt = Random.Range(1, numberTypesOfArea);
                AreaType randomAreaType = (AreaType)randomAreaTypeInt;

                Vector3 areaPos = RandomCoord();

                GameObject areaGO = Instantiate(areaPrefab, transform.position, Quaternion.identity);
                areas.Add(areaGO);
                areaGO.transform.SetParent(sector.transform);
                SectorArea area = areaGO.GetComponent<SectorArea>();

                area.Spawn(randomAreaType, areaPos);

                bool overlappingAreaDetected;

                do {
                    overlappingAreaDetected = false;
                    for (int l = 0; l < areas.Count; l++) {
                        Collider2D[] hits = area.CollidersInArea();
                        foreach (Collider2D hit in hits) {
                            //if (area.Collider.bounds.Contains(hit.transform.position))
                            if (hit.CompareTag("Area") && hit != area.Collider) {
                                //if (hit.GetComponent<SectorArea>().Type == area.Type)
                                    area.transform.position = RandomCoord();
                                    overlappingAreaDetected = true;
                            }
                        }
                    }
                } while (overlappingAreaDetected);

                area.CheckPointsInside();

            }

        }

        private Vector2 RandomCoord() {
            Vector2 minPos = Camera.main.ScreenToWorldPoint(new Vector2(0, 0), Camera.main.stereoActiveEye);
            Vector2 maxPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height), Camera.main.stereoActiveEye);

            float randomX = Random.Range(minPos.x, maxPos.x);
            float randomY = Random.Range(minPos.y, maxPos.y);

            return new Vector3(randomX, randomY, 1);
        }

        private void SetEventsQuantity() {

            nBounties = Random.Range(MIN_BOUNTIES , MAX_BOUNTIES);
            nDistresses = Random.Range(MIN_DISTRESSES, MAX_DISTRESSES);
            nElites = Random.Range(MIN_ELITES, MAX_ELITES);
            nMercernaries = Random.Range(MIN_MERCENARIES, MAX_MERCENARIES);
            nPirates = Random.Range(MIN_PIRATES, MAX_PIRATES);
            nQuests = Random.Range(MIN_QUESTS, MAX_QUESTS);
            nStores = Random.Range(MIN_STORES, MAX_STORES);
            nVoyagers = Random.Range(MIN_VOYAGERS, MAX_VOYAGERS);

            // add to dictionary
            eventAmount.Add(EventType.Bounty, nBounties);
            eventAmount.Add(EventType.Distress, nDistresses);
            eventAmount.Add(EventType.Elite, nElites);
            eventAmount.Add(EventType.Mercenary, nMercernaries);
            eventAmount.Add(EventType.Pirate, nPirates);
            eventAmount.Add(EventType.Quest, nQuests);
            eventAmount.Add(EventType.Store, nStores);
            eventAmount.Add(EventType.Voyager, nVoyagers);

        }

        private void SetupEventPawns() {

            int numberTypesOfEvent = System.Enum.GetValues(typeof(EventType)).Length;

            for(int i = 0; i < numberTypesOfEvent; i++) {
                EventType eventType = (EventType)i;
                int amount = eventAmount[eventType];
                SpawnEvent(eventType, amount);
            }
            

        }

        private void ShuffleZoneLocations() {
            //zone.locations.Sort();
            GameManager.Shuffle(zone.locations);
        }

        private void AssignLocationsToPoints() {
            for(int i = 0; i < NumberOfPoints; i++) {

                if(i > zone.locations.Count - 1)
                    break;

                GameObject pointGO = pointsParent.transform.GetChild(i).gameObject;                
                Point point = pointGO.GetComponent<Point>();
                if(pointGO != Entrance && pointGO != Exit)
                    point.location = zone.locations[i];

            }
        }

        private void SpawnEvent(EventType type, int amount) {

            for (int i = 0; i < amount; i++) {
                GameObject eventPawnGO = Instantiate(eventPawnPrefab, transform.position, Quaternion.identity);
                eventPawnGO.transform.SetParent(sector.transform);
                EventPawn eventPawn = eventPawnGO.GetComponent<EventPawn>();

                Point randomPoint = GetRandomPoint(true, type);
                if(randomPoint != null) {
                    eventPawn.Spawn(type, randomPoint);

                if(!eventPawn.IsMobile)
                    randomPoint.eventPawn = eventPawn;
                
                } else {
                    Debug.LogError("Something's wrong, the event pawn " + eventPawn.name + " couldn't get a point to spawn on");
                }
                
            }

        }
        
        private void Start() {

            // play BGM
            //AudioManager.i.PlayBGM();

        }

        public void EnterLocation(Location _location, GameObject _currentPoint) {

            // leave the current point first            
            currentPoint.IsOccupied = false;
            // player jumped
            GameManager.PlayerJump();

            // set current location
            GameManager.LocationManager.location = _location;
            GameManager.LocationManager.gameObject.SetActive(true);
            // set new current point
            currentPoint = _currentPoint.GetComponent<Point>();  
            currentPoint.IsOccupied = true;

            GameManager.LocationManager.eventCanvasGroupGO.SetActive(true);

            // move ship silhoette to new point
            shipSilhoette.transform.position = currentPoint.transform.position;

            // deactivate sector map to enter location
            sector.SetActive(false);

            // lock camera
            Camera.main.GetComponent<CameraManager>().isLocked = true;
            // reset camera position
            Camera.main.transform.position = Vector3.zero;

            // pause game
            GameManager.PauseGame();

        }

        public Point GetRandomPoint(bool unoccupied, EventType type) {
            int randomN = Random.Range(0, pointsParent.transform.childCount);
            Point randomPoint = pointsParent.transform.GetChild(randomN).GetComponent<Point>();
            if (!randomPoint) {
                Debug.LogError("Points are missing on points parent. Be sure points are created before this function gets called.");
                return null;
            }
            if (unoccupied) {
                bool isRestricted;
                do {
                    isRestricted = false;

                    randomN = Random.Range(0, pointsParent.transform.childCount);
                    randomPoint = pointsParent.transform.GetChild(randomN).GetComponent<Point>();

                    for (int i = 0; i < areas.Count; i++) {
                        SectorArea area = areas[i].GetComponent<SectorArea>();
                        if (area.PointsInside.Contains(randomPoint.gameObject))
                            if (area.RestrictedEvents.Contains(type) || randomPoint == Entrance || randomPoint == Exit)
                                isRestricted = true;
                    }                    
                } while((randomPoint.IsOccupied || isRestricted));

            }
            return randomPoint;
        }

    
    }
}
