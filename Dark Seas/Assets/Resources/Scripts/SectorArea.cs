using UnityEngine;
using System.Collections.Generic;

namespace DarkSeas {

    public enum AreaType {
        None,
        Danger,
        Nebula,
        Star
    }

    [RequireComponent(typeof(CircleCollider2D))]
    public class SectorArea : MonoBehaviour {

        private const float ALPHA_VALUE = 0.3f;
        private const float MAX_SCALE_SIZE = 12f;
        private const float MIN_SCALE_SIZE = 6f;

        [SerializeField] Color color;
        [SerializeField] private List<GameObject> pointsInside = new List<GameObject>();

        private AreaType type;

        private CircleCollider2D col;

        public CircleCollider2D Collider { get { return col; } }
        public AreaType Type { get { return type; } }
        public List<GameObject> PointsInside { get { return pointsInside; } }
        public List<EventType> RestrictedEvents = new List<EventType>();
        
        public void Spawn(AreaType _type, Vector3 _pos) {            
            type = _type;
            this.transform.position = _pos;

            this.gameObject.name = "Area " + type.ToString();

            SetSize();

            SetColor();

            SetRestrictedEvents();

            //CheckPointsInside();                  
                  
        }

        public Collider2D[] CollidersInArea() {
            float scaledRadius = col.radius * this.transform.localScale.x;
            return Physics2D.OverlapCircleAll(this.transform.position, scaledRadius);
        }

        public void CheckPointsInside() {

            Collider2D[] hits = CollidersInArea();

            foreach (Collider2D hit in hits) {
                //if (col.bounds.Contains(hit.transform.position))
                    if (hit.CompareTag("Point")) {
                        GameObject point = hit.gameObject;
                        point.GetComponent<Point>().area = type;
                        pointsInside.Add(point);
                    }
            }

            // after checking points inside, go back to 0 on z axis in order for the color's alpha to work... (FIX THIS?)
            this.transform.position = new Vector3 (
                transform.position.x,
                transform.position.y,
                0
            );

            AssignAreaTypeToPoint();

        }

        private void Awake() { col = this.GetComponent<CircleCollider2D>(); }

        private void SetSize() {
            float randomScaleSizeValue = Random.Range(MIN_SCALE_SIZE, MAX_SCALE_SIZE);
            Vector3 localScaleSize = new Vector3(randomScaleSizeValue, randomScaleSizeValue, 1);
            this.transform.localScale = localScaleSize;
        }

        private void SetColor() {

            switch(type) {
                case AreaType.Danger:
                    color = Color.red;
                    break;
                case AreaType.Nebula:
                    color = Color.blue;
                    break;
                case AreaType.Star:
                    color = Color.yellow;
                    break;
                default:
                    Debug.LogError("No color set for the " + this.name + " area gameobject");
                    color = Color.white;
                    break;
            }

            color.a = ALPHA_VALUE;

            Renderer rend = this.GetComponent<Renderer>();
            rend.material.SetColor("_Color", color);  

        }

        private void SetRestrictedEvents() {
            switch(type) {
                case AreaType.Danger:
                    RestrictedEvents.Add(EventType.Store);
                    RestrictedEvents.Add(EventType.Voyager);
                    break;
                case AreaType.Nebula:
                    RestrictedEvents.Add(EventType.Distress);
                    RestrictedEvents.Add(EventType.Store);
                    RestrictedEvents.Add(EventType.Voyager);
                    break;
                case AreaType.Star:
                    RestrictedEvents.Add(EventType.Distress);
                    RestrictedEvents.Add(EventType.Store);
                    RestrictedEvents.Add(EventType.Quest);
                    break;
                default:
                    Debug.LogError("Assign restricted events for this area's gameobject (" + this.gameObject.name + ")");
                    break;
            }
        }

        private void AssignAreaTypeToPoint() {

            for(int i = 0; i < pointsInside.Count; i++) {
                SpriteRenderer pointSR = pointsInside[i].GetComponent<SpriteRenderer>();
                pointSR.color = color;
            }

        }
    }

}