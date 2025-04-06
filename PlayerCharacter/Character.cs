using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using DamageNumbersPro;

using Cysharp.Threading.Tasks;

public abstract class Character : GameSceneObject
{   
    public enum State
    {        
        Caution,
        None,
    }
    
    public enum CharacterType
    {
        Snake,
        Enemy
    }

    public enum StatusEffect
    {
        KnockBack,
        None,
        Stun,
        Critical,
        Invincibility,
        Defense,
        Speed,
        ContinueDamage,
    }

  
 
    protected float hp;
    protected float clientSpeed;

    public CharacterType characterType { get; protected set; }    
    public float hpTotal;
    public string soundAttacked;


    public List<SpecialEffectPoint> specaiallPointList;
    public CharacterOneTimeEventGroup characterOneTimeEventGroup = new CharacterOneTimeEventGroup();



    [HideInInspector] public List<Animator> animators = new List<Animator>();
    [HideInInspector] public bool isDefence = false;
    [HideInInspector] public PhotonView pv;



    public Ability ability = new Ability();
    public AttackAbility attackAbility;
    public CharacterAttributeAbility characterAttributeAbility;

    public CharacterAttack characterAttack;

    public ReceiveSpecialEffectController receiveSpecialEffectController = new ReceiveSpecialEffectController();
    protected EnemyCollidedRecoder collidedRecoder = new EnemyCollidedRecoder();
    private DamageSync damageSync;

    private State state = State.None;

    public bool isMoveStop { get { return receiveSpecialEffectController.IsMoveStop(); } }
    public bool isInvincibility { get { return receiveSpecialEffectController.IsInvincibility(); } }




    public bool IsDefence
    {
        get
        {
            return isDefence;
        }
        set
        {
            isDefence = value;
        }
    }


    
    public float Hp 
    { 
        get 
        {
            return hp;
        }
        set
        {
            hp = value;

            HpChanged?.Invoke((float)(hp/hpTotal));


            float rate = (float)hp / hpTotal;

   
            if (hp <= 10)
            {
                if(state == State.None)
                {
                    state = State.Caution;

                    if (GameSceneManager.Instace.IsMine(pv))
                    {
                        Caution?.Invoke();
                    }
                }
            }
            else
            {
                if (state == State.Caution)
                {
                    state = State.None;

                    if (GameSceneManager.Instace.IsMine(pv))
                    {
                        CautionClear?.Invoke();
                    }
                }
            }


            if(hp == 0 && GameSceneManager.Instace.IsMine(pv))
            {
                ExecuteDestroy();
            }
        }       
    }


   
 
   
    public event System.Action Caution;
    public event System.Action CautionClear;
    public event System.Action<float> HpChanged;
    public System.Action<float> DecreaseRemainHpEvent;




