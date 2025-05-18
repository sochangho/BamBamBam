using System;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

using ExitGames.Client.Photon;

public class SpecialEffectAdder : SingletonAutoCreate<SpecialEffectAdder>
{

    public const int effect_number_stun = 0;
    public const int effect_number_knockback = 1;
    public const int effect_number_Invincibility = 2;
    public const int effect_number_Defense = 3;
    public const int effect_number_ConDamage = 4;
    public const int effect_number_Speed = 5;


    public SpecialEffectSender sender;
    public SpecialEffectReceiver receiver;


    protected override void Awake()
    {
        base.Awake();
        sender = new SpecialEffectSender(this);
        receiver = new SpecialEffectReceiver(this);
    }

    public void OnEnable()
    {
        receiver.OnEnable();
    }

    public void OnDisable()
    {
        receiver.OnDisable();
    }

    public bool SpecialEffectAddable(Character.StatusEffect statusEffect , Character subject)
    {
        return subject.receiveSpecialEffectController.SpecialEffectAddable(statusEffect);
    }


    #region External Public
    public void StunCallAtExternal(StunData data , Character subject)
    {
        if (!RandomManager.RandomProbability(data.common.probability)) return;

        EffectObjectData[] effectObjectDatas = data.common.effectobjectdatas;

        Span<EffectObjectInfo> effectObjectInfos = stackalloc EffectObjectInfo[effectObjectDatas.Length];
            
     
        for (int i = 0; i < effectObjectDatas.Length; ++i)
        {
            int viewtype = (int)effectObjectDatas[i].viewType;
            int id = effectObjectDatas[i].resourceid;

            var info = new EffectObjectInfo();
            info.effectViewType = viewtype;
            info.resourceid = id;

            effectObjectInfos[i] = info;
        }

        Stun(effectObjectInfos, subject, data.time);
    }

    public void KnockBackCallAtExternal(KnockbackData data ,in Vector2 direction, Character subject)
    {
        if (!RandomManager.RandomProbability(data.common.probability)) return;

        EffectObjectData[] effectObjectDatas = data.common.effectobjectdatas;
        Span<EffectObjectInfo> effectObjectInfos = stackalloc EffectObjectInfo[effectObjectDatas.Length];

        for (int i = 0; i < effectObjectDatas.Length; ++i)
        {
            int viewtype = (int)effectObjectDatas[i].viewType;
            int id = effectObjectDatas[i].resourceid;

            var info = new EffectObjectInfo();
            info.effectViewType = viewtype;
            info.resourceid = id;

            effectObjectInfos[i] = info;
        }

        KnockBack(effectObjectInfos, subject, direction, data.intensity);
    }

    public void InvincibilityCallAtExternal(InvincibilityData data, Character subject)
    {
        if (!RandomManager.RandomProbability(data.common.probability)) return;

        EffectObjectData[] effectObjectDatas = data.common.effectobjectdatas;
        Span<EffectObjectInfo> effectObjectInfos = stackalloc EffectObjectInfo[effectObjectDatas.Length];

        for (int i = 0; i < effectObjectDatas.Length; ++i)
        {
            int viewtype = (int)effectObjectDatas[i].viewType;
            int id = effectObjectDatas[i].resourceid;

            var info = new EffectObjectInfo();
            info.effectViewType = viewtype;
            info.resourceid = id;

            effectObjectInfos[i] = info;
        }
        Invincibility(effectObjectInfos, subject, data.time);
    }

    public void DefenseEffectCallAtExternal(DefenseData data , Character subject)
    {
        if (!RandomManager.RandomProbability(data.common.probability)) return;


        EffectObjectData[] effectObjectDatas = data.common.effectobjectdatas;
        Span<EffectObjectInfo> effectObjectInfos = stackalloc EffectObjectInfo[effectObjectDatas.Length];

        for (int i = 0; i < effectObjectDatas.Length; ++i)
        {
            int viewtype = (int)effectObjectDatas[i].viewType;
            int id = effectObjectDatas[i].resourceid;

            var info = new EffectObjectInfo();
            
            info.effectViewType = viewtype;
            info.resourceid = id;
            
            effectObjectInfos[i] = info;
        }
        Defense(effectObjectInfos, subject, data.value, data.time);
    }

