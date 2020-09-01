using UnityEngine;

namespace DarkSeas
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
    public class Weapon : ScriptableObject
    {

        public int id;
        public new string name;
        public Sprite icon;
        [Space(10)]
        public GameObject shotPrefab;
        [Space(10)]
        public int powerConsumption;
        [Space(10)]
        public int cooldown;
        [Space(10)]
        public int aim;
        [Space(10)]
        public bool isTorpedo;
        [Space(10)]
        public bool startsOffCD;
        [Space(10)]
        [Header("Projectile")]
        [Space(10)]
        public int torpedosRequired;
        [Space(10)]
        public float shotSpeed;
        [Space(10)]
        public float damage;
        public float pierce;
        public float breachChance;
        public float fireChance;

    }
}
