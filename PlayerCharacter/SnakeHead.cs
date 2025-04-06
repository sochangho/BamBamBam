using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using Cysharp.Threading.Tasks;




/// <summary>
/// 플레이어 공격 및 데미지, 컨트롤 처리
/// </summary>
public class SnakeHead : SnakePart
{
    public enum MoverType
    {
        HorizontalInput,
        FourKeyInput,
    }

    public Transform transformLaunch;

    public PlayerCondition playerCondition;

    public List<SnakeBody> snakeBodies = new List<SnakeBody>();

    public ESnakeType eSnakeType;

    public int bodyCount { get; set; } = 1;

    public float regenerativeTime = 1;

    public bool isDiePlayer = false;

    public bool isStop = false;

    public SnakeMoveController snakeMoveController;

    public UniqueSnakeStat uniqueSnakeStat = new UniqueSnakeStat();

    public AttackEnemyRecoder attackEnemyRecoder = new AttackEnemyRecoder();

    public BuffController<PlayerBuff> buffController = new BuffController<PlayerBuff>();

    public StatFieldFloat regenerativeTimeStat = new StatFieldFloat();

    protected SnakePartSkill snakePartSkill;

    public PlayerStatUpgradeExecuter upgradeExecuter = new PlayerStatUpgradeExecuter();

    public SnakeSpecialEffectItemController skillItems = new SnakeSpecialEffectItemController();


    /// <summary>
    /// MoveController Initialize
    /// </summary>
    public void InitMover()
    {
        snakeMoveController = new SnakeMoveController();
        snakeMoveController.Init(this,playerCondition,markerManager);
        snakeMoveController.InitMover();

        if (GameSceneManager.Instace.IsMine(pv))
        {
            snakeMoveController.AddBodies(bodyCount);
        }

        SnakeUpdate().Forget();
        Regenerative().Forget();
    
    }



    /// <summary>
    /// Spwand
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();

        snakePartSkill = GetComponent<SnakePartSkill>();

        if (GameSceneManager.Instace.IsMine(pv))
            EnableHeadMine();
        else
            EnableHeadOppoent();


        if(playerType == PlayerType.Oponnent)
        {
            this.gameObject.layer = LayerMask.NameToLayer(LayerStr.HEAD_OPPONENT);
        }
        