    public void SpeedEffectCallAtExternal(SpeedData data, Character subject)
    {
        if (!RandomManager.RandomProbability(data.common.probability)) return;


        EffectObjectData[] effectObjectDatas = data.common.effectobjectdatas;

        Span<EffectObjectInfo> effectObjectInfos = stackalloc EffectObjectInfo[effectObjectDatas.Length];

        for (int i = 0; i < effectObjectDatas.Length; ++i)
        {
            int viewtype = (int)effectObjectDatas[i].viewType;
            int id = effectObjectDatas[i].resourceid;

            var info = new EffectObjectInfo();

            info.effectViewType = viewtype;
            info.resourceid = id;

            effectObjectInfos[i] = info;
        }
        Speed(effectObjectInfos, subject, data.value, data.time);
    }

    public void ContinueDamageCallAtExternal(ContinueDamageData data ,Character mine ,Character subject)
    {

        if (!RandomManager.RandomProbability(data.common.probability)) return;

        EffectObjectData[] effectObjectDatas = data.common.effectobjectdatas;

        Span<EffectObjectInfo> effectObjectInfos = stackalloc EffectObjectInfo[effectObjectDatas.Length];

        for (int i = 0; i < effectObjectDatas.Length; ++i)
        {
            int viewtype = (int)effectObjectDatas[i].viewType;
            int id = effectObjectDatas[i].resourceid;

            var info = new EffectObjectInfo();

            info.effectViewType = viewtype;
            info.resourceid = id;

            effectObjectInfos[i] = info;
        }

       
        float damage = mine.characterAttack.GetAttackDamagePercent(subject, data.value);

        ContinueDamage(effectObjectInfos, subject, damage, data.time, data.intervalTime);
    }

    public void ContinueDamageCallAtExternalObstacleArea(ContinueDamageData data, Character subject)
    {

        if (!RandomManager.RandomProbability(data.common.probability)) return;

        EffectObjectData[] effectObjectDatas = data.common.effectobjectdatas;
        Span<EffectObjectInfo> effectObjectInfos = stackalloc EffectObjectInfo[effectObjectDatas.Length];

        for (int i = 0; i < effectObjectDatas.Length; ++i)
        {
            int viewtype = (int)effectObjectDatas[i].viewType;
            int id = effectObjectDatas[i].resourceid;

            var info = new EffectObjectInfo();

            info.effectViewType = viewtype;
            info.resourceid = id;

            effectObjectInfos[i] = info;
        }
        
        float damage = subject.hpTotal * data.value;
        ContinueDamage(effectObjectInfos, subject, damage, data.time, data.intervalTime);
    }



    #endregion


    private void Stun(Span<EffectObjectInfo> objInfoList , Character character 
        , float time )
    {
        if (character == null) return;

        if (GameSceneManager.Instance.IsMine(character.pv))
        {
            if (!SpecialEffectAddable(Character.StatusEffect.Stun, character)) return;
            

            ViewEffect[] viewEffects = ToViewEffects(objInfoList,character);
            StunEffect stunEffect = new StunEffect();
            stunEffect.character = character;
            stunEffect.time = time;
            stunEffect.viewEffectList = viewEffects;
            SpecialEffectToLocalCharacter(character,stunEffect);
            return;
        }

        StunEffectNetConfig stunEffectConfig = new StunEffectNetConfig();
        stunEffectConfig.common = new CommonEffectNetConfig();
        stunEffectConfig.common.character = character is Enemy ? 1 : 0;
        stunEffectConfig.common.viewid = character.photonView.ViewID;
        stunEffectConfig.common.effectObjectInfos = objInfoList.ToArray();
        stunEffectConfig.time = time;
        string data  = JsonUtility.ToJson(stunEffectConfig);
        SpecialEffectToOpponentClientSend(effect_number_stun, data);
    }

