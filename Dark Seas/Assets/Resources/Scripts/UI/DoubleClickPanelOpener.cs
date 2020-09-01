using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkSeas
{
    public class DoubleClickPanelOpener : MonoBehaviour, IPointerClickHandler
    {
        public GameObject Panel;
        public bool hasCrewmember;

        float lastClick = 0f;
        float interval = 0.4f;

        void Awake()
        {
            Panel = transform.Find("CrewAttributesPanel").gameObject;
        }

        void Start()
        {
            Panel.transform.position = this.transform.position + new Vector3(0,100f,0);
            Panel.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (hasCrewmember)
            {
                if ((lastClick + interval) > Time.time)
                {
                    Panel.gameObject.SetActive(true);
                }
                else
                {
                    Panel.gameObject.SetActive(false);
                }
            }
            
            lastClick = Time.time;
        }
    }
}
