using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;



/// <summary>
/// 플레이어 꼬리 받은데미지 처리 -> SnakeHead에서 처리
/// </summary>
public class SnakeBody : SnakePart
{    
    public int index { get; set; }

    public SnakeBodyCondition snakeBodyCondition;

  
    /// <summary>
    /// Spawned
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();

        if (playerType == PlayerType.Oponnent)
        {
            this.gameObject.layer = LayerMask.NameToLayer(LayerStr.BODY_OPPONENT);
        }
    }



    /// <summary>
    /// Part Intialize
    /// </summary>
    public override void InitPart()
    {   
        base.InitPart();
        bodyType = BodyType.Body;
        IsAutoPlayer = false;
    }

 
    /// <summary>
    /// 공격 받음
    /// </summary>
    /// <param name="value"></param>
    /// <param name="attributeType"></param>
    public override void Attacked(float value , AttributeType attributeType = AttributeType.Nomal)
    {
        if (playerType == PlayerType.Mine)
        {
            if (GameSceneManager.Instace.MineHead() == null) return;

            GameSceneManager.Instace.MineHead().Attacked(value,attributeType);
        }
        else
        {
            if (GameSceneManager.Instace.OppoentHead() == null) return;

            GameSceneManager.Instace.OppoentHead().Attacked(value,attributeType);
        }
    }


    /// <summary>
    /// Die
    /// </summary>
    public override void Die()
    {
        base.Die();

        SnakeHead head =  GameSceneManager.Instace.MineHead();

        var bodies  = head.snakeBodies;

        bodies.Remove(this);

        SynchronizationMethods.Destroy(this.gameObject);        
    }


    /// <summary>
    /// GetHead
    /// </summary>
    /// <returns></returns>
    public override SnakeHead GetSnakeHead()
    {
        if(playerType == PlayerType.Mine)
        {
           return GameSceneManager.Instace.MineHead();
        }
        else
        {
            return GameSceneManager.Instace.OppoentHead();
        }
    }


}
