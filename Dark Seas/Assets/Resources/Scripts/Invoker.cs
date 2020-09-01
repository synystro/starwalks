using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {

    public class Invoker : MonoBehaviour {

        public static Invoker i;

        private void Awake() {
            if(!i)
                i=this;
        }

        public void InvokeIgnorePause(Action action, float time) {
            StartCoroutine(WaitForTime(action, time));
        }

        private IEnumerator WaitForTime(Action action, float time) {
            while (true) {
                yield return new WaitForSecondsRealtime(time);
                action();
                yield break;
            }
        }

    }

}
