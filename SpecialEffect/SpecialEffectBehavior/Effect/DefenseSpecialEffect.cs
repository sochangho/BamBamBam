

using UnityEngine;

public class DefenseSpecialEffect : SpecialEffect
{
    public float value;//[0~1]

    public float time;

    private float currentTime;

    private int statID;
  
    public DefenseSpecialEffect()
    {
        statusEffect = Character.StatusEffect.Defense;
    }

    public override void Start()
    {
        base.Start();

        //TODO : 방어력증가 OR 감소

        float calculate = character.ability.defenceOrigine * value;

        statID = GameStatUpgrader.Instace.GetMakeID();

        character.ability.abilityData.defence.Apply(
            new System.Collections.Generic.List<StatData<float>> {

            new StatData<float>()
            {
                DataKey = statID,
                DataValue = calculate
            }

            });

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
        character.ability.abilityData.defence.Remove(statID);
    }
}
