using System.Collections.Generic;
using UnityEngine;

public class SectorManager : MonoBehaviour {

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
    public GameObject location;
    [Space(10)]
    [SerializeField]
    private List<string> locationsToSpawn = new List<string>();
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

}
