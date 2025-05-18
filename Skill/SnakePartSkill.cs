using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;



public class SnakePartSkill : MonoBehaviourPun
{
    private SnakePart snakePart;
    private SnakeHead snakeHead;

    [HideInInspector]
    public PhotonView pv;

    public ActiveSkill activeSkill;
    
    //public float activeSkillInterval = 0.01f;
    
    private int activeSkillCount = 1;
  
    public bool isEnableActiveSkill = true;
    private bool isUsingActiveSkill = false;

    public float interval = 0.1f;
    
    public List<ColliderSkill> colliderSkills = new List<ColliderSkill>();
    public List<PassiveSkill> passiveSkills = new List<PassiveSkill>();
    public List<AbilitySkill> abilitySkills = new List<AbilitySkill>();

    private float current_passiveCoolTime = 0;
    private float current_activeCoolTime = 0;

    public List<CollisionSpecialEffect> collisionSpecialEffects;

    public void Awake()
    {
        if(pv == null) pv = GetComponent<PhotonView>();

        snakePart = GetComponent<SnakePart>();
        snakeHead = snakePart.GetSnakeHead();
    }


    public void OnEnable()
    {
        isEnableActiveSkill = true;
        isUsingActiveSkill = false;
    }


    private void ActiveSkillSet(Skill skill)
    {
        if (skill is ActiveSkill)
        {
            activeSkill = skill as ActiveSkill;
            activeSkill.InitSnakePart(snakePart);

            if (GameSceneManager.Instance.IsMine(pv))
            {
                GameSceneManager.Instance.MineHead().snakeMoveController.bodyCountEvent 
                    += activeSkill.ChangeBodyCount;
            }
        }
    }

    private void PassiveSkillAdd(Skill skill)
    {
        if (skill is PassiveSkill)
        {
            
            var p = skill as PassiveSkill;
            p.InitSnakePart(snakePart);
            passiveSkills.Add(p);


            if (GameSceneManager.Instance.IsMine(pv))
            {
                GameSceneManager.Instance.MineHead().snakeMoveController.bodyCountEvent
                    += p.ChangeBodyCount;
            }
        }
    }

    private void ColliderSkillAdd(Skill skill)
    {
        if (skill is ColliderSkill)
        {
            var c = skill as ColliderSkill;
            c.InitSnakePart(snakePart);
            colliderSkills.Add(c);
        }
    }

    public void AddSkill(int skill_id)
    {
        if (GameSceneManager.Instance.IsSingle())
        {
            PunAddSkill(skill_id);
            return;
        }
        pv.RPC("PunAddSkill", RpcTarget.AllBuffered, skill_id);
    }

    public void AddAbilitySkills(string skillName)
    {
        if (GameSceneManager.Instance.IsSingle())
        {
            PunAddAbilitySkill(skillName);
            return;
        }
        pv.RPC("PunAddAbilitySkill", RpcTarget.AllBuffered, skillName);
    }

    
    public void RemovePassiveSkill(int index)
    {
        if (GameSceneManager.Instance.IsSingle())
        {
            PunRemovePassiveSkill(index);
            return;
        }
        pv.RPC("PunRemovePassiveSkill", RpcTarget.AllBuffered, index);
    }

    public void RemoveColliderSkill(int index)
    {
        if (GameSceneManager.Instance.IsSingle())
        {
            PunRemoveColliderSkill(index);
            return;
        }
        pv.RPC("PunRemoveColliderSkill", RpcTarget.AllBuffered, index);
    }
    
    public void RemoveAbilitySkill(int index)
    {


        if (abilitySkills.Count > index)
        {
            abilitySkills.RemoveAt(index);
        }
    }

    public void AbilityEndEffect(int index)
    {

        if (abilitySkills.Count > index)
        {
            abilitySkills[index].EndEffect();
        }
    }

    public void ActiveSkillCheckUse()
    {
        if (!isEnableActiveSkill)return;
        if (activeSkill == null) return;

        isEnableActiveSkill = false;
        
        VibrationManager.Vibrate(Values.InputVibrateValue);
        GameSceneManager.Instance.ClearTutorial(GameTutorialController.ATTACK);
        StartCoroutine(ActiveCoolTimeWait());
        StartCoroutine(ActiveSkillRoutine());
    }

    public void UseActiveSkill(float addattack )
    {
        if (GameSceneManager.Instance.IsSingle())
        {
            PunUseSkill(addattack);
            return;
        }

        pv.RPC("PunUseSkill", RpcTarget.AllBuffered , addattack);
    }


