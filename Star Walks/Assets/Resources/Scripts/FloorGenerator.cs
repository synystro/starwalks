using System;
using UnityEngine;

public class FloorGenerator : MonoBehaviour {

    public GameObject tilePF;

    public GameObject gridGO;

    [SerializeField]
    private int numberTiles;

    private CustomGrid grid;

    [SerializeField]
    private Vector3 startingPos;
    private float offsetX = 0f;
    private float offsetY = 0f;

    void Start() {

        // get grid component.
        grid = gridGO.GetComponent<CustomGrid>();

        // get number of tiles.
        numberTiles = grid.GetGridSize;

        DefineOffset();

        // set starting position.
        startingPos =
            new Vector3(-(grid.GetGridSizeX / 2) + offsetX, (grid.GetGridSizeY / 2) - offsetY, 0)
            +
            this.transform.position
            ;
        // apply starting position.
        this.transform.position = startingPos;

        // start generating tiles.
        GenerateTiles();

    }

    private void GenerateTiles() {

        Vector3 currentPos;
        currentPos = startingPos;
        Vector3 tileDistanceX = new Vector3(1f, 0f, 0f);
        Vector3 tileDistanceY = new Vector3(0f, 1f, 0f);

        for (int i = 0; i < grid.GetGridSizeY; i++) {

            for (int j = 0; j < grid.GetGridSizeX; j++) {

                if (!grid.NodeFromWorldPosition(currentPos).isWall) {

                    GameObject tile = Instantiate(tilePF, currentPos, Quaternion.identity);
                    tile.transform.SetParent(this.transform);

                }

                    //currentPos = currentPos + tileDistanceX;
                    currentPos = new Vector3(
                        currentPos.x + 1f,
                        currentPos.y,
                        currentPos.z
                        );

            }

            //currentPos = currentPos + tileDistanceY;
            currentPos = new Vector3(
                    startingPos.x,
                    currentPos.y - 1f,
                    startingPos.z
                    );

        }
    }

    private void DefineOffset() {

        if (grid.GetGridSizeX % 2 == 0)
            offsetX = 0.5f;

        if (grid.GetGridSizeY % 2 == 0)
            offsetY = 0.5f;

    }

}
