using System.Collections.Generic;
using UnityEngine;

public class SectorManager : MonoBehaviour {

    public GameObject shipSilhoette;
    [Space(10)]
    public GameObject currentPoint;
    [Space(10)]
    [SerializeField]
    private string zone;
    [Space(10)]
    public int pointsAmount;
    [Space(10)]
    public int pointsMinAmount;
    public int pointsMaxAmount;
    [Space(10)]
    public GameObject sector;
    public GameObject locationPrefab;
    [SerializeField]
    private GameObject currentLocation;
    [Space(10)]
    public List<Location> locationsToSpawn = new List<Location>();
    [Space(10)]
    public GameObject entrance;
    public GameObject exit;

    private GameManager gm;

    private void Awake() {
        // set amount of points on this sector.
        pointsAmount = Random.Range(pointsMinAmount, pointsMaxAmount);
    }

    void Start() {

        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        zone = gm.zone;

    }

    public void EnterLocation(Location _location, GameObject _currentPoint) {

        currentLocation = Instantiate(locationPrefab, transform.position, Quaternion.identity);
        currentPoint = _currentPoint;

        currentLocation.GetComponent<LocationManager>().location = _location;

        // move ship silhoette to new point.
        shipSilhoette.transform.position = currentPoint.transform.position;

        // deactivate sector map to enter location.
        sector.SetActive(false);

    }


}
