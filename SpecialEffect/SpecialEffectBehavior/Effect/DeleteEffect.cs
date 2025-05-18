using System.Collections.Generic;


using UnityEngine;

public class DeleteEffect
{
    public void Delete(IEnumerable<Enemy> enemies)
    {
        
        List<Enemy> enemyList = new List<Enemy>(enemies);

        if (enemyList.Count == 0) return;

        Enemy enemy = RandomManager.RandomDraw(enemyList);
        if (enemy.gameObject.activeSelf && enemy.enemyType == EnemyType.Minon)
        {
            GameSceneManager.Instace.OnEffectParticle(38, enemy.transform.position, Quaternion.identity);

            enemy.Die();
        }
    }
}
