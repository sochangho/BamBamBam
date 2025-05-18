using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ColliderSkill : Skill
{
    public int effectparticleid;
    public ColliderSkill() { skillType = SkillType.Collider; }

    public virtual void OnTrigger(Collider2D collision)
    {

      SnakeHead snakeHead =  snakePartOwner.GetSnakeHead();

      float percent = StatsManager.GetUpgraded(
            SnakeUniqueAbilityType.Dash,
            (int)UpgradeDashStats.Stats.Collision_Heal_Percent , snakeHead);

        
      float hp = percent / 100f *  snakePartOwner.hpTotal;


        if (hp == 0)
        {
            return;
        }

        snakePartOwner.Heal(hp);
    }
    public virtual void OnCollision(Collision2D collision)
    {
        SnakeHead snakeHead = snakePartOwner.GetSnakeHead();

        float percent = StatsManager.GetUpgraded(
             SnakeUniqueAbilityType.Dash,
             (int)UpgradeDashStats.Stats.Collision_Heal_Percent,snakeHead);

        float hp = percent / 100f * snakePartOwner.hpTotal;

        if(hp == 0)
        {
            return;
        }

        snakePartOwner.Heal(hp);
    }


    public virtual void OnTriggerExit(Collider2D collision){ }
    public virtual void OnCollisionExit(Collision2D collision) { }

    public void OnEffect(Vector3 position , Quaternion quaternion)
    {
        

        GameSceneManager.Instance.OnEffectParticle(effectparticleid, position, quaternion);
    }
}
