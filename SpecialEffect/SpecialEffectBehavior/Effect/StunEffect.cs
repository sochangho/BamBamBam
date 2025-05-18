
using System.Collections;
using Cysharp.Threading.Tasks;


using UnityEngine;

public class StunEffect : SpecialEffect
{
    public float time;

    float currentTime;

    public StunEffect()
    {
        statusEffect = Character.StatusEffect.Stun;
        currentTime = 0;
    }

    public override void Start()
    {        
        base.Start();      
        character.AnimStop();      
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
        character.AnimPlay();        
    }
}

