using System.Collections.Generic;
using UnityEngine;

public class BombSpecialEffect_BlackHole : BombSpecialEffect
{
    public KnockbackData knockbackData;

    public int blackholeParticleID;




    protected override void SpecialEffect(Bomb bomb, IEnumerable<Enemy> enemies, Character character)
    {

        foreach (var enemy in enemies)
        {
            if (enemy == null || bomb == null) continue;

            Vector2 dir = enemy.transform.position - bomb.transform.position;

            dir = dir.normalized * -1;

            SpecialEffectAdder.Instace.KnockBackCallAtExternal(knockbackData, dir, enemy);
        }

        GameSceneManager.Instace.OnEffectParticle(blackholeParticleID , bomb.transform.position , Quaternion.identity);
    }


}