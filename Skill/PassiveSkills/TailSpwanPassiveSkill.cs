using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class TailSpwanPassiveSkill : SpwanPassiveSkill
{
    List<StatData<float>> statSpeed;
    int statidSpeed;

    List<StatData<float>> statDefense;
    int statidDefense;


    public TailSpwanPassiveSkill()
    {
        statidSpeed = GameStatUpgrader.Instance.GetMakeID();
        statSpeed = new List<StatData<float>>();

        statidDefense = GameStatUpgrader.Instance.GetMakeID();
        statDefense = new List<StatData<float>>();
    }


    public override Skill Copy()
    {
        TailSpwanPassiveSkill skill = new TailSpwanPassiveSkill();
        //skill.damage = damage;
        //skill.coolTime = coolTime;       
        skill.skillName = skillName;
        skill.durationTime = durationTime;
        skill.duraitionType = duraitionType;
        skill.objectType = objectType;
        skill.resourceId = resourceId;
        skill.objectType = objectType;
        skill.id = id;
        skill.soundname = soundname;
        return skill;
    }

    public override void UseSkill(float addAttack = 0)
    {
        base.UseSkill();
        if (GameSceneManager.Instance.IsMine(snakePartOwner.pv))
        {

            var bodies = snakePartOwner.GetComponent<SnakeHead>().snakeBodies;

            if (bodies.Count == 0) return;

            var tail = bodies[bodies.Count - 1];

            ObjectPoolManager.Instance.GetObject(objectType, resourceId,
                tail.transform.position, Quaternion.identity, snakePartOwner, null);
        }
    }

    public override void UseSwpanSkill(float addAttack = 0) { }


    public override void ChangeBodyCount(int body)
    {
        if (body < 1)
        {
            body = 1;
        }

        base.ChangeBodyCount(body);


        float value
            = GameSceneManager.Instance.MineHead().ability.abilityData.speed.OrigineValue * 0.005f * (body - 1);


        StatData<float> statData = new StatData<float>();

        statData.DataKey = statidSpeed;
        statData.DataValue = value;

        statSpeed.Clear();
        statSpeed.Add(statData);

        GameSceneManager.Instance.MineHead().ability.abilityData.speed.Apply(statSpeed);

        ///////////////////////
        float valuedefense
          = GameSceneManager.Instance.MineHead().ability.abilityData.defence.OrigineValue * 0.005f * (body - 1);


        StatData<float> statDataDefense = new StatData<float>();

        statDataDefense.DataKey = statidDefense;
        statDataDefense.DataValue = valuedefense;

        statDefense.Clear();
        statDefense.Add(statData);

        GameSceneManager.Instance.MineHead().ability.abilityData.defence.Apply(statDefense);

    }
}