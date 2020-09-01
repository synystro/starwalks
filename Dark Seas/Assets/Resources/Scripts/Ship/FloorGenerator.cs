using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    [RequireComponent(typeof(CustomGrid))]
    public class FloorGenerator : MonoBehaviour
    {

        public GameObject tilePF;

        public GameObject ship;

        public GameObject gridGO;

        public List<GameObject> tiles = new List<GameObject>();

        private List<Vector3> tilesPos = new List<Vector3>();

        [SerializeField]
        private int numberOfNodes;

        private CustomGrid grid;
        private Ship shipManager;

        [SerializeField]
        private Vector3 startingPos;

        void Start()
        {

            startingPos = Vector3.zero;

            // get grid component.
            grid = gridGO.GetComponent<CustomGrid>();

            // get ship manager.
            if (ship.GetComponent<Ship>() != null)
                shipManager = ship.GetComponent<Ship>();
            else
                Debug.LogError("Ship (script) component is missing on GO: " + this.name);

            // get number of tiles.
            numberOfNodes = grid.GetGridSize;

            // set starting position.
            Vector3 gridTopLeft = new Vector3(-grid.GetGridSizeX / 2, grid.GetGridSizeY / 2, startingPos.z);
            startingPos = gridTopLeft;

            // set impassable tiles.
            CalculateImpassable();

            // start generating tiles.
            GenerateTiles();

            // align it with the ship.
            //this.transform.localPosition = ship.transform.position + grid.GetGridOffset;
            this.transform.localPosition = ship.transform.position;

        }

        private void CalculateImpassable()
        {

            bool isPreviousWall = false;
            bool isAfterWall = false;
            Vector3 wallStartPos = Vector3.zero;
            Vector3 wallEndPos = Vector3.zero;
            Vector3 currentPos;
            currentPos = startingPos;

            for (int i = 0; i < grid.GetGridSizeY; i++)
            {

                List<Vector3> rowTiles = new List<Vector3>();

                for (int j = 0; j < grid.GetGridSizeX; j++)
                {

                    if (isPreviousWall && !isAfterWall)
                    {

                        if (grid.NodeFromWorldPosition(currentPos).isWalkable)
                        {
                            wallStartPos = currentPos;
                            rowTiles.Add(currentPos);
                            isAfterWall = true;
                            isPreviousWall = false;
                        }

                    }
                    else if (isAfterWall)
                    {

                        if (grid.NodeFromWorldPosition(currentPos).isWalkable)
                        {
                            rowTiles.Add(currentPos);
                        }
                        else
                        {

                            // run through the entirety of its X axis' Y axis to check for walls. if it finds walls on both extremeties then the tile is within walls.
                            foreach (Vector3 tilePosition in rowTiles)
                            {
                                for (int k = (int)tilePosition.y; k < (grid.GetGridSizeY - k); k++)
                                {
                                    if (!grid.NodeFromWorldPosition(new Vector3(tilePosition.x, k, tilePosition.z)).isWalkable)
                                    {
                                        for (int l = (int)tilePosition.y; l > (-grid.GetGridSizeY - l); l--)
                                        {
                                            if (!grid.NodeFromWorldPosition(new Vector3(tilePosition.x, l, tilePosition.z)).isWalkable)
                                                tilesPos.Add(tilePosition);
                                        }
                                    }
                                }
                            }

                            isAfterWall = false;
                            isPreviousWall = true;
                        }

                    }
                    else
                    {

                        if (grid.NodeFromWorldPosition(currentPos).isWalkable)
                            isPreviousWall = false;
                        else
                            isPreviousWall = true;

                    }

                    currentPos = new Vector3(
                        currentPos.x + 1f,
                        currentPos.y,
                        currentPos.z
                        );

                }

                isPreviousWall = false;
                isAfterWall = false;
                wallStartPos = Vector3.zero;
                wallEndPos = Vector3.zero;

                currentPos = new Vector3(
                        startingPos.x,
                        currentPos.y - 1f,
                        startingPos.z
                        );


            }
        }

        private void GenerateTiles()
        {

            foreach (Vector3 tilePos in tilesPos)
                InstantiateTile(tilePos);

        }

        private void InstantiateTile(Vector3 currentPos)
        {

            // instantiate.
            GameObject tile = Instantiate(tilePF, currentPos, Quaternion.identity);
            // group up.
            tile.transform.SetParent(this.transform);
            // set tile hp and armor.
            FloorTile tileScript = tile.GetComponent<FloorTile>();
            tileScript.hp = shipManager.tileHP;
            // pass ship (manager) reference.
            tileScript.shipManager = shipManager;
            // add tile to the list of tiles.
            tiles.Add(tile);

        }

    }
}
