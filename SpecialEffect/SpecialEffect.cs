
using System.Collections.Generic;



/// <summary>
/// 특수효과
/// </summary>
public abstract class SpecialEffect
{
    public Character character;
    public Character.StatusEffect statusEffect;

    public bool duplicate = false;

    public List<ViewEffect> viewEffectList;

    protected bool isEnd;
    
    public bool IsEnd { get { return isEnd; } }
     

    public virtual void Start() 
    {
        isEnd = false;

        for(int i =0; i < viewEffectList.Count; ++i)
        {
            viewEffectList[i].View();
        }

    }
    
    public virtual void Update()
    { }

    public virtual void Remove() 
    {
        isEnd = true;

        for (int i = 0; i < viewEffectList.Count; ++i)
        {
            viewEffectList[i].ViewEnd();
        }
    }    
}
