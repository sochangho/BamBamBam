using UnityEngine;
using System.Collections;
public class VibrationSendStom : VibrationActiveSkill
{
    public override void UseSkill(float addAttack = 0)
    {
        base.UseSkill(addAttack);
        if (!GameSceneManager.Instance.IsMine(snakePartOwner.pv)) { return; }

        var head = snakePartOwner.GetComponent<SnakeHead>();
     
        WaveAttack(resourceid, head.transform.position, Quaternion.identity, snakePartOwner, head.transform);
    }



    public override Skill Copy()
    {
        VibrationSendStom skill = new VibrationSendStom();

        skill.skillName = skillName;

        skill.id = id;
        skill.resourceid = resourceid;
        skill.soundname = soundname;

        return skill;
    }

    public override void EndActiveSkill()
    {

    }


}



