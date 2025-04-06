using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionSpecialEffect : MonoBehaviour
{
    public void SpecialEffectExecute(GameObject gameObject, Character character)
    {
        Enemy enemy = GameSceneManager.Instace.FindEnemyFinder(gameObject);

        if (enemy != null)
        {
            SpecialEffect(enemy, character);
        }
    }
    protected abstract void SpecialEffect(Enemy enemy, Character character);
}

