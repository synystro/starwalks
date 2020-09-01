using UnityEngine;

namespace DarkSeas
{
    public class Sector : MonoBehaviour
    {

        [SerializeField]
        private Zone zone;

        void Start()
        {

            // assign a random zone for this sector.
            int random = Random.Range(0, GameManager.zones.Count);
            zone = GameManager.zones[random];

        }

        private void OnMouseUp()
        {

            GameManager.currentZone = zone;
            //SceneManager.LoadScene("sector");
            GameManager.GalaxyManager.gameObject.SetActive(false);
            GameManager.SectorManager.gameObject.SetActive(true);

        }
    }
}
