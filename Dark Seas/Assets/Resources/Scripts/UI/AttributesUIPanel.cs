using UnityEngine;
using UnityEngine.UI;


namespace DarkSeas {
    public class AttributesUIPanel : MonoBehaviour {

        [SerializeField] private const float spacingBetweenAttributes = 50f;
        [SerializeField] private Transform attributePanel;
        [SerializeField] private Transform attributePrefab;
        
        private Crewmember crewmember;

        public void SetCrewmember(Crewmember crewmember) {
            this.crewmember = crewmember;
            SetupPanel();
        }

        void SetupPanel() {

            int x = 0;
            int y = 0;

            if(crewmember.crewmemberStats.skills == null) {
                print("oh well");
                return;
            }

            for (int i = 0; i < crewmember.crewmemberStats.skills.Count; i++) {

                RectTransform attributeRectTransform = Instantiate(attributePrefab).GetComponent<RectTransform>();
                attributeRectTransform.SetParent(attributePanel.transform);
                float attributeRectWidth = attributeRectTransform.sizeDelta.x;
                float attributeRectHeight = attributeRectTransform.sizeDelta.y;
                attributeRectTransform.gameObject.SetActive(true);
                attributeRectTransform.anchoredPosition = new Vector2(
                    (x * attributeRectWidth + spacingBetweenAttributes) * i,
                    y * attributeRectHeight
                );

                AttributeUIManager attributeUIManager = attributeRectTransform.GetComponent<AttributeUIManager>();
                attributeUIManager.attLevel.text = crewmember.crewmemberStats.skills[(SkillType)i].ToString();
            }
            
        }

    }
}
