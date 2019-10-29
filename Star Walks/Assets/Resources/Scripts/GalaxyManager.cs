using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class GalaxyManager : MonoBehaviour {

    public GameManager gm;
    public Transform ship;
    public Transform sectorsGroup;

    public GameObject sectorPrefab;

    [SerializeField]
    private List<Transform> sectors = new List<Transform>();

    void Start() {

        // scan sectors.
        ScanSectors();

        // sort sectors.
        SortSectors();

    }

    void Update() {

        /*
        if(Input.GetMouseButtonDown(0)) {

            Camera cam = Camera.main;

            Vector2 tapPos = cam.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(tapPos, -Vector2.up);

            if (hit.collider != null) {
                if (hit.collider.gameObject.tag == "Sector") {

                    Sector sector = hit.collider.gameObject.GetComponent<Sector>();
                    // confirmation?
                    EnterSector(sector);

                }

            } else {
                print("nothing hit");
            }

        }
        */

    }

    private void ScanSectors() {

        Vector2 pos = ship.position;

        Vector2 offset1 = new Vector2(pos.x -1, pos.y -1);
        Vector2 offset2 = new Vector2(pos.x -1, pos.y +1);
        Vector2 offset3 = new Vector2(pos.x +1, pos.y -1);
        Vector2 offset4 = new Vector2(pos.x +1, pos.y +1);

        RaycastHit2D hit1 = Physics2D.Raycast(pos, -Vector2.up);
        RaycastHit2D hit2 = Physics2D.Raycast(offset1, -Vector2.up);
        RaycastHit2D hit3 = Physics2D.Raycast(offset2, -Vector2.up);
        RaycastHit2D hit4 = Physics2D.Raycast(offset3, -Vector2.up);
        RaycastHit2D hit5 = Physics2D.Raycast(offset4, -Vector2.up);

        if (hit1.collider == null)
            SpawnSector(pos);

        if (hit2.collider == null)
            SpawnSector(offset1);

        if (hit3.collider == null)
            SpawnSector(offset2);

        if (hit4.collider == null)
            SpawnSector(offset3);

        if (hit5.collider == null)
            SpawnSector(offset4);

    }

    private void SpawnSector(Vector2 position) {

        GameObject sector = Instantiate(sectorPrefab, position, Quaternion.identity);
        sector.name = position.x.ToString() + position.y.ToString();
        sector.transform.SetParent(sectorsGroup);

    }

    private void SortSectors() {
        foreach (Transform go in sectorsGroup) {
            sectors.Add(go);
        }
    }

}
