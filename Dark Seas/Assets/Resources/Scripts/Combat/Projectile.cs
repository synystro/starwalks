using UnityEngine;

namespace DarkSeas {
    [RequireComponent(typeof(Weaponry))]
    public class Projectile : MonoBehaviour {
        public GameObject source;
        [Space(10)]
        public float damage;
        [Space(10)]
        [SerializeField] private float finalDamage;
        [Space(10)]
        public float speed;
        [Space(10)]
        [SerializeField] private float finalSpeed;
        [Space(10)]
        public Weapon weapon;
        [Space(10)]
        public Transform target;
        [Space(10)]
        public GameObject owner;
        [Space(10)]
        public GameObject explosion;
        
        public Vector3 NormalizedDirection { get { return normalizedDirection; } set { normalizedDirection = value; } }

        private Vector3 direction;
        private Vector3 normalizedDirection;
        private Weaponry sourceWeapon;
        private CameraManager camManager;

        void Start() {

            // set weapon attributes
            damage = weapon.damage;
            speed = weapon.shotSpeed;
            direction = target.position - transform.position;

            // set projectile's weapon (source)
            sourceWeapon = source.GetComponent<Weaponry>();

            // get camera
            camManager = Camera.main.GetComponent<CameraManager>();

        }

        void Update() {

            // apply es to damage
            finalDamage = (damage + sourceWeapon.Ship.shipEffects.WeaponsDMG) * (1 + sourceWeapon.Ship.shipEffects.WeaponsDMG_P);
            finalSpeed = (speed + sourceWeapon.Ship.shipEffects.WeaponsSPD) * (1 + sourceWeapon.Ship.shipEffects.WeaponsSPD_P);

            if (target != null) {

                // rotate towards the target.
                RotateTowardsTarget();

                // move towards target
                transform.position = Vector3.MoveTowards(transform.position, target.position, finalSpeed * Time.deltaTime);

                // when target is reached
                if (transform.position == target.position) {

                    if (target.GetComponent<FloorTile>() != null) {

                        FloorTile tile = target.GetComponent<FloorTile>();

                        // random dodge number
                        //int randomDodgeNumber = Random.Range(0, 100);

                        // if hits target
                        //if (randomDodgeNumber > tile.shipManager.evasion) {
                        if (Hit(tile)) {

                            tile.TakeDamage(finalDamage, weapon);
                            // destroy itself
                            Destroy(this.gameObject);

                            sourceWeapon.shotsBeforeHit = 1;

                        } else {
                            //miss
                            target = null;

                            sourceWeapon.shotsBeforeHit++;

                        }

                    } else {
                        Debug.LogError("FloorTile script component is missing on: " + this.name);
                    }

                    sourceWeapon.DestroyAim();

                }

            } else {

                transform.position += normalizedDirection * finalSpeed * Time.deltaTime;
                camManager.CheckIfOutOfBounds(this.gameObject);

            }

        }

        private void RotateTowardsTarget() {

            Vector2 dir = transform.position - target.transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = rotation;


        }

        private void OnDisable() {
            Destroy(this.gameObject);
        }

        private void OnDestroy() {
            // display explosion
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        private bool Hit(FloorTile target) {

            // here we get all the information needed to calculate the damage output to the target
            // gathering the C constant for the shot's pseudo-random probability of hitting its target
            // the finaldamage only refers to the weapon's projectile,
            // the actual damage is calculated on the ship's floor tile considering the ship's armor (damage reduction)

            float shooterMarksmanLevel;

            if(sourceWeapon.handler != null)
                shooterMarksmanLevel = sourceWeapon.handler.crewmemberStats.marksmanLevel;
            else
                shooterMarksmanLevel = 0;

            float accuracy = BattleManager.ProjectileAccuracy(weapon.aim, shooterMarksmanLevel, target.shipManager.evasion);

            decimal c = BattleManager.CfromP((decimal)accuracy);

            float chanceToHit = (float)c * sourceWeapon.shotsBeforeHit;

            int rgn = GameManager.RGN(100);

            print(owner.gameObject.name + " shot " + target.shipManager.gameObject.name + " with " + accuracy + " of accuracy. Chance to hit is " + chanceToHit + ". RGN: " + rgn + " with its " + finalDamage + " of damage");

            return chanceToHit > rgn ? true : false;
        }

    }
}
