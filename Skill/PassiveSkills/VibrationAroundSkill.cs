using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class VibrationAroundSkill : PassiveSkill
{

    public int resourceId;


    public void WaveAttack(int resouceid, Vector2 postion, Quaternion quaternion, Character subject, Transform target)
    {

        ObjectPoolManager.Instance.GetObject(BamGameObjectType.Obstacle, resouceid,
              postion, quaternion, subject, (iobj) => {

                  if (target != null)
                  {
                      if (iobj is BamVibrationParticle)
                      {
                          var p = iobj as BamVibrationParticle;

                          p.SetFollow(target);
                      }
                  }

              });

    }

    public override void UseSkill(float addAttack = 0)
    {
        base.UseSkill();
        if (!GameSceneManager.Instance.IsMine(snakePartOwner.pv)) { return; }

        SpawnWave().Forget();        
    }


    public async UniTask SpawnWave()
    {
        var head = snakePartOwner.GetComponent<SnakeHead>();

        for (int i = 0; i < head.snakeBodies.Count; ++i)
        {
            var snakeBody = head.snakeBodies[i];

            WaveAttack(resourceId, snakeBody.transform.position,
               Quaternion.AngleAxis(90, new Vector3(0, 0, 1)), snakePartOwner, snakeBody.transform);

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.2f), cancellationToken: snakePartOwner.GetCancellationTokenOnDestroy());
        }

    }


    public override Skill Copy()
    {
        VibrationAroundSkill skill = new VibrationAroundSkill();

        skill.skillName = skillName;
        skill.durationTime = durationTime;
        skill.duraitionType = duraitionType;    
        skill.id = id;
        skill.resourceId = resourceId;
        skill.soundname = soundname;
        return skill;
    }
    
}