    public void UsePassiveSkill(int index , float addattack)
    {
      
        if (GameSceneManager.Instance.IsSingle())
        {
            PunUsePassiveSkill(index , addattack);
                 
            return;
        }

        pv.RPC("PunUsePassiveSkill", RpcTarget.AllBuffered ,index , addattack);        
    }



    public void Update()
    {
        if (GameSceneManager.Instance.IsMine(pv))
        {
            UpdatePassiveSkill();
            UpdateAbilitySkill();
        }
                
    }



    public void UpdatePassiveSkill()
    {

        List<int> deleteSkillIndexs = new List<int>();
        float skillPassiveCoolTime
             = StatsManager.GetUpgraded(
         SnakeUniqueAbilityType.Skill,
         (int)UpgradeSkillStats.Stats.Skill_CoolTime_Passive, snakeHead);

        if (skillPassiveCoolTime == 0) return;
        
        for (int i = 0; i < passiveSkills.Count; ++i)
        {
            var passive = passiveSkills[i];            
            current_passiveCoolTime += Time.deltaTime;

            if (current_passiveCoolTime >= skillPassiveCoolTime)
            {              
               current_passiveCoolTime = 0;

                
                StartCoroutine(PassiveSkillRoutine(i));
            }
            

            if(passive.duraitionType == DuraitionTime.LimitTime) 
            {              
                passive.durationTimeCurrent += Time.deltaTime;

                if (passive.durationTime < passive.durationTimeCurrent)
                {
                    passive.durationTimeCurrent = passive.durationTime;
                    deleteSkillIndexs.Add(i);
                }
            }

        }

        foreach(var i in deleteSkillIndexs)
        {
            RemovePassiveSkill(i);//RPC
        }

    }

   
    /// <summary>
    /// RPC 사용하여 동기화 -> Ability Skill 로컬에서만 추가 및 제거로 변경
    /// </summary>
    public void UpdateAbilitySkill()
    {
        List<int> deleteSkillIndexs = new List<int>();

    
        for (int i = 0; i < abilitySkills.Count; ++i)
        {
            if(abilitySkills[i].duraitionType == DuraitionTime.Infinite) { continue; }

            abilitySkills[i].currentduration -= Time.deltaTime;
     
            if(abilitySkills[i].currentduration < 0)
            {
               
                abilitySkills[i].currentduration = 0;
                AbilityEndEffect(i);
                deleteSkillIndexs.Add(i);
            }
        }

        foreach (var i in deleteSkillIndexs)
        {
            RemoveAbilitySkill(i);
        }

    }


    IEnumerator ActiveCoolTimeWait()
    {
        current_activeCoolTime = 0;
        float activeCoolTime = StatsManager.GetUpgraded(
            SnakeUniqueAbilityType.Skill,
            (int)UpgradeSkillStats.Stats.Skill_CoolTime_Active,snakeHead);

        isUsingActiveSkill = true;

        while (activeCoolTime > current_activeCoolTime)
        {
            current_activeCoolTime += Time.deltaTime;

            yield return null;
        }
        isEnableActiveSkill = true;
        isUsingActiveSkill = false;
    } 


    public void OnTriggerSkills(Collider2D collision)
    {
        

        StartCoroutine(OnTriggerSkillsRoutine(collision));
    }

    public void OnCollisionSkills(Collision2D collision)
    {
       

        StartCoroutine(OnCollsionSkillsRoutine(collision));
    }

    public void OnTriggerExitSkills(Collider2D collision)
    {
        StartCoroutine(OnTriggerExitSkillsRoutine(collision));
    }

    public void OnCollisionExitSkills(Collision2D collision)
    {
        StartCoroutine(OnCollsionExitSkillsRoutine(collision));
    }

    IEnumerator OnTriggerSkillsRoutine(Collider2D collision)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(interval);

        if (isUsingActiveSkill)
        {
            foreach (var effect in collisionSpecialEffects)
            {
                effect.SpecialEffectExecute(collision.gameObject, snakePart);
            }
        }

