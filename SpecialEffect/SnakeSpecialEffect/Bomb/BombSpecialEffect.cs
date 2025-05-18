using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BombSpecialEffect : MonoBehaviour
{    
    protected float rangeEffect;

    public void SpecialEffectExecute(Bomb bomb ,IEnumerable<Enemy> enemies , Character character )
    {       
        SpecialEffect(bomb , enemies , character);
    }

    protected abstract void SpecialEffect(Bomb bomb, IEnumerable<Enemy> enemies, Character character);

    protected bool RangeInsideOpponent(Vector2 center ,float range)
    {
        var opponent = GameSceneManager.Instace.OppoentHead();

        if(opponent != null && 
            Vector2.Distance(opponent.transform.position , center) <= range)
        {
            return true; 
        }

        return false;
    }

    protected bool RangeInsideMine(Vector2 center, float range)
    {
        var mine = GameSceneManager.Instace.MineHead();

        if (mine != null &&
            Vector2.Distance(mine.transform.position, center) <= range)
        {
            return true;
        }

        return false;
    }

}