    private void KnockBack(Span<EffectObjectInfo> objInfoList , Character character
        ,Vector2 direction , float intensity)
    {
        if (character == null) return;

        if (GameSceneManager.Instance.IsMine(character.pv))
        {

            if (!SpecialEffectAddable(Character.StatusEffect.KnockBack, character)) return;

            ViewEffect[] viewEffects = ToViewEffects(objInfoList, character);

            KnockBackEffect knockBackEffect = new KnockBackEffect();
            knockBackEffect.character = character;
            knockBackEffect.viewEffectList = viewEffects;
            knockBackEffect.direction = direction;
            knockBackEffect.intensity = intensity;

            SpecialEffectToLocalCharacter(character, knockBackEffect);

            return;
        }
        
        KnockBackNetConfig knockBackConfig = new KnockBackNetConfig();
        knockBackConfig.common = new CommonEffectNetConfig();

        knockBackConfig.common.character = character is Enemy ? 1 : 0;
        knockBackConfig.common.viewid = character.photonView.ViewID;
        knockBackConfig.common.effectObjectInfos = objInfoList.ToArray();
        knockBackConfig.x = direction.x;
        knockBackConfig.y = direction.y;
        knockBackConfig.intensity = intensity;

        string data = JsonUtility.ToJson(knockBackConfig);

        SpecialEffectToOpponentClientSend(effect_number_knockback, data);
    }

    private void Invincibility(Span<EffectObjectInfo> objInfoList, Character character
        , float time)
    {
        if (character == null) return;

        if (GameSceneManager.Instance.IsMine(character.pv))
        {
            if (!SpecialEffectAddable(Character.StatusEffect.Invincibility, character)) return;

            ViewEffect[] viewEffects = ToViewEffects(objInfoList, character);

            InvincibilityEffect invincibilityEffect = new InvincibilityEffect();
            invincibilityEffect.character = character;
            invincibilityEffect.time = time;
            invincibilityEffect.viewEffectList = viewEffects;
            SpecialEffectToLocalCharacter(character, invincibilityEffect);
            return;
        }

        InvincibilityEffectNetConfig config = new InvincibilityEffectNetConfig();
        config.common = new CommonEffectNetConfig();
        config.common.character = character is Enemy ? 1 : 0;
        config.common.viewid = character.photonView.ViewID;
        config.common.effectObjectInfos = objInfoList.ToArray();
        config.time = time;
        string data = JsonUtility.ToJson(config);
        SpecialEffectToOpponentClientSend(effect_number_Invincibility, data);

    }

    private void Defense(Span<EffectObjectInfo> objectInfos , Character character,float value,float time)
    {
        if (character == null) return;

        if (GameSceneManager.Instance.IsMine(character.pv))
        {
            if (!SpecialEffectAddable(Character.StatusEffect.Defense, character)) return;

            ViewEffect[] viewEffects = ToViewEffects(objectInfos, character);

            DefenseSpecialEffect defenseEffect = new DefenseSpecialEffect();
            defenseEffect.character = character;
            defenseEffect.time = time;
            defenseEffect.value = value;
            defenseEffect.viewEffectList = viewEffects;
            SpecialEffectToLocalCharacter(character, defenseEffect);
            return;
        }

        DefenseEffectNetConfig config = new DefenseEffectNetConfig();
        config.common = new CommonEffectNetConfig();
        config.common.character = character is Enemy ? 1 : 0;
        config.common.viewid = character.photonView.ViewID;
        config.common.effectObjectInfos = objectInfos.ToArray();
        config.time = time;
        config.value = value;

        string data = JsonUtility.ToJson(config);
        SpecialEffectToOpponentClientSend(effect_number_Defense, data);
    }

    private void Speed(Span<EffectObjectInfo> objectInfos, Character character, float value, float time)
    {
        if (character == null) return;

        if (GameSceneManager.Instance.IsMine(character.pv))
        {
            if (!SpecialEffectAddable(Character.StatusEffect.Speed, character)) return;

            ViewEffect[] viewEffects = ToViewEffects(objectInfos, character);

            SpeedSpecialEffect effect = new SpeedSpecialEffect();
            effect.character = character;
            effect.time = time;
            effect.value = value;
            effect.viewEffectList = viewEffects;
            SpecialEffectToLocalCharacter(character, effect);
            return;
        }

        SpeedEffectNetConfig config = new SpeedEffectNetConfig();
        config.common = new CommonEffectNetConfig();
        config.common.character = character is Enemy ? 1 : 0;
        config.common.viewid = character.photonView.ViewID;
        config.common.effectObjectInfos = objectInfos.ToArray();
        config.time = time;
        config.value = value;

        string data = JsonUtility.ToJson(config);
        SpecialEffectToOpponentClientSend(effect_number_Speed, data);
    }

