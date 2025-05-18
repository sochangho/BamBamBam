using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class DashSkill : ActiveSkill
{
    public int resorceId;
    public BamGameObjectType objectType;
    public string loadAbilitySkillname;

    protected SnakePartSkill snakePartSkill;
    protected AbilitySkill abilitySkill;

    protected StatFieldFloat dashValue;



    public DashSkill()
    {

    }

 
    public override void UseSkill(float addAttack = 0)
    {
        base.UseSkill();


        if (snakePartSkill == null)
        {
            snakePartSkill = snakePartOwner.GetComponent<SnakePartSkill>();
        }


        if (GameSceneManager.Instance.IsMine(snakePartSkill.pv))
        {
           // snakePartSkill.AddAbilitySkills("DashSpeedAbility");

            if (abilitySkill == null)
            {
                abilitySkill = GameSkillSystem.Instance.GetAbilitySkill(loadAbilitySkillname);
            }

            if(dashValue == null)
            {
                dashValue = new StatFieldFloat();
                dashValue.OrigineValue = abilitySkill.value;      
            }

           SnakeHead snakeHead =  snakePartOwner.GetSnakeHead();

           float percent = StatsManager.GetUpgraded(SnakeUniqueAbilityType.Dash,(int)UpgradeDashStats.Stats.Dash_Attack_Speed,snakeHead);

           BamDebug.Log($"<color=red> ½ºÇÇµå : {percent} </color>");

           abilitySkill.value =  dashValue.OrigineValue * (1 + percent / 100);
           
           

           snakePartSkill.AddAbilitySkillAndStart(abilitySkill);           
        }


     
        ObjectPoolManager.Instance.GetObject(BamGameObjectType.Particle, resorceId
            , snakePartOwner.transform.position, 
             snakePartOwner.transform.rotation,snakePartOwner,null
            );
    }



    public override void EndActiveSkill()
    {

    }

    public override Skill Copy()
    {
        DashSkill skill = new DashSkill();
        //skill.damage = damage;
        skill.id = id;
        //skill.coolTime = coolTime;
        skill.skillName = skillName;
        skill.resorceId = resorceId;
        skill.objectType = objectType;
        skill.soundname = soundname;
        skill.loadAbilitySkillname = loadAbilitySkillname;

        return skill;
    }
}
