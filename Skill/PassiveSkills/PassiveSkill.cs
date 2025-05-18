using System.Collections;
using System.Collections.Generic;

public abstract class PassiveSkill : Skill
{
    public float durationTime;
    public float durationTimeCurrent;
    public DuraitionTime duraitionType;
    
    public PassiveSkill() { skillType = SkillType.Passive; }

    public virtual void ChangeBodyCount(int body) { }
}
