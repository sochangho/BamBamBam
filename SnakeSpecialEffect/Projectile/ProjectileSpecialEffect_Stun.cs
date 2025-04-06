using System.Collections.Generic;

public class ProjectileSpecialEffect_Stun : ProjectileSpecialEffect
{
    public StunData stunData;

    protected override void SpecialEffect(Projectile projectile, IEnumerable<Enemy> enemies, Character character)
    {
        foreach(var enemy in enemies)
        {
            SpecialEffectAdder.Instace.StunCallAtExternal(stunData, enemy);
        }
    }
}