    /// <summary>
    /// Awake 
    /// </summary>
    public sealed override void AwakeGameObjectBase()
    {
        base.AwakeGameObjectBase();
        pv = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Awake
    /// </summary>
    public override void AwakeGameObject()
    {
        
    }

    /// <summary>
    /// Start
    /// </summary>
    public override void StartGameObject(){
        
        receiveSpecialEffectController.Play();
        damageSync = new DamageSync(this);
        Spawned();     
    }

    /// <summary>
    /// Update
    /// </summary>
    public override void UpdateGameObject()
    {     
        receiveSpecialEffectController.RoutineSpecialEffects();

        if (damageSync != null)
        {
            damageSync.Update();
        }
        CharacterRoutine();
    }

    /// <summary>
    /// Destory
    /// </summary>
    public override void DestroyGameObject(){
        damageSync = null;
        Die();     
    }

    /// <summary>
    /// Active Object 
    /// </summary>
    /// <returns></returns>
    public override bool IsActiveGameObject(){ return this.gameObject.activeSelf;}

    /// <summary>
    /// Spawnd
    /// </summary>
    public abstract void Spawned();

    /// <summary>
    /// Die
    /// </summary>
    public abstract void Die();


    /// <summary>
    /// Disable
    /// </summary>
    public override void Disable(){}

    /// <summary>
    /// ChracterUpdate
    /// </summary>
    public abstract void CharacterRoutine();

    /// <summary>
    /// Attack
    /// </summary>
    /// <param name="opCharacter"></param>
    /// <param name="addAttack"></param>
    /// <returns></returns>
    public virtual float Attack(Character opCharacter ,float addAttack = 0)
    {       
        return characterAttack.Attack(opCharacter, addAttack);
    }

    /// <summary>
    /// Damage
    /// </summary>
    /// <param name="character"></param>
    /// <param name="damage"></param>
    public virtual void AttackOnlyWithDamage(Character character ,float damage)
    {
        character.Attacked(damage);
    }


    /// <summary>
    /// Attacked
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="attributeType"></param>
    public virtual void Attacked(float damage , AttributeType attributeType = AttributeType.Nomal)
    {

        if (Hp <= 0) return;
       
        if (isInvincibility) return;

        float d = damage * -1;

        float multi = characterAttributeAbility.AttackAttributeMultiple(attributeType);

        d *= multi;

        if (GameSceneManager.Instace.IsMine(pv)){

            if(damageSync != null) damageSync.AddHpCalculate(d , true);
        }
        else
        {
            pv.RPC("AddHpCalculateToOpponent", GameSceneManager.Instace.GetOpponentPlayer(), d);            
        }
    }


    /// <summary>
    /// Heal Character
    /// </summary>
    /// <param name="value"></param>
    public virtual void Heal(float value)
    {
        if (GameSceneManager.Instace.IsMine(pv))
        {
            if (damageSync != null) damageSync.AddHpCalculate(value,true);
        }
        else
        {
            pv.RPC("AddHpCalculateToOpponent", GameSceneManager.Instace.GetOpponentPlayer(), value);
        }
    }


    /// <summary>
    /// Change hp
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetHp(float value)
    {
        if (GameSceneManager.Instace.IsSingle())
        {
            PunSetHp(value);
            return;
        }

        pv.RPC("PunSetHp", RpcTarget.All, value);
    }

    /// <summary>
    /// Hp 증가 팝업 
    /// </summary>
    /// <param name="value"></param>
    public void IncreaseHpPopup(float value)
    {
        if(GameSceneManager.Instace.IsSingle())
        {
            PunIncreagePopup(value);
            return;
        }

        pv.RPC("PunIncreagePopup", RpcTarget.All, value);
    }

    /// <summary>
    /// Hp감소 팝업
    /// </summary>
    /// <param name="value"></param>
    public void DecreaseHpPopup(float value)
    {
        if(GameSceneManager.Instace.IsSingle())
        {
            PunDecreasePopup(value);
            return;
        }

        pv.RPC("PunDecreasePopup", RpcTarget.All, value);
    }


    /// <summary>
    /// Get Character 속성
    /// </summary>
    /// <returns></returns>
    public  AttributeType GetAttributeType()
    {
        return characterAttributeAbility.AttributeType;
    }

    /// <summary>
    /// 충돌이 되었는지
    /// </summary>
    /// <returns></returns>
    public EnemyCollided GetRecentEnemyCollided()
    {
        return collidedRecoder.RecentValue();
    }


    [PunRPC]
    public void AddHpCalculateToOpponent(float value)
    {
        if (damageSync != null) damageSync.AddHpCalculate(value , false);
    }

    
    [PunRPC]
    public void PunSetHp(float value)
    {
        Hp = value;
    }

    /// <summary>
    /// PunRPC 힐 팝업
    /// </summary>
    /// <param name="number"></param>
    [PunRPC]
    public void PunIncreagePopup(float number)
    {
        SpawnPopupHill(number);
    }

    /// <summary>
    /// PunRPC 데미지 팝업 
    /// </summary>
    /// <param name="number"></param>
    [PunRPC]
    public void PunDecreasePopup(float number)
    {
        SpawnPopupDamage(number);
    }


    /// <summary>
    /// Damage Popup
    /// </summary>
    /// <param name="number"></param>
    public void SpawnPopupDamage(float number)
    {
        GameSceneDamagePopupManager.Instace.SpawnPopupDamge(this.transform.position, number);
    }

    /// <summary>
    /// Hill Popup 
    /// </summary>
    /// <param name="number"></param>
    public void SpawnPopupHill(float number)
    {
        GameSceneDamagePopupManager.Instace.SpawnPopupHill(this.transform.position, number);
    }

    /// <summary>
    /// 공격받았을 때 사운드 처리 
    /// </summary>
    public void AttackedSound()
    {
        GameSceneManager.Instace.OnSound(soundAttacked);        
    }

    public abstract  UniTask<bool> FindUniqueBuff(string uniqueid);

    /// <summary>
    /// Find Buff
    /// </summary>
    /// <param name="uniqueid"></param>
    /// <returns></returns>
    public abstract bool FindUniqueBuffLocal(string uniqueid);


    /// <summary>
    //  한번만 발생하는 이벤트 추가
    /// </summary>
    /// <param name="characterOneTimeEvent"></param>
    public void OnetimeEventAdd(CharacterOneTimeEvent characterOneTimeEvent)
    {
        characterOneTimeEventGroup.Add(characterOneTimeEvent);
    }

    /// <summary>
    /// 런타임 중 특수효과 추가
    /// </summary>
    /// <param name="specialEffect"></param>
    public void SpecialEffectAdd(SpecialEffect specialEffect)
    {
        
        receiveSpecialEffectController.Add(specialEffect.statusEffect, specialEffect);
    }

    #region AnimationCotoller

    /// <summary>
    /// 소환되었을때 애니메이션 처리
    /// </summary>
    public void SpwanAnim()
    {
        AllAnimator();
        AllAnimatorPlay(true);
    }

    /// <summary>
    /// 애니메이션 플레이
    /// </summary>
    public void AnimPlay()
    {
        AllAnimatorPlay(true);
    }
    
    /// <summary>
    /// 애니메이션 멈춤
    /// </summary>
    public void AnimStop()
    {
        AllAnimatorPlay(false);
    }

    /// <summary>
    /// 모든 애니메이션 재생
    /// </summary>
    public void AllAnimator()
    {
        if (GameSceneManager.Instace.IsSingle())
        {
            PunAllAnimator();
            return;
        }

        pv.RPC("PunAllAnimator", RpcTarget.All);
    }

    /// <summary>
    /// 모든 애니메이터 재생
    /// </summary>
    /// <param name="play"></param>
    public void AllAnimatorPlay(bool play)
    {
        if (GameSceneManager.Instace.IsSingle())
        {
            PunAllAnimatorPlay(play);
            return;
        }
        pv.RPC("PunAllAnimatorPlay", RpcTarget.All, play);
    }


    /// <summary>
    /// PunRPC 모든 애니메이션 캐싱
    /// </summary>
    [PunRPC]
    public void PunAllAnimator()
    {
        animators.Clear();
        Animator[] ats = GetComponentsInChildren<Animator>();
        animators.AddRange(ats);
    }

    /// <summary>
    /// PunRPC 애니메이션 재생
    /// </summary>
    /// <param name="play"></param>
    [PunRPC]
    public void PunAllAnimatorPlay(bool play)
    {
        foreach (var animator in animators)
        {
            if (play)
                animator.StopPlayback();
            else
                animator.StartPlayback();
        }
    }

    #endregion


    /// <summary>
    /// 이펙트 발생 위치
    /// </summary>
    /// <param name="specialEffectPointType"></param>
    /// <returns></returns>
    public Transform GetSpecialEffectPoint(SpecialEffectPointType specialEffectPointType)
    {

       var find = specaiallPointList.Find(x => x.specialEffectPointType.Equals(specialEffectPointType));

        if (find == null) return null;


        return find.transform;
    } 

    /// <summary>
    /// 상대 클라이언트한테 이 오브젝트 속도 전달
    /// </summary>
    /// <param name="speed"></param>
    public void SetNotMineClientSpeed(float speed)
    {
        clientSpeed = speed;
    }

}




