using System.Collections.Generic;

public class WaveSpecialEffect_Stun : WaveSpecialEffect
{
    public StunData stunData;
    public override void SpecialEffect(BamVibrationParticleBase wave, IEnumerable<Enemy> enemies, Character character)
    {
        foreach (var enemy in enemies)
        {
            SpecialEffectAdder.Instace.StunCallAtExternal(stunData, enemy);
        }
    }
}