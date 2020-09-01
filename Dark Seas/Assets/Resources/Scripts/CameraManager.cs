using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkSeas {

    public class CameraManager : MonoBehaviour {

        public delegate void OnZoomChanged();
        public static event OnZoomChanged onZoomChangedCallback;

        public BoxCollider2D locationBounds;
        [Space(10)]
        public bool isLocked;
        [Space(10)]
        public float zoomSpeed;
        [Space(10)]
        public float maxZoomIn = 5.4f;
        public float maxZoomOut = 21.6f;
        [Space(10)]
        [SerializeField]
        public float scrollValue;
        [Space(20)]
        public float moveCap;
        [Space(10)]
        public float swipeSpeed;
        [Space(10)]
        public float swipeResist;
        [Space(10)]
        public float smoothSpeed;

        private Vector2 panDirection;
        [SerializeField] private Vector2 tapStart;
        [SerializeField] private Vector2 currentFingerPos;

        private Vector3 minBounds;
        private Vector3 maxBounds;
        private float camHalfHeight;
        private float camHalfWidth;

        private Camera cam;

        private void Awake() {

            cam = this.GetComponent<Camera>();

        }

        private void Start() {

            scrollValue = 0f;

            // get location bounds
            minBounds = locationBounds.bounds.min;
            maxBounds = locationBounds.bounds.max;

        }

        private void Update() {            

            // get camera's current half height and width
            camHalfHeight = cam.orthographicSize;
            camHalfWidth = camHalfHeight * Screen.width / Screen.height;

            if (isLocked)
                return;

            CheckForZoomInput();

            //CheckForSwipe();

            CheckForTap();

            if (locationBounds != null)
                Clamp();

        }

        private void Clamp() {


            float clampedX = Mathf.Clamp(transform.position.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
            float clampedY = Mathf.Clamp(transform.position.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        }

        private void CheckForZoomInput() {

            if (Input.GetAxis("Mouse ScrollWheel") != 0) {

                // get scroll value.
                scrollValue = Input.GetAxis("Mouse ScrollWheel");

                if(scrollValue != 0)
                    onZoomChangedCallback?.Invoke();

                // invert axis and mutiply it by zoom speed.
                scrollValue = (scrollValue * -1) * zoomSpeed;

                // set camera zoom.
                cam.orthographicSize += scrollValue;

                // cap zoom amount to a min and a max.
                if (cam.orthographicSize < maxZoomIn)
                    cam.orthographicSize = maxZoomIn;
                else if (cam.orthographicSize > maxZoomOut)
                    cam.orthographicSize = maxZoomOut;

            }

        }

        private void CheckForTap() {

            if (Input.GetMouseButtonDown(0)) {

                tapStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            }

            if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
                panDirection = tapStart - (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position += (Vector3)panDirection;
            }

        }

        private void CheckForSwipe() {
            
            // mouse 0 click.
            if (Input.GetMouseButtonDown(0))
                tapStart = Input.mousePosition;
            // mouse 0 release.
            if (Input.GetMouseButtonUp(0)) {

                // dividing by 100 so it matches unit sizing (i.e. 100f = 1 unit).
                Vector3 swipeForce = (tapStart - (Vector2)Input.mousePosition) / 100f;

                // check if swipe force is less than swipe resist
                if (Mathf.Abs(swipeForce.x) < swipeResist && Mathf.Abs(swipeForce.y) < swipeResist)
                    return;

                // cap max camera slide movement on x axis.
                if (Mathf.Abs(swipeForce.x) - transform.position.x > moveCap) {
                    if (swipeForce.x < 0) {
                        swipeForce.x = -moveCap;
                    } else {
                        swipeForce.x = moveCap;
                    }

                }

                // cap max camera slide movement on y axis.
                if (Mathf.Abs(swipeForce.y) - transform.position.y > moveCap) {
                    if (swipeForce.y < 0) {
                        swipeForce.y = -moveCap;
                    } else {
                        swipeForce.y = moveCap;
                    }

                }

                // get desired screen target position.
                panDirection.x = transform.position.x + swipeForce.x * swipeSpeed;
                panDirection.y = transform.position.y + swipeForce.y * swipeSpeed;

            }

        }

        private void OnEnable() {

            cam.orthographicSize = maxZoomOut;

        }

        public void Clamp(GameObject go) {


            float clampedX = Mathf.Clamp(go.transform.position.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
            float clampedY = Mathf.Clamp(go.transform.position.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);

            go.transform.position = new Vector3(clampedX, clampedY, transform.position.z);

        }

        public void CheckIfOutOfBounds(GameObject go) {


            if (go.transform.position.x < minBounds.x || go.transform.position.x > maxBounds.x)
                Destroy(go);
            else if(go.transform.position.y < minBounds.y || go.transform.position.y > maxBounds.y)
                Destroy(go);

        }

    }

}
