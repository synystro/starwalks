using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas
{
    [System.Serializable]
    public enum SkillType {
        Pilot,
        Engineer,
        Repairman,
        Marksman,
        Defensor
    }

    [System.Serializable]
    public class CrewmemberStats 
    {
        [Header("Name")]
        public string name;
        [Header("Stats")]
        public float maxHealth = 100f;
        public float health;
        public float speed = 3f;
        [Header("Skills")]
        public Dictionary<SkillType, int> skills;
        public int pilotLevel;
        public int engineerLevel;
        public int repairLevel;
        public int marksmanLevel;
        public int defensorLevel;
        [Header("Base XP needed to Level Up")]
        [SerializeField] private float xpNeededToLevelUp;
        [SerializeField] private const float baseXpNeededToLevelUp = 100f;
        [Header("XP")]
        public float repairXP;
        public float engineerXP;
        public float pilotXP;

        public void GatherSkills() {

            // set xp need to level up
            xpNeededToLevelUp = baseXpNeededToLevelUp;

            // gather all skills to a dictionary
            skills = new Dictionary<SkillType, int>();
            skills.Add(SkillType.Pilot, pilotLevel);            
            skills.Add(SkillType.Engineer, engineerLevel);
            skills.Add(SkillType.Repairman, repairLevel);            
            skills.Add(SkillType.Marksman, marksmanLevel);
            skills.Add(SkillType.Defensor, defensorLevel);
        }

        public void UpgradeSkill()
        {
            //upgrade repair skill
            if (repairXP >= xpNeededToLevelUp)
            {
                repairLevel++;
                repairXP = 0;
                xpNeededToLevelUp *= repairLevel;
            }

            //upgrade engineer skill
            if (engineerXP >= xpNeededToLevelUp)
            {
                engineerLevel++;
                engineerXP = 0;
                xpNeededToLevelUp *= engineerLevel;

                GameManager.BattleUiManager.RefreshEvasion();
            }

            //upgrade helmsman skill
            if (pilotXP >= xpNeededToLevelUp)
            {
                pilotLevel++;
                pilotXP = 0;
                xpNeededToLevelUp *= pilotLevel;

                GameManager.BattleUiManager.RefreshEvasion();
            }
        }
    }

}
