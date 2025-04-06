using System.Collections.Generic;

public class ProjectileSpecialEffect_DefenseToEnemy : ProjectileSpecialEffect
{
    public DefenseData defenseData;

    protected override void SpecialEffect(Projectile projectile, IEnumerable<Enemy> enemies, Character character)
    {
        foreach(var enemy in enemies)
        {
            SpecialEffectAdder.Instace.DefenseEffectCallAtExternal(defenseData, enemy);
        }
    }
}
