using UnityEngine;

namespace DarkSeas
{
    public class AimIcon : MonoBehaviour
    {

        public GameObject expectedProjectile;

        private string expectedProjectileName;

        private void Start()
        {
            expectedProjectileName = string.Concat(expectedProjectile.name, "(Clone)");
        }

        private void Update()
        {
            if (!GameManager.InBattle)
                Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.name == expectedProjectileName)
                Destroy(this.gameObject);
        }

    }
}
