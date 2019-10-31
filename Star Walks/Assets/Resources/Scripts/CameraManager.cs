using UnityEngine;

public class CameraManager : MonoBehaviour {

    public float zoomSpeed;
    [Space(10)]
    public float maxZoomIn = 5.4f;
    public float maxZoomOut = 10.8f;
    [Space(10)]
    [SerializeField]
    private float scrollValue;
    [Space(20)]
    public float moveCap;
    [Space(10)]
    public float swipeSpeed;
    [Space(10)]
    public float swipeResist;
    [Space(10)]
    public float smoothSpeed;


    private Vector3 targetPos;
    private Vector3 touchPos;
    private Camera cam;

    private void Start() {

        scrollValue = 0f;

        cam = GetComponent<Camera>();

    }

    private void Update() {

        CheckForZoomInput();

        CheckForSwipe();

    }

    private void FixedUpdate() {

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

    }

    private void CheckForZoomInput() {

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {

            // get scroll value.
            scrollValue = Input.GetAxis("Mouse ScrollWheel");

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

    private void CheckForSwipe() {

        if (Input.GetMouseButtonDown(0))
            touchPos = Input.mousePosition;

        if (Input.GetMouseButtonUp(0)) {

            // dividing by 100 so it matches unit sizing (i.e. 100f = 1 unit).
            Vector3 swipeForce = (touchPos - Input.mousePosition) / 100f;

            if (Mathf.Abs(swipeForce.x) < swipeResist && Mathf.Abs(swipeForce.y) < swipeResist)
                return;

            if (Mathf.Abs(swipeForce.x) - transform.position.x > moveCap) {
                if (swipeForce.x < 0) {
                    swipeForce.x = -moveCap;
                } else {
                    swipeForce.x = moveCap;
                }

            }

            if (Mathf.Abs(swipeForce.y) - transform.position.y > moveCap) {
                if (swipeForce.y < 0) {
                    swipeForce.y = -moveCap;
                } else {
                    swipeForce.y = moveCap;
                }

            }

            targetPos.x = transform.position.x + swipeForce.x * swipeSpeed;
            targetPos.y = transform.position.y + swipeForce.y * swipeSpeed;

        }

    }

}
