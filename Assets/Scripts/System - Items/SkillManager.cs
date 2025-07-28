using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    #region Singleton
    public static SkillManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    #endregion


    public static Action<SkillSetup> OnSkillUnlocked;

    [SerializeField] private List<SkillSetup> _skillSettings = new();


    public void UnlockSkill(Skills skill)
    {
        SkillSetup targetSkill = _skillSettings.FirstOrDefault(x => x.Skill == skill);
        targetSkill.Unlock();

        OnSkillUnlocked?.Invoke(targetSkill);
    }

    public SkillSetup GetSkillSetup(Skills skill)
    {
        SkillSetup targetSkill = _skillSettings.FirstOrDefault(x => x.Skill == skill);
        return targetSkill;
    }


    /// <summary>
    ///Return true if the target skill has all skills needed unlocked before hand.
    ///Returns true if no skills are inside the list of items needed.
    /// </summary>
    public bool HasSkillRequiremenet(Skills skill)
    {
        foreach (var item in GetSkillSetup(skill).RequiredItems)
        {
            if (item.Item is Skills)
            {
                if (!GetSkillSetup(item.Item as Skills).UnlockStatus)
                {
                    return false;
                }
            }
        }
        return true;
    }


}

[System.Serializable]
public class SkillSetup
{
    public Skills Skill;
    public List<ItemAmount> RequiredItems;

    public bool UnlockStatus;

    public void Unlock()
    {
        UnlockStatus = true;
    }
}

[System.Serializable]
public class ItemAmount
{
    public Item Item;
    public int Amount;
}
