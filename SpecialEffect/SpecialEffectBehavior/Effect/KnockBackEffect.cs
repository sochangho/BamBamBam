

using UnityEngine;

public class KnockBackEffect : SpecialEffect
{

    public Vector3 direction;
    public float intensity;

    float copySpeed ;
    float copyDeceleration;
    float jerk;


    public KnockBackEffect()
    {
        statusEffect = Character.StatusEffect.KnockBack;

       copySpeed = Values.KnockBackMoveSpeedConstance;
       copyDeceleration = Values.KnockBackDeceleration;
       jerk = Values.KnockBackJerk;

    }

    public override void Start()
    {
        base.Start();
       
       
    }

    public override void Update()
    {
        base.Update();

        if (copySpeed > 0)
        {

            float clientSpeed = Values.SpeedConstant * copySpeed * intensity * 0.1f;

            character.SetNotMineClientSpeed(clientSpeed);

            character.transform.position += direction * clientSpeed * Time.fixedDeltaTime ;

            copyDeceleration += jerk;
            copySpeed -= copyDeceleration;

            if (copySpeed <= 0)
            {
                copySpeed = 0;
            }

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
