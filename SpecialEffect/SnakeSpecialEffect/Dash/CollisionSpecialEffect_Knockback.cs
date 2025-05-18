public class CollisionSpecialEffect_Knockback : CollisionSpecialEffect
{
    public KnockbackData knockbackData;

    protected override void SpecialEffect(Enemy enemy, Character character)
    {
       var direction = enemy.transform.position - character.transform.position;

        direction = direction.normalized;

        SpecialEffectAdder.Instace.KnockBackCallAtExternal(knockbackData, direction, enemy);
    }
}