    private void ContinueDamage(Span<EffectObjectInfo> objectInfos, Character character, float value,
        float time , float intervalTime)
    {
        if (character == null) return;

        if (GameSceneManager.Instance.IsMine(character.pv))
        {
            if (!SpecialEffectAddable(Character.StatusEffect.ContinueDamage,character)) return;

            ViewEffect[] viewEffects = ToViewEffects(objectInfos, character);

            ContinueDamageEffect effect = new ContinueDamageEffect();
            effect.character = character;
            effect.time = time;
            effect.value = value;
            effect.intervalTime = intervalTime;
            effect.viewEffectList = viewEffects;
            SpecialEffectToLocalCharacter(character, effect);
            return;
        }
        ContinueDamageEffectNetConfig config = new ContinueDamageEffectNetConfig();
        config.common = new CommonEffectNetConfig();
        config.common.character = character is Enemy ? 1 : 0;
        config.common.viewid = character.photonView.ViewID;
        config.common.effectObjectInfos = objectInfos.ToArray();
        config.time = time;
        config.value = value;
        config.intervalTime = intervalTime;

        string data = JsonUtility.ToJson(config);
        SpecialEffectToOpponentClientSend(effect_number_ConDamage, data);
    }



    public void SpecialEffectToOpponentClientReceive(int typenumber, string data)
    {
        if (typenumber.Equals(effect_number_stun))
        {
            ExecuteStun(data);
        }
        else if (typenumber.Equals(effect_number_knockback))
        {
            ExecuteKnockback(data);
        }
        else if (typenumber.Equals(effect_number_Invincibility))
        {
            ExecuteInvincibility(data);
        }
        else if (typenumber.Equals(effect_number_Defense))
        {
            ExecuteDefenseEffect(data);
        }
        else if (typenumber.Equals(effect_number_Speed))
        {
            ExecuteSpeedEffect(data);
        }
        else if (typenumber.Equals(effect_number_ConDamage))
        {
            ExecuteContinueDamage(data);
        }

    }

    #region private



    private ViewEffect[] ToViewEffects(Span<EffectObjectInfo> infos , Character character)
    {
        ViewEffect[] viewEffects = new ViewEffect[infos.Length];
     
        for(int i = 0; i < infos.Length; ++i)
        {
            ViewEffect.ViewType viewType = (ViewEffect.ViewType)infos[i].effectViewType;
            int id = infos[i].resourceid;

            ViewEffect viewEffect = default(ViewEffect);

            if(viewType == ViewEffect.ViewType.Pariticle)
            {
                viewEffect = new ViewParticleEffect(character, id);
            }

            viewEffects[i] = viewEffect;
        }
        return viewEffects;
    }


    private void SpecialEffectToLocalCharacter(Character character , SpecialEffect specialEffect)
    {
        character.SpecialEffectAdd(specialEffect);
    } 

    private void SpecialEffectToOpponentClientSend(int typenumber , string sendData) 
        => sender.SendSpecialEffect(typenumber, sendData);
    

  
    private void ExecuteStun(string data)
    {
        StunEffectNetConfig stunEffectConfig
               = JsonUtility.FromJson<StunEffectNetConfig>(data);

        int c = stunEffectConfig.common.character;
        int viewid = stunEffectConfig.common.viewid;

        EffectObjectInfo[] effectObjectInfos = stunEffectConfig.common.effectObjectInfos;

        float time = stunEffectConfig.time;


        Character character = FindCharacter(c, viewid);

        Span<EffectObjectInfo> infos = stackalloc EffectObjectInfo[effectObjectInfos.Length];

        for(int i = 0; i < infos.Length; ++i)
        {
            infos[i] = effectObjectInfos[i]; 
        }

        
        Stun(infos, character, time);
    }

    private void ExecuteKnockback(string data)
    {
        KnockBackNetConfig knockBackConfig
              = JsonUtility.FromJson<KnockBackNetConfig>(data);

        int c = knockBackConfig.common.character;
        int viewid = knockBackConfig.common.viewid;

        EffectObjectInfo[] effectObjectInfos = knockBackConfig.common.effectObjectInfos;

        float x = knockBackConfig.x;
        float y = knockBackConfig.y;

        float intensity = knockBackConfig.intensity;

        Character character = FindCharacter(c, viewid);

        Span<EffectObjectInfo> infos = stackalloc EffectObjectInfo[effectObjectInfos.Length];

        for (int i = 0; i < infos.Length; ++i)
        {
            infos[i] = effectObjectInfos[i];
        }

        KnockBack(infos, character, new Vector2(x, y), intensity);
    }