        regenerativeTimeStat.OrigineValue = 1;
    }


    /// <summary>
    /// Attack
    /// </summary>
    /// <param name="opCharacter"></param>
    /// <param name="addAttack"></param>
    /// <returns></returns>
    public override float Attack(Character opCharacter, float addAttack = 0)
    {
        if(opCharacter is Enemy)
        {
            Enemy enemy = opCharacter as Enemy;
            attackEnemyRecoder.RecodeValue(enemy);
        }

        return base.Attack(opCharacter, addAttack);
    }


    /// <summary>
    /// Character Loop
    /// </summary>
    public override void CharacterRoutine()
    {
        base.CharacterRoutine();
        buffController.BuffUpdate();
    }


    /// <summary>
    /// SnakeMove
    /// </summary>
    public override void SnakeMove(){}


    /// <summary>
    /// Heal
    /// </summary>
    /// <param name="value"></param>
    public override void Heal(float value)
    {
        base.Heal(value);

        if (GameSceneManager.Instace.IsMine(pv)) 
            BuffSystemHelper.Instace.BuffPeriodApply(BuffTriggerPeriod.OnHeal,this);
    }


    /// <summary>
    /// Snake Update
    /// </summary>
    /// <returns></returns>
    public async UniTask SnakeUpdate()
    {
        while (!isDiePlayer)
        {
            if (!isStop)
            {
                if (snakeMoveController == null) return;
                snakeMoveController.SnakeMove();
            }

            await UniTask.WaitForFixedUpdate(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

    }

    /// <summary>
    /// SnakePart Initialize
    /// </summary>
    public override void InitPart()
    {
        base.InitPart();

        bodyType = BodyType.Head;

        SetHp(hpTotal);

        upgradeExecuter.Initialized(this);

        Caution += playerCondition.RemoveTail;
        CautionClear += playerCondition.RemoveTailCancel;
        IsAutoPlayer = false;
    }



    public void EnableHeadMine()
    {
        GameSceneManager.Instace.SetMineHead(this);       
    }

    public void EnableHeadOppoent()
    {
        GameSceneManager.Instace.SetOpponetHead(this);     
    }
   
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameSceneManager.Instace.IsMine(pv)&&snakePartSkill != null)
        {   
            snakePartSkill.OnTriggerSkills(collision);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameSceneManager.Instace.IsMine(pv) && snakePartSkill != null)
        {
            snakePartSkill.OnCollisionSkills(collision);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (GameSceneManager.Instace.IsMine(pv) && snakePartSkill != null)
        {
            snakePartSkill.OnTriggerExitSkills(collision);
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (GameSceneManager.Instace.IsMine(pv) && snakePartSkill != null)
        {
            snakePartSkill.OnCollisionExitSkills(collision);
        }
    }



    #region Attacked


    /// <summary>
    /// 공격 받은 함수
    /// </summary>
    /// <param name="value"></param>
    /// <param name="attributeType"></param>
    public override void Attacked(float value, AttributeType attributeType = AttributeType.Nomal)
    {        
        if (!playerCondition.IsAttackable)
            return;

      
        if (playerType == PlayerType.Mine)
            MineAttacked(value , attributeType);           
        else
            OppoentAttacked(value, attributeType);    
    }

   
    /// <summary>
    /// 이 객체가 Local일 경우
    /// </summary>
    /// <param name="value"></param>
    /// <param name="attributeType"></param>
    public void MineAttacked(float value , AttributeType attributeType )
    { 
        PunSnakeAttacked(value , (int)attributeType);        
    }


    /// <summary>
    /// 이 객체가 상대오브젝트일 경우 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="attributeType"></param>
    public void OppoentAttacked(float value , AttributeType attributeType)
    {       
        pv.RPC("PunSnakeAttacked",PhotonCustomProperty.FindOpponent() ,value,(int)attributeType);
    }

    public override SnakeHead GetSnakeHead()
    {
        return this;
    }


    [PunRPC]
   public void PunSnakeAttacked(float value,int attributeType)
   {
#if DEV_OPTIMIZATION
        BamDebug.Log("DEV_OPTIMIZATION Damage : 0");
        return;
#endif

        if (GameSceneManager.Instace.IsMine(pv) && GameStateManager.isInvincibility) return;
        
        if (GameSceneManager.Instace.waveSystemVersion2.GetState() == WaveSystemVersion2.State.Proceeding)
        {
            //GameSceneManager.Instace.OnSound("MagicSwordHit_Poison_2");

            base.Attacked(value , (AttributeType)attributeType);
                        
            playerCondition.CollisionEnemy();
        }
    }

    #endregion


    public void HitWall()
    {
        if (GameSceneManager.Instace.IsMine(pv) && GameStateManager.isInvincibility) return;

        if (GameSceneManager.Instace.waveSystemVersion2.GetState() == WaveSystemVersion2.State.Proceeding)
        {
            if (playerCondition.IsAttackable)
            {
                base.Attacked(Values.HitWallDecreaseHpConstant);
            }
        }
    
    }

#region Destroy

    
    public override void Die()
    {
        base.Die();

        isDiePlayer = true;

        Caution -= playerCondition.RemoveTail;
        CautionClear -= playerCondition.RemoveTailCancel;

        if (GameSceneManager.Instace.IsMine(pv))
        {
           GameSceneManager.Instace.waveSystemVersion2.PlayerDie();           
        }

        GameSceneManager.Instace.OnEffectParticle(39, this.transform.position, Quaternion.identity);

        GameSceneManager.Instace.DestroyPlayer();
        GameSceneManager.Instace.GameOver();
    }



    #endregion

    /// <summary>
    /// 재생 루틴
    /// </summary>
    /// <returns></returns>
    protected async UniTask Regenerative()
    {
        while (!isDiePlayer)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(regenerativeTimeStat.Value)
                , cancellationToken: this.GetCancellationTokenOnDestroy());

            float regenerative = ability.regenerative;

            float percent = regenerative / 100f;

            float value =  percent / 100 * hpTotal;

            float hillHp = hp + value;

            if(hillHp > hpTotal)
            {
                hillHp = hpTotal;
            }

            SetHp(hillHp);
        }
    }


    /// <summary>
    /// 버프 실행/중지
    /// </summary>
    /// <param name="active"></param>

    public void BuffUpdateActive(bool active)
    {
        buffController.isBuffUpdate = active;
    }

    /// <summary>
    /// 재생루프 간격 컨트롤
    /// </summary>
    /// <param name="percent"></param>
    public void RegenerativeTimeDecrease(float percent)
    {
       float value = MathUtilities.CalurateStat(HowtoIncrease.PercentDecrease, regenerativeTimeStat.OrigineValue, percent);
        regenerativeTimeStat.Apply(new List<StatData<float>>() { new StatData<float>() {
            DataKey = GameStatUpgrader.Instace.GetMakeID() ,
            DataValue = value

        }});
    }


    /// <summary>
    /// Get 플레이어 스킬
    /// </summary>
    /// <returns></returns>
    public SnakePartSkill GetSnakePartSkiil() => snakePartSkill;

}

