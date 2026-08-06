using UnityEngine;

namespace MySample
{
    /// <summary>
    /// 몬스터 타입 정의
    /// </summary>
    public enum MonsterType
    {
        M_Slime,
        M_Zombiem,
        M_Goblin,
        M_Skeleton,

    }

    /// <summary>
    /// 몬스터의 기본(부모) 추상 클래스
    /// </summary>
    public abstract class Monster
    {
        //몬스터의 공통 기능....
        public abstract void Attack();
    }

    /// <summary>
    /// 슬라임 몬스터
    /// </summary>
    public class Slime : Monster
    {
        public override void Attack()
        {
            Debug.Log("Slime Attack");
        }

        //... 슬라임 기능 구현
    }

    /// <summary>
    /// 좀비 몬스터
    /// </summary>
    public class Zombie : Monster
    {
        public override void Attack()
        {
            Debug.Log("Zombie Attack");
        }

        //... 좀비 기능 구현
    }


    /// <summary>
    /// 고블린 몬스터
    /// </summary>
    public class Goblin : Monster
    {
        public override void Attack()
        {
            Debug.Log("Goblin Attack");
        }

        //... 고블린 기능 구현
    }

    /// <summary>
    /// 스켈레톤 몬스터
    /// </summary>
    public class Skeleton : Monster
    {
        public override void Attack()
        {
            Debug.Log("Skeleton Attack");
        }

        //... 스켈레톤 기능 구현
    }

}