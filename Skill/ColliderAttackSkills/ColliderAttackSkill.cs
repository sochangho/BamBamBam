using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class ColliderAttackSkill : ColliderSkill
{
    
    public override void OnCollision(Collision2D collision)
    {
        Enemy e = GameSceneManager.Instance.FindEnemyFinder(collision.gameObject);
        IAttackableObject attackableObject = collision.gameObject.GetComponent<IAttackableObject>();

        if (e != null )
        {               
            base.OnCollision(collision);
            ColliderAttack(e);                 
        }
        
        if(attackableObject != null)
        {
            attackableObject.Attacked(snakePartOwner);
        }

    }

    public override void OnTrigger(Collider2D collision)
    {     
        Enemy e = GameSceneManager.Instance.FindEnemyFinder(collision.gameObject);
        IAttackableObject attackableObject = collision.gameObject.GetComponent<IAttackableObject>();
        if (e != null)
        {           
           base.OnTrigger(collision);
           ColliderAttack(e);                            
        }

        if (attackableObject != null)
        {
            attackableObject.Attacked(snakePartOwner);
        }
    }

    public override void OnTriggerExit(Collider2D collision)
    {
        base.OnTriggerExit(collision);      
    }

    public override void OnCollisionExit(Collision2D collision)
    {
        base.OnCollisionExit(collision); 
    }


    public void Attack(Enemy e)
    {
        if (e != null)
        {
            snakePartOwner.Attack(e);
        }
    }

    public override void UseSkill(float addAttack = 0)
    {
        GameSceneManager.Instance.OnSound(soundname);
    }

    public override Skill Copy()
    {
        ColliderAttackSkill skill = new ColliderAttackSkill();        
        skill.id = id;
        skill.effectparticleid = effectparticleid;
        skill.skillName = skillName;
        skill.soundname = soundname;
        return skill;
    }

    public void ColliderAttack(Enemy enemy)
    {
      
         UseSkill();
         OnEffect(enemy.transform.position, Quaternion.identity);
         Attack(enemy);

        snakePartOwner.GetSnakeHead().skillItems.DashAttackEvent(this, enemy);
        snakePartOwner.GetSnakeHead().upgradeExecuter.addAttacks.AddAttributeAttackApply(TriggerPeriod.Period_Attack, enemy);
    }


}
