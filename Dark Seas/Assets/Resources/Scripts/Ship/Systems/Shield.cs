using UnityEngine;

namespace DarkSeas
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteMask))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class Shield : ShipSystem
    {

        public float capacity;
        public float bonusCapacity;
        public float integrity;
        [Space(10)]
        public float baseRegenRate;

        private SpriteMask mask;
        private SpriteRenderer sr;
        private Color spriteColor;

        private bool hasStarted;

        private void OnEnable() {

            if(!hasStarted)
                return;

            Start();

        }

        private void AutoPower() {

            // set capacity
            capacity = 100f + bonusCapacity; // make a formula to define this later on

            // set integrity
            if (IsPowered)
                integrity = capacity;
            else if (!Room.IsDamaged) {
                for (int i = 0; i < ship.reactor; i++) {
                    if (powerConsumption < powerCap) {
                        powerConsumption += 1;
                        ship.reactor -= 1;
                    }
                }
                integrity = capacity;
            }

        }

        public override void Start()
        {
            hasStarted = true;

            base.Start();

            AutoPower();

            // get components
            sr = this.GetComponent<SpriteRenderer>();
            mask = this.GetComponent<SpriteMask>();

            // set mask sprite
            mask.sprite = ship.shell.GetComponent<SpriteRenderer>().sprite;

        }

        public override void Update()
        {

            capacity = 100f + bonusCapacity;

            if (IsPowered)
            {
                // recover based on dedicated power and crewmember skill level
                integrity += baseRegenRate * (powerConsumption + crewmemberSkillLevel) * Time.deltaTime;
                if (integrity > capacity)
                    integrity = capacity;
            }
            else
            {
                integrity = 0;
            }

            // change sprite alpha according to integrity of shield
            spriteColor = sr.color;
            spriteColor.a = integrity > 0 ? (integrity / capacity) / 2 : 0;
            sr.color = spriteColor;

            battleUiManager.RefreshShieldlIntegrity();

        }

        private void TakeDamage(Projectile projectile)
        {

            // absorb damage (dedicated power acts as resistance, times five)
            //integrity -= (projectile.damage - (powerConsumption * 5));

            integrity -= projectile.damage;

            // if negative, reduce (absorb) damage and speed, plus make integrity become zero
            if (integrity < 0)
            {
                projectile.damage = Mathf.RoundToInt(integrity) * -1;
                projectile.speed /= 2f;
                integrity = 0;
            }
            else
            {
                Destroy(projectile.gameObject);
            }

        }

        private void OnTriggerEnter2D(Collider2D col)
        {

            if (col.GetComponent<Projectile>())
            {

                Projectile projectile = col.GetComponent<Projectile>();
                if (projectile.weapon.isTorpedo || integrity <= 0)
                    return;
                else if (projectile.owner != ship.gameObject)
                    TakeDamage(projectile);

            }

        }

    }
}
