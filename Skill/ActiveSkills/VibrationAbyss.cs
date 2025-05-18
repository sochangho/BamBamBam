using UnityEngine;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;
public class VibrationAbyss : VibrationActiveSkill
{
    public override void UseSkill(float addAttack = 0)
    {
        base.UseSkill(addAttack);
        if (!GameSceneManager.Instance.IsMine(snakePartOwner.pv)) { return; }

        WaveCreate().Forget();
    }


    public override Skill Copy()
    {
        VibrationAbyss skill = new VibrationAbyss();

        skill.skillName = skillName;

        skill.id = id;
        skill.resourceid = resourceid;
        skill.soundname = soundname;

        return skill;
    }

    public async UniTask WaveCreate()
    {
        var head = snakePartOwner.GetComponent<SnakeHead>();

        for (int i = 0; i < head.snakeBodies.Count; ++i)
        {
            WaveAttack(resourceid,
                head.snakeBodies[i].transform.position, Quaternion.identity, snakePartOwner, head.snakeBodies[i].transform);
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f), cancellationToken: snakePartOwner.GetCancellationTokenOnDestroy());
        }
    }


    public override void EndActiveSkill()
    {

    }


}
