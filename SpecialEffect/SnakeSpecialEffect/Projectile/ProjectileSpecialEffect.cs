using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileSpecialEffect : MonoBehaviour
{
    public void SpecialEffectExecute(Projectile projectile
       , IEnumerable<Enemy> enemies, Character character)
    {
        SpecialEffect(projectile,enemies,character);
    }

    protected abstract void SpecialEffect(Projectile projectile
        , IEnumerable<Enemy> enemies, Character character);
}
