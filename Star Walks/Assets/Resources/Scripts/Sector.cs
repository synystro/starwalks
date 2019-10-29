using UnityEngine;
using UnityEngine.SceneManagement;

public class Sector : MonoBehaviour {

    public GameManager gm;
    public Zone Control { get => control; set => control = value; }

    [SerializeField]
    private Zone control;

    public enum Zone { Federation, Mercenary, AI }

    void Start() {

        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        int randomControIndex = Random.Range(0, 3);

        switch (randomControIndex) {

            case 0:
                control = Zone.Federation;
                break;
            case 1:
                control = Zone.Mercenary;
                break;
            case 2:
                control = Zone.AI;
                break;

        }

    }

    private void OnMouseDown() {

        gm.zone = control.ToString();
        SceneManager.LoadScene("sector");

    }
}
