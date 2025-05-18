using System.Collections;
using System.Collections.Generic;

public abstract class SpwanActiveSkill : ActiveSkill
{
    public BamGameObjectType objectType;
    public int resourceId;

    public override void EndActiveSkill()
    {
       
    }

    public override void UseSkill(float addAttack = 0)
    {
        base.UseSkill();
        UseSwpanSkill(addAttack);
    }

    public abstract void UseSwpanSkill(float addAttack = 0);

}
