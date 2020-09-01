using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {
    public class Point : MonoBehaviour {

        public Location location;
        public EventPawn eventPawn;
        public bool IsOccupied { get; set; }
        public bool IsVisited { get; set; }
        public AreaType area;
        [Space(10)]
        public List<GameObject> neighbourPoints = new List<GameObject>();

        private Location emptyLocation;

        //private GameObject lineToTarget;
        private List<GameObject> lines = new List<GameObject>();
        private Vector3[] connectPositions;

        private SectorManager sm;

        private void Awake() {
            emptyLocation = Resources.Load<Location>("In-Game/Locations/AlreadyExplored");
            if(emptyLocation == null)
                Debug.LogError("Empty location is null. Either its path has changed or the file is missing!");
        }

        private void Start() {
            sm = GameManager.SectorManager;
        }

        public void ConnectToNeighbours() {

            connectPositions = new Vector3[neighbourPoints.Count];


            for (int i = 0; i < neighbourPoints.Count; i++) {

                connectPositions[i] = neighbourPoints[i].transform.position;

            }

        }

        private void DrawLines() {

            for (int i = 0; i < neighbourPoints.Count; i++) {

                // generate object to generate line (using LineRenderer). 
                GameObject line = new GameObject();
                line.transform.SetParent(this.transform);
                lines.Add(line);
                LineRenderer lineRend = line.AddComponent<LineRenderer>();

                // LineRenderer settings.
                lineRend.material = new Material(Shader.Find("Sprites/Default"));
                lineRend.widthMultiplier = 0.02f;
                lineRend.positionCount = 2;
                lineRend.sortingOrder = 1;
                // set start & end positions.
                lineRend.SetPositions(new Vector3[] { transform.position, neighbourPoints[i].transform.position });

            }

        }

        private void DestroyLines() {

            for (int i = 0; i < lines.Count; i++)
                Destroy(lines[i]);

            //DestroyTargetLine();

            lines.Clear();

        }

        private void CheckLocationToEnter() {

            if (IsOccupied && eventPawn != null) {
                DestroyLines();
                sm.EnterLocation(eventPawn.LocationEvent, this.gameObject);
                eventPawn.Destroy();
            } else {

                if (!IsVisited) {
                    DestroyLines();
                    sm.EnterLocation(location, this.gameObject);
                } else {
                    DestroyLines();
                    sm.EnterLocation(emptyLocation, this.gameObject);
                }

            }

        }

        public void OnMouseDown() {

            // check if this is the point the player is currently in, if true then no need to jump to it
            if (sm.currentPoint == this || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            int fuelNecessary = (int)Vector2.Distance(sm.currentPoint.transform.position, this.transform.position);

            Ship playerShip = GameManager.ship;

            if (location == null && eventPawn == null) {
                Debug.LogError("No location set for this point: " + transform.name);
                return;
            }

            if (playerShip.fuel >= fuelNecessary && location != null) {                

                CheckLocationToEnter();

                playerShip.fuel -= fuelNecessary;

            }
            else
                Debug.LogWarning("Not enough fuel to sail to this point. You need " + fuelNecessary);

        }

        public void OnMouseEnter() {
            DrawLines();
        }

        public void OnMouseExit() {
            DestroyLines();
        }

        // public void DestroyTargetLine() {
        //     if(lineToTarget != null)
        //         Destroy(lineToTarget);
        // }

    }
}
