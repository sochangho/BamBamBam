using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;



/// <summary>
/// ĳ���� ������ ����
/// </summary>
public class DamageSync
{
    public Character character;

    public Queue<DamageSyncElement> damageQueue =
        new Queue<DamageSyncElement>();

    Queue<float> damagedNumberQueue = new Queue<float>();
    Queue<float> healQueue = new Queue<float>();

    
    public DamageSync(Character character)
    {
        this.character = character;
    }



    /// <summary>
    /// ������ �߰�
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="isMine"></param>
    public void AddHpCalculate(float damage,bool isMine)
    {

        DamageSyncElement damageSyncElement = new DamageSyncElement();
        damageSyncElement.isMine = isMine;
        damageSyncElement.damage = damage;

        damageQueue.Enqueue(damageSyncElement);
    }


    /// <summary>
    /// ������ ó��
    /// </summary>
    private void ExecuteDamage()
    {
        damagedNumberQueue.Clear();
        healQueue.Clear();

        while (damageQueue.Count > 0 && character.gameObject.activeSelf) {

            if (character.Hp == 0)
            {
                damageQueue.Clear();
                
                return;
            }

            if (character.isDefence)
            {
                damageQueue.Clear();
                character.DecreaseHpPopup(0);
                return;
            }

            DamageSyncElement element = damageQueue.Dequeue();

            if(element.damage >= 0)
            {
                //Heal               
                healQueue.Enqueue(element.damage);
            }
            else
            {
                //Damage
                damagedNumberQueue.Enqueue(element.damage);

                if(character is SnakePart)
                {
                    SnakePart snake = character as SnakePart;
                    GameSceneManager.Instace.recoderDamage.AddDamage(element.damage);                
                }
            }

            float currentHp = character.Hp + element.damage;


            if (currentHp <= 0)
            {
                currentHp = 0;

            }
            else if(currentHp > character.hpTotal)
            {
                currentHp = character.hpTotal;
            }

            if (element.isMine)
                EnemyAttackEvent(currentHp);
            else
                OpponentEnemyAttackEvent(currentHp);

            character.SetHp(currentHp);        
        }

        DamageNumberActive().Forget();
        HealNumberActive().Forget();
    }
    

    /// <summary>
    /// ť Ŭ����
    /// </summary>
    public void Clear()
    {
        damageQueue.Clear();        
    }



    /// <summary>
    /// ������ ó�� �Լ� ����
    /// </summary>
    public void Update()
    {
        if (GameSceneManager.Instace.IsMine(character.pv))
        {
            ExecuteDamage();
        }
    }

    /// <summary>
    /// �� ���� �̺�Ʈ ó��
    /// </summary>
    /// <param name="hp"></param>
    public void EnemyAttackEvent(float hp)
    {
        if(character is Enemy)
        {
           var e = character as Enemy;

            e.OnEnemyAttacked(hp);
        }
    }

    /// <summary>
    /// ���� Local�� �ƴ� ��� �̺�Ʈ ó��
    /// </summary>
    /// <param name="hp"></param>
    public void OpponentEnemyAttackEvent(float hp)
    {
        if (character is Enemy)
        {
            var e = character as Enemy;

            e.OnEnemyAttackedAtOpponent(hp);
        }
    }


    /// <summary>
    /// ������ �˾�
    /// </summary>
    /// <returns></returns>
    public async UniTask DamageNumberActive()
    {
        while(damagedNumberQueue.Count > 0)
        {
            character.DecreaseHpPopup(damagedNumberQueue.Dequeue());

            await UniTask.WaitForSeconds(0.1f,false,PlayerLoopTiming.Update,
                this.character.gameObject.GetCancellationTokenOnDestroy());
        }
    }

    /// <summary>
    /// ȸ�� �˾�
    /// </summary>
    /// <returns></returns>
    public async UniTask HealNumberActive()
    {
        while (healQueue.Count > 0)
        {
            character.IncreaseHpPopup(healQueue.Dequeue());

            await UniTask.WaitForSeconds(0.1f, false, PlayerLoopTiming.Update,
                this.character.gameObject.GetCancellationTokenOnDestroy());
        }
    }
         
}

public struct DamageSyncElement
{
    public bool isMine;
    public float damage;
}


public abstract class ValueSync<T>
{
    protected T syncedValue;

    public event System.Action<T> ChangedValue;

    public T SyncedValue
    {
        get
        {
            return syncedValue;
        }
        set
        {
            syncedValue = value;
            ChangedValue?.Invoke(syncedValue);
        }
    }

 


    public Queue<T> queueValue = new Queue<T>();

    public void Add(T v)
    {
        queueValue.Enqueue(v);
    }

    public void UpdateCount()
    {
        if(queueValue.Count > 0)
        {
            T v = queueValue.Dequeue();
            Calulate(v);
        }
    }

    public abstract void Calulate(T v);


}



public class ValueIntSync : ValueSync<int>
{
    public override void Calulate(int v)
    {
        SyncedValue += v;
    }
}
