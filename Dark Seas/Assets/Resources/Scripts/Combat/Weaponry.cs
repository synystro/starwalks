using UnityEngine;
namespace DarkSeas {
    public class Weaponry : ShipSystem {

        private const int TARGET_LAYER_INT = 9;

        public int hp;
        [Space(10)]
        public Weapon weapon;
        [Space(10)]
        public int Dmg;
        [Space(10)]
        [SerializeField] private float cooldown;
        [Space(10)]
        public bool isSelected;
        public bool OnCooldown { get { return cooldown > 0 ? true : false; } }
        public new bool IsDamaged { get { return hp < powerCap ? true : false; } }
        [Space(10)]
        [SerializeField] private Transform target;
        [Space(10)]
        [SerializeField] private Weapons weaponsSystem;
        [Space(10)]
        public int shotsBeforeHit = 1;

        private GameObject aimIconGO;
        private LayerMask targetLayerMask;

        public float WeaponCooldown { get { return weapon.cooldown; } }
        public float CurrentCooldownTime { get { return cooldown; } }
        public float CurrentRechargeRate { get { return (Time.deltaTime + ship.shipEffects.WeaponsRR) * ( 1 + ship.shipEffects.WeaponsRR_P); } }
        public Ship Ship { get { return ship;} }

        // const
        //private const int TARGET1_VALUE = 8;
        //private const int TARGET2_VALUE = 9;

        private void AISetup() {

            // power up weapon
            powerConsumption = weapon.powerConsumption;

        }

        public override void Start()
        {

            base.Start();

            // set target layermask
            targetLayerMask = LayerMask.GetMask("Aimable");

            // set power cap
            powerCap = weapon.powerConsumption;
            // set hp
            hp = powerCap;
            // get weapon's cd
            if (!weapon.startsOffCD)
                cooldown = weapon.cooldown;
            else
                cooldown = 0f;

            // get ship
            ship = GetComponentInParent<Ship>();

            // get weapon system's component
            GetWeaponsSystem();

            Room = weaponsSystem.Room;
            handler = weaponsSystem.handler;

            // get battle UI manager
            battleUiManager = GameManager.BattleUiManager;

            // AI setup
            if (ship.isHostile)
                AISetup();

        }

        public override void Update()
        {
            // refresh handler
            handler = weaponsSystem.handler;

            // if no power or damage
            if (!IsPowered || IsDamaged)
            {
                // reset cooldown
                cooldown = weapon.cooldown;
                // unselect weapon
                if (isSelected)
                    isSelected = false;
                // skip everything else
                return;
            }

            // cooldown system
            if (OnCooldown)
            {
                cooldown -= CurrentRechargeRate;
                if (cooldown < 0f)
                {
                    cooldown = 0f;
                }
            }

            // if selected and is player
            if (isSelected && !ship.isHostile)
            {

                if (Input.GetMouseButtonDown(0))
                {

                    if (weapon.isTorpedo && ship.ammo < weapon.torpedosRequired)
                    {
                        Debug.LogWarning(ship.name + " has not enough ammo to fire its " + weapon.name + ". It requires " + weapon.torpedosRequired + " torpedo(es) and the ship has " + ship.ammo);
                        isSelected = false;
                        return;
                    }

                    SetTarget();

                }

            }

            // if not on cooldown and target is set: shoot
            if (!OnCooldown && target != null)
                Shoot();

        }

        private void GetWeaponsSystem() {
            if(weaponsSystem == null)
                weaponsSystem = GetComponentInParent<Weapons>();
        }

