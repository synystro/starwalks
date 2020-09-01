using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    public class WeaponsRoom : Room
    {
        public List<Weapon> weaponsEquipped = new List<Weapon>();

        [SerializeField] private List<GameObject> weapons = new List<GameObject>();

        public void Setup() {

            // set room hp based on system power consumption
            for (int i = 0; i < weaponsEquipped.Count; i++) {

                Weapon weapon = weaponsEquipped[i];
                GameObject weaponGO = new GameObject();
                weaponGO.name = weapon.name;
                weaponGO.transform.position += shipManager.transform.position + (Vector3)shipManager.weaponSlotsPositions[i];
                weaponGO.transform.eulerAngles -= shipManager.transform.eulerAngles;
                weaponGO.transform.SetParent(shipManager.arsenal.transform);
                weaponGO.AddComponent<SpriteRenderer>();
                Weaponry weaponry = weaponGO.AddComponent<Weaponry>();
                weaponry.weapon = weaponsEquipped[i];
                weaponry.hp = weaponry.weapon.powerConsumption;
                maxHP += weaponry.hp;

                weapons.Add(weaponGO);

            }

            if (shipManager.isPlayer) {
                GameManager.GetShipWeapons();
                GameManager.BattleUiManager.SetupWeapons();
            }

            // rotate system go according to the ship's rotation so weapons can be positioned properly
            systemGO.transform.localRotation = shipManager.body.transform.localRotation;

            currentHP = maxHP;

        }

        public void AttachWeapon(Weapon weapon) {
            if(weapon == null) {
                Debug.LogError("Weapon not passed (needed in order to attach it).");
                return;
            }
            GameObject weaponGO = new GameObject();
            weaponGO.name = weapon.name;
            //FIXME give a proper position for this weapon (I think it shoots from there and it's bad if it's all )
            //weaponGO.transform.position += shipManager.transform.position + (Vector3)shipManager.weaponSlotsPositions[i];
            weaponGO.transform.eulerAngles -= shipManager.transform.eulerAngles;
            weaponGO.transform.SetParent(shipManager.arsenal.transform);
            weaponGO.AddComponent<SpriteRenderer>();
            Weaponry weaponry = weaponGO.AddComponent<Weaponry>();
            weaponry.weapon = weapon;
            weaponry.hp = weaponry.weapon.powerConsumption;
            maxHP += weaponry.hp;
            currentHP += weaponry.hp;

            weaponsEquipped.Add(weapon);
            weapons.Add(weaponGO);     

            GameManager.GetShipWeapons();
            GameManager.BattleUiManager.SetupWeapons();

            //weaponry.RefreshWeaponsSystem();
            
            if(GameManager.InBattle)
                    GameManager.BattleUiManager.RefreshWeaponsPowerUsage();
        }

        public void DetachWeapon(Weapon weapon) {
            if(weapon == null) {
                Debug.LogError("Weapon not passed (needed in order to detach it).");
                return;
            }
            if(weaponsEquipped.Contains(weapon))
                weaponsEquipped.Remove(weapon);
            else
                Debug.LogError("Weapon to detach isn't part of equipped weapons.");
            GameManager.GetShipWeapons();
            GameManager.BattleUiManager.SetupWeapons(); 
        }

        public override void Start()
        {

            base.Start();

            Setup();

        }
        public override void Update()
        {
            base.Update();
        }

        // receive damage from one of this room's tile
        public override void TakeDamage(float damage)
        {

            base.TakeDamage(damage);

            int roomDamage = Mathf.RoundToInt(damage / 10);

            if (currentHP <= maxHP)
            {

                for (int i = 0; i < weapons.Count; i++)
                {

                    Weaponry weaponManager = weapons[i].GetComponent<Weaponry>();

                    // if weapon is powered and there's still damage to apply
                    if (roomDamage > 0)
                    {

                        // if was powered return weapon's power back to the ship's reactor
                        if (weaponManager.IsPowered)
                            shipManager.reactor += weaponManager.powerConsumption;
                        // damage weapon
                        // decrease hp according to room dmg
                        int previousHP = weaponManager.hp;
                        weaponManager.hp -= roomDamage;
                        if (weaponManager.hp < 0)
                            weaponManager.hp = 0;
                        // reduce room damage according to the weapon's power consumption (for the possible next weapon damage)
                        roomDamage -= previousHP - weaponManager.hp;
                        // set weapon power consumption to zero
                        weaponManager.powerConsumption = 0;
                        // refresh weapons system power consumption and capacity
                        //weaponManager.RefreshWeaponsSystem();

                    }

                }

            }

            // refresh power usage
            GameManager.BattleUiManager.RefreshPowerUsage();

        }

        // repair system with a crew member or robot
        public override void RepairOneBar()
        {
            //int hpRecovered = 0;

            for (int i = 0; i < weapons.Count; i++) {
                Weaponry weapon = weapons[i].GetComponent<Weaponry>();
                if (weapon.hp < weapon.powerCap) {
                    weapon.hp += 1;
                    //hpRecovered = 1;
                    break;
                }
            }

            currentHP++;

            if (currentHP > maxHP)
                currentHP = maxHP;

            GameManager.BattleUiManager.RefreshPowerUsage();

        }

    }
}
