using System.Collections;
using UnityEngine;


using System.Collections.Generic;

public abstract class WaveSpecialEffect : MonoBehaviour
{
   
    public void SpecialEffectExecute(BamVibrationParticleBase wave
        , IEnumerable<Enemy> enemies, Character character)
    {     
       SpecialEffect(wave,enemies,character);                
    }
    public abstract void SpecialEffect(BamVibrationParticleBase wave
        , IEnumerable<Enemy> enemies, Character character);


    protected bool RangeInsideOpponent(Vector2 center, float range)
    {
        var opponent = GameSceneManager.Instace.OppoentHead();

        if (opponent != null &&
            Vector2.Distance(opponent.transform.position, center) <= range)
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
