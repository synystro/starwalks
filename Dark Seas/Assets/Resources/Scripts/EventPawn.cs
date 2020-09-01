using UnityEngine;
using System.Collections.Generic;

namespace DarkSeas {

    [System.Serializable]
    public enum EventType {
            Bounty,
            Distress,
            Elite,
            Mercenary,
            Pirate,
            Quest,
            Store,
            Voyager
    };

    [RequireComponent(typeof(SpriteRenderer))]
    public class EventPawn : MonoBehaviour {   

        // chances
        private const int FIGHT_CHANCE = 20;
        private const int MOBILE_CHANCE = 30;
        private const int STORE_CHANCE = 10;

        private Point currentPoint;
        [SerializeField] private Location locationEvent;
        [SerializeField] private GameObject currentPointGO;
        [SerializeField] private GameObject previousPointGO;
        [SerializeField] private GameObject nextPointGO;
        [SerializeField] private bool isFight;
        [SerializeField] private bool isMobile;
        [SerializeField] private bool isStore;
        [SerializeField] private bool isInsideNebula;
        [Space(10)]
        [SerializeField] List<GameObject> closestPoints = new List<GameObject>();

        private Vector2 pos;        
        private Sprite sprite;
        private SpriteRenderer sr;
        private CircleCollider2D col;
        private GameObject lineToTarget;

        public EventType type;
        public Location LocationEvent { get { return locationEvent; } }
        public bool IsMobile { get { return isMobile; } }

        private void Awake() { sr = this.GetComponent<SpriteRenderer>(); col = this.GetComponent<CircleCollider2D>(); }

        public void Spawn(EventType _type, Point _point) {

            type = _type;
            currentPointGO = _point.gameObject;
            currentPoint = _point;
            currentPoint.IsOccupied = true;
            currentPoint.eventPawn = null;
            this.transform.position = _point.transform.position;
            this.gameObject.name = "Event " + type.ToString();            

            SetSprite();

            SetEventProperties();

            AssignEvent();

            UpdateClosestPoints();

            HandleVisibility();

            if(isMobile)
                HeadingTo();

            GameManager.onPlayerJump += OnPlayerJump;

        }

        public void Destroy() {
            Destroy(this.gameObject);
        }

        private void SetSprite() {
            sprite = Resources.Load<Sprite>("Sprites/EventPawns/" + type.ToString());
            sr.sprite = sprite;

            if(sprite == null)
                Debug.LogError("Sprite not found for " + this.name + ", fix the sprite's file name?");
        }

        private void SetEventProperties() {

            switch (type) {
                case EventType.Bounty:
                    isMobile = true;
                    break;
                case EventType.Distress:
                    isFight = RandomChance(FIGHT_CHANCE);
                    if (!isFight)
                        isStore = RandomChance(STORE_CHANCE);
                    break;
                case EventType.Elite:
                    isMobile = true;
                    isFight = true;
                    break;
                case EventType.Mercenary:
                    isMobile = true;
                    break;
                case EventType.Pirate:
                    isMobile = true;
                    isFight = true;
                    break;
                case EventType.Quest:
                    break;
                case EventType.Store:
                    isMobile = RandomChance(MOBILE_CHANCE);
                    isFight = RandomChance(FIGHT_CHANCE);
                    isStore = true;
                    break;
                case EventType.Voyager:
                    isMobile = true;
                    break;
                default:
                    break;
            }

        }

        private void AssignEvent() {
            Location[] allEventsOfType = Resources.LoadAll<Location>("In-Game/Locations/Events/" + type.ToString());

            if(allEventsOfType.Length < 1)
                Debug.LogError("Events missing for " + this.name + ", did the folder path change or it empty?");

            locationEvent = GameManager.SelectRandomArrayContent(allEventsOfType);
        }

        private bool RandomChance(int chance) {
            int randomN = Random.Range(0, 100);
            return randomN < STORE_CHANCE;
        }
        
        private void UpdateClosestPoints() {

            closestPoints.Clear();

            for(int i = 0; i < currentPoint.neighbourPoints.Count; i++)
                if(currentPoint.neighbourPoints[i] != previousPointGO)
                    closestPoints.Add(currentPoint.neighbourPoints[i]);            

        }

        private void HeadingTo() {

            List<GameObject> availablePoints = new List<GameObject>();

            for (int i = 0; i < closestPoints.Count; i++)
                if (!closestPoints[i].GetComponent<Point>().IsOccupied)
                    availablePoints.Add(closestPoints[i]);

            if(availablePoints.Count <= 0)
                return;

            int randomN = Random.Range(0, availablePoints.Count); 
            nextPointGO = availablePoints[randomN];
            Point nextPoint = nextPointGO.GetComponent<Point>();
            nextPoint.IsOccupied = true;
            nextPoint.eventPawn = this;

            // rotate towards next point?
            if(type == EventType.Voyager)
                transform.up = nextPointGO.transform.position - this.transform.position;
            // draw line towards next point (except when inside nebula)
            if(lineToTarget != null)
                Destroy(lineToTarget);
            if(!isInsideNebula)
                DrawLineTo(nextPointGO);

        }

        public bool Move() {

            currentPoint = currentPointGO.GetComponent<Point>();
            currentPoint.eventPawn = null;
            currentPoint.IsOccupied = false;
            //currentPoint.DestroyTargetLine();
            previousPointGO = currentPointGO;          
            currentPointGO = nextPointGO;
            currentPoint = currentPointGO.GetComponent<Point>();
            currentPoint.eventPawn = null;

            HandleVisibility();

            return true;
        }

        private void HandleVisibility() {

            if(currentPoint.area == AreaType.Nebula)
                isInsideNebula = true;
            else
                isInsideNebula = false;

            // turn event pawn invisible (sprite off) inside nebula
            if(isInsideNebula)
                this.GetComponent<SpriteRenderer>().enabled = false;
            else
                this.GetComponent<SpriteRenderer>().enabled = true;

            this.transform.position = currentPointGO.transform.position;
        }

        private void OnDestroy() {
            currentPoint.IsOccupied = true;
            GameManager.onPlayerJump -= OnPlayerJump;
        }

        private void OnPlayerJump() {

            // if disabled, skip (does this function get called by its delegate even when disabled?)
            if (!this.gameObject.activeSelf)
                return;

            if (isMobile) {
                Move();
                UpdateClosestPoints();
                HeadingTo();
            }

        }

        private void DrawLineTo(GameObject _target) {

            GameObject line = new GameObject();
            line.name = this.name + "'s line";
            line.transform.SetParent(this.transform);
            lineToTarget = line;
            LineRenderer lineRend = line.AddComponent<LineRenderer>();

            // lineRenderer settings
            lineRend.material = new Material(Shader.Find("Sprites/Default"));
            lineRend.widthMultiplier = 0.02f;
            lineRend.positionCount = 2;
            lineRend.sortingOrder = 1;
            // set start & end positions.
            lineRend.SetPositions(new Vector3[] { transform.position, _target.transform.position });

        }

    }

}