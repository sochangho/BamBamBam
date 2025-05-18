using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SkillType
{
    Active,
    Passive,
    Collider,


    Size,
}

public abstract class Skill
{

    public int id;
    
    public SnakePart snakePartOwner;

    public string skillName;
    public string soundname;

    protected SkillType skillType;

    public void InitSnakePart(SnakePart snakePart) => snakePartOwner = snakePart;

    public SkillType GetSkillType() => skillType;

    public virtual void UseSkill(float addAttack = 0) 
    { 
        GameSoundManager.Instance.OnPlaySound(soundname, Sound.Effect); 
    }

    public abstract Skill Copy();
    
}
