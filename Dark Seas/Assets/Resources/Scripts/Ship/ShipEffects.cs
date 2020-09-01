using UnityEngine;

namespace DarkSeas {

    [System.Serializable]
    public class ShipEffects {

        [Header("Armor")]
        public float Armor;
        public float Armor_P;

        [Header("Evasion")]
        public float EV;
        public float PilotEV;
        public float EngineEV;
        public float EngineRR;
        public float CloakingEV;
        public float CloakingD;
        public float CloakingRR;
        public float EV_P;
        public float CaptainEV_P;
        public float EngineEV_P;
        public float EngineRR_P;
        public float CloakingEV_P;
        public float CloakingD_P;
        public float CloakingRR_P;

        [Header("Weapons")]
        public float WeaponsDMG;
        public float WeaponsSPD;
        public float WeaponsRR;
        public float WeaponsDMG_P;
        public float WeaponsSPD_P;
        public float WeaponsRR_P;
        [Header("Shield")]
        public float ShieldCAP;
        public float ShieldRR;
        public float ShieldCAP_P;
        public float ShieldRR_P;
        [Header("Health")]
        public float OxygenRR;
        public float MedbayHR;
        public float OxygenRR_P;
        public float MedbayHR_P;

    }

}