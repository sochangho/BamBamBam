using System.Collections.Generic;

public class BombSpecialEffect_Stun : BombSpecialEffect
{
    public StunData stunData;


    protected override void SpecialEffect(Bomb bomb, IEnumerable<Enemy> enemies, Character character)
    {

        foreach (var enemy in enemies)
        {
            SpecialEffectAdder.Instace.StunCallAtExternal(stunData, enemy);
        }
    }
}



