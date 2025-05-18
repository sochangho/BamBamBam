using System;
using System.Collections;
public abstract class SpwanPassiveSkill : PassiveSkill
{
    public BamGameObjectType objectType;
    public int resourceId;
    public override void UseSkill(float addAttack = 0)
    {
        base.UseSkill();
        UseSwpanSkill(addAttack);
    }

    public abstract void UseSwpanSkill(float addAttack = 0);

}
