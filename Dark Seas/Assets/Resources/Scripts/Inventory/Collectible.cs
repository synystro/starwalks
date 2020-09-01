using UnityEngine;

namespace DarkSeas
{
    public class Collectible : MonoBehaviour
    {

        public int fuel;
        public int ammo;
        public int scraps;

        public Item item;

        private void OnMouseDown()
        {

            if (fuel > 0)
            {
                GameManager.ship.fuel += fuel;
                Destroy(this.gameObject);
                return;
            }
            if (ammo > 0)
            {
                GameManager.ship.ammo += ammo;
                Destroy(this.gameObject);
                return;
            }
            if (scraps > 0)
            {
                GameManager.ship.scraps += scraps;
                Destroy(this.gameObject);
                return;
            }

            if (item != null)
            {
                GameManager.ship.ReceiveItem(item);
                Destroy(this.gameObject);
                return;
            }

        }

        private void OnDisable()
        {
            Destroy(this.gameObject);
        }

    }
}