    private void ExecuteInvincibility(string data)
    {
        InvincibilityEffectNetConfig config
            = JsonUtility.FromJson<InvincibilityEffectNetConfig>(data);

        int c = config.common.character;
        int viewid = config.common.viewid;

        EffectObjectInfo[] effectObjectInfos = config.common.effectObjectInfos;

        float time = config.time;
        
        Character character = FindCharacter(c, viewid);

        Span<EffectObjectInfo> infos = stackalloc EffectObjectInfo[effectObjectInfos.Length];

        for (int i = 0; i < infos.Length; ++i)
        {
            infos[i] = effectObjectInfos[i];
        }

        Invincibility(infos,character,time);
    }

    private void ExecuteDefenseEffect(string data)
    {
        DefenseEffectNetConfig config = JsonUtility.FromJson<DefenseEffectNetConfig>(data);

        int c = config.common.character;
        int viewid = config.common.viewid;

        EffectObjectInfo[] effectObjectInfos = config.common.effectObjectInfos;

        float time = config.time;
        float value = config.value;

        Character character = FindCharacter(c, viewid);

        Span<EffectObjectInfo> infos = stackalloc EffectObjectInfo[effectObjectInfos.Length];

        for (int i = 0; i < infos.Length; ++i)
        {
            infos[i] = effectObjectInfos[i];
        }

        Defense(infos, character, value, time);
    }


    private void ExecuteSpeedEffect(string data)
    {
        SpeedEffectNetConfig config = JsonUtility.FromJson<SpeedEffectNetConfig>(data);

        int c = config.common.character;
        int viewid = config.common.viewid;

        EffectObjectInfo[] effectObjectInfos = config.common.effectObjectInfos;

        float time = config.time;
        float value = config.value;

        Character character = FindCharacter(c, viewid);

        Span<EffectObjectInfo> infos = stackalloc EffectObjectInfo[effectObjectInfos.Length];

        for (int i = 0; i < infos.Length; ++i)
        {
            infos[i] = effectObjectInfos[i];
        }

        Speed(infos, character, value, time);
    }

    private void ExecuteContinueDamage(string data)
    {
        ContinueDamageEffectNetConfig config = JsonUtility.FromJson<ContinueDamageEffectNetConfig>(data);

        int c = config.common.character;
        int viewid = config.common.viewid;

        EffectObjectInfo[] effectObjectInfos = config.common.effectObjectInfos;

        float time = config.time;
        float intervalTime = config.intervalTime;
        float value = config.value;

        Character character = FindCharacter(c, viewid);

        Span<EffectObjectInfo> infos = stackalloc EffectObjectInfo[effectObjectInfos.Length];

        for (int i = 0; i < infos.Length; ++i)
        {
            infos[i] = effectObjectInfos[i];
        }

        ContinueDamage(infos, character, value, time, intervalTime);
    }



    private Character FindCharacter(int chractertype , int viewid)
    {
        Character character = chractertype == 1 ?
          GameSceneManager.Instance.GetSpwaner().GetFindEnemyByViewId(viewid)
          : GameSceneManager.Instance.MineHead();
        return character;
    }

    #endregion
}




public class SpecialEffectSender
{
    private SpecialEffectAdder effectAdder;

    public SpecialEffectSender(SpecialEffectAdder specialEffectAdder)
    {
        effectAdder = specialEffectAdder;
    }

    public void SendSpecialEffect(int numbertype , string data)
    {
        object[] content = new object[2];
        content[0] = numbertype;
        content[1] = data;

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(EventCodes.SpecialEffectCode, content, raiseEventOptions, sendOptions);
    }

}

public class SpecialEffectReceiver: IOnEventCallback
{
    private SpecialEffectAdder effectAdder;

    public SpecialEffectReceiver(SpecialEffectAdder specialEffectAdder)
    {
        effectAdder = specialEffectAdder;
    }

    public void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    
    public void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);

    
    public void OnEvent(EventData photonEvent)
    {
        byte code = photonEvent.Code;

        if(code == EventCodes.SpecialEffectCode)
        {
            Receive(photonEvent);
        }
    }

    public void Receive(EventData eventData)
    {
        object[] data = (object[])eventData.CustomData;

        int numtype = (int)data[0];

        string jsondata = (string)data[1];

        effectAdder.SpecialEffectToOpponentClientReceive(numtype, jsondata);
    }

}

