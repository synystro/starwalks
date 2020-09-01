using UnityEngine;

namespace DarkSeas {

    public static class EquipmentManager {

        public static bool EquipWeapon(Weapon weapon) {
            if(weapon == null) {
                Debug.LogError("You're trying to equip a weapon that apparently never existed.");
                return false;
            }

            if(GameManager.ship.WeaponsRoom.weaponsEquipped.Count >= GameManager.ship.weaponsCapacity) {
                Debug.Log(GameManager.ship.name + "'s equipped weapons capacity is at its limit");
                return false;
            } else {
                GameManager.ship.WeaponsRoom.AttachWeapon(weapon);
                return true;
            }
        }

        public static bool UnequipWeapon(Weapon weapon) {
            if(weapon == null) {
                Debug.LogError("You're trying to unequip a weapon that apparently never existed.");
                return false;
            }
            if(GameManager.ship.WeaponsRoom.weaponsEquipped.Contains(weapon)) {
                GameManager.ship.WeaponsRoom.weaponsEquipped.Remove(weapon);
                if(GameManager.InBattle)
                    GameManager.BattleUiManager.RefreshWeaponsPowerUsage();
                return true;
            }
            else {
                Debug.LogError("The weapon you're trying to unequip apparently is NOT currently equipped.");
                return false;
            }
            
        }        

    }

}
