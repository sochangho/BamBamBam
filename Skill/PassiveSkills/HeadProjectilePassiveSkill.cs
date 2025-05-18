using System.Collections.Generic;

using UnityEngine;

public class HeadProjectilePassiveSkill : SpwanPassiveSkill
{

    SnakeHead mineHead;

    List<StatData<float>> statAttacks;
    int statid;

    public HeadProjectilePassiveSkill()
    {
        statid = GameStatUpgrader.Instance.GetInstanceID();
        statAttacks = new List<StatData<float>>();
    }

    public override Skill Copy()
    {
        HeadProjectilePassiveSkill skill = 
            new HeadProjectilePassiveSkill();
        //skill.damage = damage;

        //skill.coolTime = coolTime;
        skill.skillName = skillName;
        skill.objectType = objectType;
        skill.id = id;
        skill.resourceId = resourceId;
        skill.soundname = soundname;
        skill.duraitionType = duraitionType;

        return skill;
    }

    public override void UseSwpanSkill(float addAttack = 0)
    {
        if (GameSceneManager.Instance.IsMine(snakePartOwner.pv))
        {

            if (mineHead == null) mineHead = snakePartOwner.GetSnakeHead();
            if (mineHead == null) return;

            var count = (int)StatsManager.GetUpgraded(SnakeUniqueAbilityType.Projectile, (int)UpgradeProjectileStats.Stats.Projectile_Count, mineHead);
            int addFire = (int)StatsManager.GetUpgraded(SnakeUniqueAbilityType.Projectile, (int)UpgradeProjectileStats.Stats.Projectile_FragmentCount, mineHead);


            Vector3 pos = mineHead.transformLaunch.position;
            ProjectileAttackGlobal.ProjectileLaunch(resourceId, count, addFire, mineHead.transform.right, pos, mineHead, addAttack);
        }
    }

    public override void ChangeBodyCount(int body)
    {
        if (body < 1)
        {
            body = 1;
        }

        if (mineHead == null) mineHead = snakePartOwner.GetComponent<SnakeHead>();
        if (mineHead == null) return;


        base.ChangeBodyCount(body);

        float value = mineHead.ability.abilityData.attack.OrigineValue * 0.05f * (body - 1);

        StatData<float> statData = new StatData<float>();

        statData.DataKey = statid;
        statData.DataValue = value;

        statAttacks.Clear();
        statAttacks.Add(statData);

        mineHead.ability.abilityData.attack.Apply(statAttacks);
    }

}