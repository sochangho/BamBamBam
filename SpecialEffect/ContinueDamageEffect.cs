

using UnityEngine;

public class ContinueDamageEffect : SpecialEffect
{

    public float value;
    public float time;
    public float intervalTime;

    private float currentTime;
    private float currentIntervalTime;

    public ContinueDamageEffect()
    {
        statusEffect = Character.StatusEffect.ContinueDamage;
        currentTime = 0;
        currentIntervalTime = 0;
    }

    public override void Update()
    {
        base.Update();


        if (time > currentTime)
        {
            currentTime += Time.fixedDeltaTime;
            DamageToCharacter();
        }
        else
        {
            if (!IsEnd)
            {
                Remove();
            }
        }

    }

    public void DamageToCharacter()
    {
        if(intervalTime > currentIntervalTime)
        {
            currentIntervalTime += Time.fixedDeltaTime;
            return;
        }

        currentIntervalTime = 0;

        character.Attacked(value);
    }

}
