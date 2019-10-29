using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    #region singleton

    private void Awake() {

        DontDestroyOnLoad(this);

        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

    }

    #endregion

    public GameObject shipSilhoette;

    public string zone;


    void Start() {

    }

    public void SetShipSilhouette(Vector2 _position) {

        SectorManager sm = GameObject.FindGameObjectWithTag("SectorManager").GetComponent<SectorManager>();
        shipSilhoette = sm.shipSilhoette;

        shipSilhoette.transform.position = _position;

    }
}
