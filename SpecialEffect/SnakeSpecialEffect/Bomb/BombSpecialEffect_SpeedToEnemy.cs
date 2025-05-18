using System.Collections;
using System.Collections.Generic;

public class BombSpecialEffect_SpeedToEnemy : BombSpecialEffect
{
    public SpeedData speedData;

    protected override void SpecialEffect(Bomb bomb, IEnumerable<Enemy> enemies, Character character)
    {

        foreach (var enemy in enemies)
        {
            SpecialEffectAdder.Instace.SpeedEffectCallAtExternal(speedData, enemy);
        }
    }
}
