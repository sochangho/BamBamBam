

using UnityEngine;

public class SpeedSpecialEffect : SpecialEffect
{
    public float value;//[0~1]

    public float time;

    private float currentTime;

    private int statID;

    public SpeedSpecialEffect()
    {
        statusEffect = Character.StatusEffect.Speed;
    }

    public override void Start()
    {
        base.Start();

        //TODO : 방어력증가 OR 감소

        float calculate = character.ability.speedOrigine * value;

        statID = GameStatUpgrader.Instace.GetMakeID();

        character.ability.abilityData.speed.Apply(
            new System.Collections.Generic.List<StatData<float>> {
            new StatData<float>()
            {
                DataKey = statID,
                DataValue = calculate
            }});
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
        character.ability.abilityData.speed.Remove(statID);
    }
}
