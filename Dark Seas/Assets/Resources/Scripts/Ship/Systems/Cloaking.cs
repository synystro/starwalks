using System.Collections;
using UnityEngine;

namespace DarkSeas {

    public class Cloaking : ShipSystem {

        public int cloakDuration = 5;
        public float cooldown = 30f;
        public float cloakedAlphaValue = 0.25f;
        public bool OnCooldown { get { return currentCD > 0 ? true : false; } }
        [Space(10)]
        [SerializeField] private float currentCD;
        [Space(20)]
        [SerializeField] private float evasionBonus = 50f;

        public override void Start() {

            base.Start();

            // set cooldown
            //onCooldown = true;
            currentCD = cooldown;

        }

        public override void Update() {

            // if no power or damage
            if (!IsPowered || IsDamaged) {
                // reset cooldown
                currentCD = cooldown;
                // skip everything else
                return;
            }

            // cooldown system
            if (OnCooldown) {
                currentCD -= Time.deltaTime;
                if (currentCD < 0f) {
                    currentCD = 0f;
                    //onCooldown = false;
                }
            }

        }

        public override void ToggleSelectSystem() {

            base.ToggleSelectSystem();

            // if off cooldown
            if(currentCD == 0f)
                StartCoroutine("Cloak");

        }

        private IEnumerator Cloak() {

            // reset cooldown
            currentCD = cooldown;

            // change ship's alpha to cloaking effect
            ship.SetAlpha(cloakedAlphaValue);

            // add bonus evasion
            ship.shipEffects.CloakingEV += evasionBonus;
            // refresh evasion to UI
            battleUiManager.RefreshEvasion();

            // wait for cloaking duration in seconds
            yield return new WaitForSeconds(cloakDuration);

            // remove bonus evasion
            ship.shipEffects.CloakingEV -= evasionBonus;
            // refresh evasion to UI
            battleUiManager.RefreshEvasion();

            // change ship's alpha back to normal
            ship.SetAlpha(1f);
        }

    }

}
