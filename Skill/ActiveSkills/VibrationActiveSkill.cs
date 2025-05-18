using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class VibrationActiveSkill : ActiveSkill
{
    public int resourceid;

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



}