        foreach(var s in colliderSkills)
        {
            s.OnTrigger(collision);

            yield return waitForSeconds;
        }
    }

    IEnumerator OnCollsionSkillsRoutine(Collision2D collision)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(interval);

        if (isUsingActiveSkill)
        {
            foreach (var effect in collisionSpecialEffects)
            {
                effect.SpecialEffectExecute(collision.gameObject, snakePart);
            }
        }
        
        foreach (var s in colliderSkills)
        {
            s.OnCollision(collision);

            yield return waitForSeconds;
        }
    }

    IEnumerator OnTriggerExitSkillsRoutine(Collider2D collision)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(interval);

        foreach (var s in colliderSkills)
        {
            s.OnTriggerExit(collision);

            yield return waitForSeconds;
        }
    }

    IEnumerator OnCollsionExitSkillsRoutine(Collision2D collision)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(interval);
        foreach (var s in colliderSkills)
        {
            s.OnCollisionExit(collision);

            yield return waitForSeconds;
        }
    }


    IEnumerator ActiveSkillRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(Values.Constanct_Attack_Interval);

        int count = (int)StatsManager.GetUpgraded(
            SnakeUniqueAbilityType.Skill,
            (int)UpgradeSkillStats.Stats.Skill_Active_AttackCount,snakeHead);

        float finalAddAttack = StatsManager.GetUpgraded
            (
            SnakeUniqueAbilityType.Skill,
            (int)UpgradeSkillStats.Stats.Skill_Active_FinalAttack_Increase_Percent,snakeHead
            ); 



        for (int attackIndex = 0; attackIndex < count; ++attackIndex)
        {
            float addattack;

            if (attackIndex == count - 1)
            {               
                addattack = finalAddAttack;
            }
            else
            {
                addattack = 0f;
            }
            UseActiveSkill(addattack);
            yield return waitForSeconds;
        }
    }

    IEnumerator PassiveSkillRoutine(int index)
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(Values.Constanct_Attack_Interval);

        int count = (int)StatsManager.GetUpgraded(
         SnakeUniqueAbilityType.Skill,
         (int)UpgradeSkillStats.Stats.Skill_Passive_Use_Count, snakeHead);

        float finalAddAttack = StatsManager.GetUpgraded
            (
            SnakeUniqueAbilityType.Skill,
            (int)UpgradeSkillStats.Stats.Skill_Passive_FinalUse, snakeHead
            );

        for (int attackIndex = 0; attackIndex < count; ++attackIndex)
        {
            float addattack;

            if (attackIndex == count - 1)
            {
                addattack = finalAddAttack;
            }
            else
            {
                addattack = 0f;
            }
            UsePassiveSkill(index , addattack);
            yield return waitForSeconds;
        }
    }


    #region PunRPC


    [PunRPC]
    public void PunAddSkill(int  skill_id)
    {
        var skill = GameSkillSystem.Instance.GetSkill(skill_id);

        if(skill.GetSkillType() == SkillType.Active)
        {
            ActiveSkillSet(skill);
        }
        else if(skill.GetSkillType() == SkillType.Passive)
        {
            
            PassiveSkillAdd(skill);
        }
        else if(skill.GetSkillType() == SkillType.Collider)
        {
            ColliderSkillAdd(skill);
        }

    }


 
    [PunRPC]
    public void PunAddAbilitySkill(string skillname)
    {
        var skill = GameSkillSystem.Instance.GetAbilitySkill(skillname);
        AddAbilitySkillAndStart(skill);
    }

    public void AddAbilitySkillAndStart(AbilitySkill skill)
    {
        if (skill.abillityEffect == null)
        {
            skill.AddAbillityEffect(snakePart.ability);
        }
        
        skill.StartEffect();

        skill.currentduration = skill.duration;

        var find = abilitySkills.Find(x => x.id == skill.id);

        if (find == null)
        {           
            abilitySkills.Add(skill);
        }
    } 


    [PunRPC]
    public void PunRemovePassiveSkill(int index)
    {
        if (passiveSkills.Count > index)
        {
            passiveSkills.RemoveAt(index);
        }      
    }


    [PunRPC]
    public void PunRemoveColliderSkill(int index)
    {
        if (colliderSkills.Count > index)
        {
            colliderSkills.RemoveAt(index);
        }
    }


    [PunRPC]
    public void PunUseSkill(float addattack)
    {
        if (activeSkill == null) return;

        activeSkill.UseSkill(addattack);
    }

    [PunRPC]
    public void PunUsePassiveSkill(int index , float addattack)
    {
        if (passiveSkills.Count > index)
        {
            passiveSkills[index].UseSkill(addattack);
        }

    }



    #endregion

}
