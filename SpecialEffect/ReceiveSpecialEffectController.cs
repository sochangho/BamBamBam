using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

using KV = System.Collections.Generic.KeyValuePair<Character.StatusEffect, SpecialEffect>;

/// <summary>
/// 특수효과 Controller
/// </summary>
public class ReceiveSpecialEffectController
{
    private Queue<SpecialEffect>[] reservationAddEffect; 
       
    private Queue<KV> reservationRemoveEffect 
        = new Queue<KV>();
        
    bool isStop = false;

    private List<SpecialEffect>[] specialEffects;
   
    readonly private int typeLength;

    public ReceiveSpecialEffectController()
    {
       typeLength =  Enum.GetValues(typeof(Character.StatusEffect)).Length;

       specialEffects = new List<SpecialEffect>[typeLength];
        reservationAddEffect = new Queue<SpecialEffect>[typeLength];


       for(int i = 0; i < typeLength; ++i)
       {
            specialEffects[i] = new List<SpecialEffect>();
            reservationAddEffect[i] = new Queue<SpecialEffect>();
       }
    }

    #region External
    public void Add(Character.StatusEffect effectType,  SpecialEffect specialEffect)
    {        
        if (isStop) return;        
        reservationAddEffect[(int)effectType].Enqueue(specialEffect);
    }
    
    public void RoutineSpecialEffects()
    {
        if (isStop) return;
    
        AddReservation();
        UpdateSpecialEffect();
        RemoveReservation();
        Remove();
    }

    public void ExcuteAllRemoveFromOutSide()
    {
        
        Stop();

        for (int i = 0; i < typeLength; ++i)
        {
            reservationAddEffect[i].Clear();
        }

        
        AllRemove();
        
        for(int i = 0; i < typeLength; ++i)
        {
            specialEffects[i].Clear();
        }
    }

    public bool IsMoveStop()
    {
        return specialEffects[(int)Character.StatusEffect.Stun].Count > 0 
            || specialEffects[(int)Character.StatusEffect.KnockBack].Count > 0;
    }

    public bool IsInvincibility()
    {
        return specialEffects[(int)Character.StatusEffect.Invincibility].Count > 0; 
    }


    public int SpecialEffectCount()
    {
        int count = 0;

        for(int i = 0; i < typeLength; ++i)
        {
            count += reservationAddEffect[i].Count;
        }

        for(int i =0; i < typeLength; ++i)
        {
            count += specialEffects[i].Count;           
        }
        return count;
    }

   

    public override string ToString()
    {
        string s = string.Empty;

        for(int i = 0; i < typeLength; ++i)
        {
            if (specialEffects[i].Count > 0)
            {
                 s += ((Character.StatusEffect)i).ToString() + ", ";
            }
        }

        return s;
    }


    public bool SpecialEffectAddable(Character.StatusEffect effectType)
    {
        return specialEffects[(int)effectType].Count == 0 
            && reservationAddEffect[(int)effectType].Count == 0 && SpecialEffectCount() < TemporaryValues.SPECIAL_EFFECT_MAX_COUNT;
    }

    public void Play()
    {
        isStop = false;
    }

    public void Stop()
    {
        isStop = true;
    }

    #endregion

    private void AddReservation()
    {

        for (int i = 0; i < typeLength; ++i)
        {
            while (reservationAddEffect[i].Count > 0)
            {               
                Character.StatusEffect type = (Character.StatusEffect)i;
                SpecialEffect specialEffect = reservationAddEffect[i].Dequeue();
                var typeEffects = specialEffects[(int)type];
                typeEffects.Add(specialEffect);
                typeEffects[typeEffects.Count - 1].Start();
            }
        }
    }

    private void UpdateSpecialEffect()
    {
        for(int i = 0;  i < typeLength; ++i)
        {
            for(int j = 0; j < specialEffects[i].Count; ++j)
            {
                specialEffects[i][j].Update();
            }
        }
    }

    private void RemoveReservation()
    {
        for (int i = 0; i < typeLength; ++i)
        {
            for (int j = 0; j < specialEffects[i].Count; ++j)
            {
                if (specialEffects[i][j].IsEnd)
                {                    
                    KV kv = new KV((Character.StatusEffect)i , specialEffects[i][j]);

                    reservationRemoveEffect.Enqueue(kv);
                }
            }
        }
    }
    
    private void Remove()
    {
        while (reservationRemoveEffect.Count > 0)
        {
            KV kv = reservationRemoveEffect.Dequeue();
            
            int type = (int)kv.Key;
        
           
            specialEffects[type].Remove(kv.Value);
        }
    }
    
    private void AllRemove()
    {
        for (int i = 0; i < typeLength; ++i)
        {
            for (int j = 0; j < specialEffects[i].Count; ++j)
            {
                if (!specialEffects[i][j].IsEnd)
                {
                    specialEffects[i][j].Remove();
                }
            }
        }        
    }

    


}
