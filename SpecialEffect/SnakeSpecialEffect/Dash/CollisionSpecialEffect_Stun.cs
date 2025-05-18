
//스턴 , 스피드  , 넉백 , 화상 , 방어력 감소

public class CollisionSpecialEffect_Stun : CollisionSpecialEffect
{
    public StunData stunData;

    protected override void SpecialEffect(Enemy enemy, Character character)
    {
        SpecialEffectAdder.Instace.StunCallAtExternal(stunData, enemy);
    }
} 

