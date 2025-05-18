using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

using DamageNumbersPro;

using Cysharp.Threading.Tasks;

public enum BodyType
{
    Head,
    Body,
}


public abstract class SnakePart : Character, IPunObservable
{
   
    public Animator animatorSnake;

    public SpriteRenderer spriteRenderer;
    public MarkerManager markerManager;
    public Rigidbody2D rigidbody2D;

    private Vector3 receivePos;
    private Quaternion receiveRot;
   

    public PlayerType playerType { get; protected set; }
    public BodyType bodyType { get; protected set; }

    public bool IsAutoPlayer { get; protected set; }

    private bool IsEnd = false;

    public override void AwakeGameObject()
    {
        base.AwakeGameObject();
        characterType = CharacterType.Snake;
    }

    public override void Spawned()
    {
        InitPlayerType();
    }

    public override void Die()
    {
        GameSceneManager.Instance.RemoveSnakePartAtFinder(this.gameObject);
    }

    public override void CharacterRoutine()
    {
       
        SnakeNoIsMineUpdate();
        SnakeMove();
       
    }


    public virtual void SnakeMove() { }


    private void InitPlayerType()
    {
       
        if (GameSceneManager.Instance.IsMine(pv))
            playerType = PlayerType.Mine;
        else
            playerType = PlayerType.Opponent;


        GameSceneManager.Instance.AddSnakePartAtFinder(this);
    }


    virtual public void InitPart()
    {
        if (markerManager == null)
        {
            markerManager = GetComponent<MarkerManager>();
        }

        if (rigidbody2D == null)
        {
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        rigidbody2D.gravityScale = 0f;
        
    }

    

    public void SetBodyTransform(MarkerManager.Marker prevMarker)
    {
        markerManager.transform.SetPositionAndRotation(prevMarker.position, prevMarker.rotation);
    }

    public void SetOrderinLayer(int order)
    {
        if (GameSceneManager.Instance.IsSingle())
        {
            PunSetOrderinLayer(order);
            return;
        }

        pv.RPC("PunSetOrderinLayer", RpcTarget.AllBuffered, order);
    }

    public void SnakeMoveLeftAnimation()
    {
        if (GameSceneManager.Instance.IsSingle())
        {
            PunSnakeMoveLeftAnimation();
            return;
        }
        pv.RPC("PunSnakeMoveLeftAnimation", RpcTarget.AllBuffered);
    }

    public void CollisionAtEnemy(Enemy enemy , float damage , Vector2 pos)
    {
        if (GameSceneManager.Instance.IsSingle())
        {
            PunRecordeCollistionEnemyDamage(enemy.photonView.ViewID, damage,pos);          
            return;
        }
        pv.RPC("PunRecordeCollistionEnemyDamage", RpcTarget.All,enemy.photonView.ViewID, damage,pos);   
    }

    
    #region PunRPC

    [PunRPC]
    public void PunSetOrderinLayer(int order)
    {
        spriteRenderer.sortingOrder = order;
    }


    [PunRPC]
    public void PunSnakeMoveRightAnimation()
    {
        animatorSnake.SetBool("Right", true);
        animatorSnake.SetBool("Left", false);
    }


    [PunRPC]
    public void PunSnakeMoveLeftAnimation()
    {
        animatorSnake.SetBool("Left", true);
        animatorSnake.SetBool("Right", false);
    }




    [PunRPC]
    public void PunRecordeCollistionEnemyDamage(int viewid ,float damage , Vector2 collisionPos)
    {
        if (GameSceneManager.Instance.IsMine(pv))
        {
            Enemy enemy = 
                GameSceneManager.Instance.GetSpwaner().GetFindEnemyByViewId(viewid);

            if (enemy != null)
            {
                var enemyCollided =new EnemyCollided();
                enemyCollided.enemyViewId = viewid;
                enemyCollided.damage = damage;
                enemyCollided.positionCollided = collisionPos;

                collidedRecoder.RecodeValue(enemyCollided);            
            }


            BuffSystemHelper.Instance.BuffPeriodApply(BuffTriggerPeriod.OnCollisionEnemy,GetSnakeHead());
        }
    }


    #endregion



    public void SnakeNoIsMineUpdate()
    {

        if (!GameSceneManager.Instance.IsMine(pv))
        {
            var opponentHead =  GameSceneManager.Instance.OppoentHead();

            if (opponentHead == null) return;

            float speed = 0;

            if (opponentHead.playerCondition.IsMovable)
            {
                speed = GameSceneManager.Instance.OppoentHead().oppoentClientSpeed;
            }
            else
            {
                speed = Values.HitWallBackMoveSpeed;
            }

            transform.position = Vector3.MoveTowards(transform.position, receivePos,
                speed * Values.SpeedConstant * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, 100 * Time.fixedDeltaTime);
        }
    }



    public override async UniTask<bool> FindUniqueBuff(string uniqueid)
    {     
        return false;
    }

    public override bool FindUniqueBuffLocal(string uniqueid)
    {
        return false;
    }


    public override void SendPhotonSerializeView(PhotonStream stream)
    {     
        stream.SendNext(transform.position);
        stream.SendNext(transform.rotation);
        stream.SendNext(oppoentClientSpeed);
    }


    public override void ReceivePhotonSerializeView(PhotonStream stream)
    {        
        receivePos = (Vector3)stream.ReceiveNext();
        receiveRot = (Quaternion)stream.ReceiveNext();
        oppoentClientSpeed = (float)stream.ReceiveNext();
    }



    public abstract SnakeHead GetSnakeHead();
}
