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

    public GameObject shipSilhouettePrefab;

    public string zone;


    void Start() {

    }

    public void SpawnShipSilhouette(Vector2 position) {

        shipSilhoette = Instantiate(shipSilhouettePrefab, position, Quaternion.identity);

    }
}
