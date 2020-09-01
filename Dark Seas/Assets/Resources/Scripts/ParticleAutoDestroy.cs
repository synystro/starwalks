using UnityEngine;

namespace DarkSeas
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleAutoDestroy : MonoBehaviour
    {

        private ParticleSystem ps;

        public void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        public void Update()
        {
            if (ps & !ps.IsAlive())
                Destroy(gameObject);
        }

    }
}