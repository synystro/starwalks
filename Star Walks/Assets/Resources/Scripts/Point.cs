using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {

    public Location location;

    public bool wasVisited;

    public List<GameObject> neighbourPoints = new List<GameObject>();

    private List<GameObject> lines = new List<GameObject>();
    private Vector3[] connectPositions;

    private SectorManager sm;

    private void Start() {

    }

    public void ConnectToNeighbours() {

        connectPositions = new Vector3[neighbourPoints.Count];


        for (int i = 0; i < neighbourPoints.Count; i++) {

            connectPositions[i] = neighbourPoints[i].transform.position;

        }

    }

    private void DestroyLines() {

        for (int i = 0; i < lines.Count; i++)
            Destroy(lines[i]);

        lines.Clear();

    }

    private void EnterLocation() {



    }

    void OnMouseDown() {

        print("clicked!");
        sm = GameObject.FindGameObjectWithTag("SectorManager").GetComponent<SectorManager>();

        foreach(GameObject neighbour in neighbourPoints) {
            if(sm == neighbour) {
                EnterLocation();
            }
        } 

    }

    void OnMouseEnter() {

        for (int i = 0; i < neighbourPoints.Count; i++) {

            // generate object to generate line (using LineRenderer). 
            GameObject line = new GameObject();
            line.transform.SetParent(this.transform);
            lines.Add(line);
            LineRenderer lineRend = line.AddComponent<LineRenderer>();

            // LineRenderer settings.
            lineRend.material = new Material(Shader.Find("Sprites/Default"));
            lineRend.widthMultiplier = 0.02f;
            lineRend.positionCount = 1;
            lineRend.positionCount = 2;
            lineRend.sortingOrder = 1;
            // set start & end positions.
            lineRend.SetPositions(new Vector3[] { transform.position, neighbourPoints[i].transform.position });

        }

    }

    void OnMouseExit() {

        DestroyLines();

    }

}
