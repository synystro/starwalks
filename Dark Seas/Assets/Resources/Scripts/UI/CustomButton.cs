using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DarkSeas {

    public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {

        public bool hasLongTap;
        [SerializeField] private Vector3 tapPos;
        [HideInInspector] public UnityEvent onTap;
        [HideInInspector] public UnityEvent onDoubleTap;
        [HideInInspector] public UnityEvent onLongTap;
        public float requiredHoldTime;

        private bool pressingDown;
        private float pressingDownTimer;
        private float pressingDownDelayTimer;
        private float tapTimer;
        private int tapCount;

        private bool tapStarted;
        private bool wasLongTap;

        private const float PRESS_DOWN_DELAY = 1f;
        private const float DOUBLE_TAP_DELAY = 0.2f;

        public Vector3 TapPos { get { return tapPos; } }

        public Image fillImage;

        private void Update() {

            if(!hasLongTap)
                return;

            if (pressingDown && hasLongTap) {

                pressingDownDelayTimer += Time.fixedDeltaTime;

                if (pressingDownDelayTimer >= PRESS_DOWN_DELAY) {
                    wasLongTap = true;
                    pressingDownTimer += Time.fixedDeltaTime;
                    if (pressingDownTimer >= requiredHoldTime) {
                        onLongTap?.Invoke();
                        Reset();
                    }
                    if (fillImage != null)
                        fillImage.fillAmount = pressingDownTimer / requiredHoldTime;
                }

            }

        }

        public void OnPointerDown(PointerEventData eventData) {
            tapPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pressingDown = true;
        }

        public void OnPointerUp(PointerEventData eventData) {
            Reset();
        }

        private void Reset() {
            pressingDown = false;
            pressingDownTimer = 0f;
            pressingDownDelayTimer = 0f;
            if (fillImage != null)
                fillImage.fillAmount = pressingDownTimer / requiredHoldTime;

        }

        public void OnPointerClick(PointerEventData eventData) {

            if(wasLongTap) {
                wasLongTap = false;
                return;
            }

            tapCount++;
            if(tapStarted)
                return;

            tapStarted = true;
            //Invoke("CheckTap", DOUBLE_TAP_DELAY);
            if(Invoker.i != null)
                Invoker.i.InvokeIgnorePause(CheckTap, DOUBLE_TAP_DELAY);
            else
                Debug.LogError("Invoker instance is missing on the current scene");

        }

        private void CheckTap() {

            if(tapCount > 1) {
                onDoubleTap?.Invoke();
                Reset();
            }
            else {
                onTap?.Invoke();
                Reset();                
            }

            tapStarted = false;
            tapCount = 0;

        }

    }

}
