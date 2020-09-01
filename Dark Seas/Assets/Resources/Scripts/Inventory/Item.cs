using UnityEngine;
using System.Collections.Generic;

namespace DarkSeas {

    [System.Serializable]
    public class Effect {

        public enum att { None,
            CommandEvasion,
            EngineEvasion,
            EngineRechargeRate,
            OxygenRechargeRate,
            ShieldCapacity,
            ShieldRechargeRate,
            WeaponsDamage,
            WeaponsSpeed,
            WeaponsRechargeRate,
            MedbayHealingRate,
            CloakingEvasion,
            CloakingDuration,
            CloakingRechargeRate,
            Armor
        }
        public att Attribute;
        public float value;

    }

    [CreateAssetMenu(fileName = "New Item", menuName = "Item")]
    public class Item : ScriptableObject {

        public enum Type {
            Ammo,
            Component,
            Fuel,
            Augmentation,
            Crew,
            Droid,
            System,
            Weapon
        }

        public Type type;
        public Sprite icon;
        public int amount;
        public int value;
        public Weapon weapon;     

        [Header("Effects")]
        public List<Effect> effects = new List<Effect>();
        [Space(10)]
        public List<Effect> effectsPercentage = new List<Effect>();  

    }

}

