using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {

    public class AugmentsManager : MonoBehaviour {

        [SerializeField] private List<Item> augs;

        private Ship ship;
        private ShipEffects shipEffects;
        private List<Weaponry> shipWeapons = new List<Weaponry>();

        private void OnEnable() {
            Start();
        }

        void Start() {

            //ship.Augments.onItemChangedCallback += Refresh;

            #region get stuff

            // get ship's stats
            ship = GetComponentInParent<Ship>();
            augs = ship.Cargo.GetAugmentList();
            shipEffects = ship.shipEffects;
            // get ship's weapons
            for (int i = 0; i < GameManager.shipWeapons.Count; i++) {
                shipWeapons.Add(GameManager.shipWeapons[i].GetComponent<Weaponry>());
            }

            #endregion

            for (int i = 0; i < augs.Count; i++) {

                // if there's no aug, skip to next iteration
                if (augs[i] == null)
                    continue;

                #region buffs

                #region raw

                for (int j = 0; j < augs[i].effects.Count; j++) {

                    Effect effect = augs[i].effects[j];

                    float buffValue = effect.value;

                    switch (effect.Attribute) {

                        case Effect.att.Armor:
                            shipEffects.Armor += buffValue;
                            break;
                        case Effect.att.CommandEvasion:
                            shipEffects.PilotEV += buffValue;
                            break;
                        case Effect.att.EngineEvasion:
                            shipEffects.EngineEV += buffValue;
                            break;
                        case Effect.att.EngineRechargeRate:
                            shipEffects.EngineRR += buffValue;
                            break;
                        case Effect.att.OxygenRechargeRate:
                            shipEffects.OxygenRR += buffValue;
                            break;
                        case Effect.att.ShieldCapacity:
                            shipEffects.ShieldCAP += buffValue;
                            break;
                        case Effect.att.WeaponsDamage:
                            shipEffects.WeaponsDMG += buffValue;
                            break;
                        case Effect.att.WeaponsSpeed:
                            shipEffects.WeaponsSPD += buffValue;
                            break;
                        case Effect.att.WeaponsRechargeRate:
                            shipEffects.WeaponsRR += buffValue;
                            break;
                        case Effect.att.CloakingEvasion:
                            shipEffects.CloakingEV += buffValue;
                            break;
                        case Effect.att.CloakingDuration:
                            shipEffects.CloakingD += buffValue;
                            break;
                        case Effect.att.CloakingRechargeRate:
                            shipEffects.CloakingRR += buffValue;
                            break;
                        default:
                            break;
                    }

                }

                #endregion

                #region percentage

                for (int j = 0; j < augs[i].effectsPercentage.Count; j++) {

                    Effect effect = augs[i].effectsPercentage[j];

                    float buffPercentage = (effect.value / 100f);

                    switch (effect.Attribute) {

                        case Effect.att.Armor:
                            shipEffects.Armor_P = buffPercentage;
                            break;
                        case Effect.att.CommandEvasion:
                            shipEffects.CaptainEV_P = buffPercentage;
                            break;
                        case Effect.att.EngineEvasion:
                            shipEffects.EngineEV_P = buffPercentage;
                            break;
                        case Effect.att.EngineRechargeRate:
                            shipEffects.EngineRR_P = buffPercentage;
                            break;
                        case Effect.att.OxygenRechargeRate:
                            shipEffects.OxygenRR_P = buffPercentage;
                            break;
                        case Effect.att.ShieldCapacity:
                            shipEffects.ShieldCAP_P = buffPercentage;
                            break;
                        case Effect.att.ShieldRechargeRate:
                            shipEffects.ShieldRR_P = buffPercentage;
                            break;
                        case Effect.att.WeaponsDamage:
                            shipEffects.WeaponsDMG_P = buffPercentage;
                            break;
                        case Effect.att.WeaponsSpeed:
                            shipEffects.WeaponsSPD_P = buffPercentage;
                            break;
                        case Effect.att.WeaponsRechargeRate:
                            shipEffects.WeaponsRR_P = buffPercentage;
                            break;
                        case Effect.att.CloakingEvasion:
                            shipEffects.CloakingEV_P = buffPercentage;
                            break;
                        case Effect.att.CloakingDuration:
                            shipEffects.CloakingD_P = buffPercentage;
                            break;
                        case Effect.att.CloakingRechargeRate:
                            shipEffects.CloakingRR_P = buffPercentage;
                            break;
                        default:
                            break;
                    }

                }

                #endregion

                #endregion

            }

        }

        void Refresh()
        {
            // augs.Clear();
            // foreach (Augment aug in ship.Augments.GetItemList())
            //     augs.Add(aug);
        }

    }

}
