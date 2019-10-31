using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Crew : MonoBehaviour {

    [Header("Path Finding")]
    private bool isDestinationSet;
    private int destinationIndex = 0;
    private Vector3 tapPos;
    private PathFinding path;
    private List<Vector2> destinations;

    private Rigidbody2D rb;
    private BoxCollider2D col;

    public float health = 100f;
    public float speed = 3f;
    public bool cantMove = false;

    [HideInInspector]
    //public float horizontalMove = 0f;
    //public float verticalMove = 0f;
    public float curSpeed;

    private void Start() {

        // getting components.
        path = GetComponent<PathFinding>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        // initialize destinations list.
        destinations = new List<Vector2>();

        // set current speed to default speed.
        curSpeed = speed;

        isDestinationSet = false;

    }

    public virtual void Update() {

        ProcessInputs();
    }

    void FixedUpdate() {

            #region Follow Path

            if (destinations.Count < 1)
                return;

            if (destinationIndex < destinations.Count) {
                if (destinations[destinationIndex] != (Vector2)transform.position) {
                    transform.position = Vector3.MoveTowards(transform.position, destinations[destinationIndex], curSpeed * Time.deltaTime);
                } else {
                    destinationIndex++;
                }
            }

            if ((Vector2)transform.position == destinations[destinations.Count - 1]) {
                ClearDestination();
            }

            #endregion

    }

    void ProcessInputs() {

        CheckTap();
        Movement();

    }

    void CheckTap() {

        if (!EventSystem.current.IsPointerOverGameObject()) {

            if (Input.GetMouseButtonDown(0)) {

                ClearDestination();

                // get tap screen to world position.
                tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // find path.
                path.FindPath(transform.position, tapPos);

            }
        }
    }

    void Movement() {
        // chekc if cannot move.
        if (cantMove) {
            curSpeed = 0;
            return;
        }

        // set destination
        if (!isDestinationSet && path.FinalPath != null)
            StartCoroutine("SetDestination");

    }

    public void ToggleCanMove() {

        cantMove = !cantMove;

        if (cantMove) {
            rb.isKinematic = true;
        } else {
            rb.isKinematic = false;
        }

    }

    IEnumerator SetDestination() {

        foreach (Node node in path.FinalPath)
            destinations.Add(node.position);

        isDestinationSet = true;

        yield return null;

    }

    void ClearDestination() {

        // erase entire path.
        path.FinalPath = null;
        // destination is not set anymore;
        isDestinationSet = false;
        // clear destination list.
        destinations.Clear();
        // reset destination index.
        destinationIndex = 0;

    }

}