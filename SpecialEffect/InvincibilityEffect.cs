

using UnityEngine;

public class InvincibilityEffect : SpecialEffect
{
    public float time;

    float currentTime;

    public InvincibilityEffect()
    {
        statusEffect = Character.StatusEffect.Invincibility;
        currentTime = 0;
    }

    public override void Start()
    {
        base.Start();
       
    }

    public override void Update()
    {
        base.Update();

       

        if (time > currentTime)
        {
            currentTime += Time.fixedDeltaTime;
        }
        else
        {
            if (!IsEnd)
            {
                Remove();
            }
        }

    }

    public override void Remove()
    {
        base.Remove();

    }

}
