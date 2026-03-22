using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Upgrade {

}
[System.Serializable]
public class StatUpgrade : Upgrade {
    public ST stat;
    public float amount = -1;
}
[System.Serializable]
public class UnlockSkill : Upgrade {
    public SkillSO skillSO;
}
[System.Serializable] 
public class SkillUpgrade : Upgrade{
    public SkillSO skillSO;
}

