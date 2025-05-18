using System.Collections.Generic;

public class WaveSpecialEffect_ContinueDamage : WaveSpecialEffect
{
    public ContinueDamageData continueDamageData;

    public override void SpecialEffect(BamVibrationParticleBase wave, IEnumerable<Enemy> enemies, Character character)
    {
        foreach (var enemy in enemies)
        {
            SpecialEffectAdder.Instace.ContinueDamageCallAtExternal(continueDamageData,wave.GetMineCharacter(),enemy);
        }
    }
}