        private void SetTarget() {

            Vector2 tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(tapPos, -Vector2.up, Mathf.Infinity, targetLayerMask);

            if(hit.collider == null) {
                isSelected = false;
                return;
            }

            if (hit.collider.CompareTag("Room")) {

                Transform room = hit.transform;                         

                // check if target is hostile.
                if (room.GetComponent<Room>().Ship.isHostile) {

                    int numberOfTiles = hit.transform.childCount;
                    GameObject randomRoomTileGO = hit.transform.GetChild(GameManager.RGN(numberOfTiles)).gameObject;

                    target = randomRoomTileGO.transform;

                    // put aim icon on the room
                    PlaceAim(room);
                    isSelected = false;

                } else {
                    // unselect if not hostile.
                    isSelected = false;

                }

            } else {
                // unselect if not targetable.
                isSelected = false;

            }

            #region old tile targeting

            // RaycastHit hit;
            // var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // // if some collider is hit.
            // if (Physics.Raycast(ray, out hit)) {
            //     print(hit.collider.gameObject.name);

            //     // check if collider is targetable.
            //     //if (hit.collider.gameObject.layer == targetLayer1 || hit.collider.gameObject.layer == targetLayer2)
            //     if (hit.collider.CompareTag("FloorTile")) {

            //         // check if target is hostile.
            //         if (hit.collider.GetComponent<FloorTile>().shipManager.isHostile) {
            //             // set target and shoot if hostile.
            //             target = hit.collider.transform;

            //             // put aim icon on target.
            //             PlaceAim();
            //             isSelected = false;

            //         } else {
            //             // unselect if not hostile.
            //             isSelected = false;

            //         }

            //     } else {
            //         // unselect if not targetable.
            //         isSelected = false;

            //     }

            // } else {
            //     // unselect if clicked on nothing.
            //     isSelected = false;
            // }

            #endregion

        }

        public void Shoot()
        {

            if (weapon.isTorpedo)
            {
                if (ship.ammo < weapon.torpedosRequired)
                {
                    Debug.LogWarning(ship.name + " has not enough ammo to fire its " + weapon.name + ". It requires " + weapon.torpedosRequired + " torpedo(es) and the ship has " + ship.ammo);
                    isSelected = false;
                    return;
                }
                ship.ammo -= weapon.torpedosRequired;
            }
            
            GameObject projectileGO = Instantiate(weapon.shotPrefab, transform.position, Quaternion.identity);
            projectileGO.transform.SetParent(GameManager.LocationManager.transform);
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            projectile.NormalizedDirection = (target.position - projectile.transform.position).normalized;
            projectile.target = target;
            projectile.source = this.gameObject;
            projectile.owner = ship.gameObject;
            projectile.weapon = weapon;
            cooldown = weapon.cooldown;
            target = null;
            isSelected = false;

        }

        public void ToggleSelectWeapon()
        {

            if (!IsDamaged)
                isSelected = !isSelected;
            else
                print(this.name + " is damaged!");

        }

        public void SetTarget(Transform _target)
        {
            target = _target;
        }

        public void PlaceAim(Transform target)
        {
            Destroy(aimIconGO);
            Vector3 targetPos = target.GetComponent<BoxCollider2D>().bounds.center;
            aimIconGO = Instantiate(battleUiManager.aimIconPrefab, targetPos, Quaternion.identity);
            SpriteRenderer aimIconSR = aimIconGO.GetComponent<SpriteRenderer>();
            aimIconSR.sortingOrder = 100;
            aimIconGO.transform.SetParent(this.transform);
            aimIconGO.GetComponent<AimIcon>().expectedProjectile = weapon.shotPrefab;

        }

        public void DestroyAim() {
            Destroy(aimIconGO);
        }

        public void TogglePower()
        {

            // if damaged, skip
            if (IsDamaged) {
                print("weapon " + weapon.name + " is damaged!");
                return;
            }

            if (!IsPowered)
            {
                if (GameManager.ship.reactor >= weapon.powerConsumption)
                {
                    GameManager.ship.reactor -= weapon.powerConsumption;
                    powerConsumption = weapon.powerConsumption;
                }
            }
            else
            {
                GameManager.ship.reactor += weapon.powerConsumption;
                powerConsumption = 0;
            }

            // refresh weapons system's power consumption and capacity
            //RefreshWeaponsSystem();

            // set weapon power usage to UI
            battleUiManager.RefreshPowerUsage();

        }

        // public void RefreshWeaponsSystem() {
        //     GetWeaponsSystem();
        //     weaponsSystem.Refresh();
        // }

    }
}
