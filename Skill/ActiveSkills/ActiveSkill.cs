using System.Collections;
using System.Collections.Generic;

public abstract class ActiveSkill : Skill
{
    public ActiveSkill() { skillType = SkillType.Active; }
    public abstract void EndActiveSkill();
    public virtual void ChangeBodyCount(int body) { }
  
}
