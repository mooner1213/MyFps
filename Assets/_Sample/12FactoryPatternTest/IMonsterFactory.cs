using UnityEngine;

namespace MySample
{
    /// <summary>
    /// 몬스터 팩토리 기능 정의한 인터페이스
    /// 필수 구현사항: (몬스터) 생성 메서드
    /// </summary>
    public interface IMonsterFactory
    {
        public Monster CreateMonster(); //(몬스터) 생성 메서드
    }

    //몬스터를 생산하는 공장 만들기
    /// <summary>
    /// 슬라임만 생성하는 슬라임 전용 공장
    /// </summary>
    public class SlimeFactory : IMonsterFactory
    {
        //슬라임 생성 갯수
        private int count = 0;

        //(몬스터) 생성 메서드
        public Monster CreateMonster()
        {
            return new Slime();
        }

        public void SlimeCount()
        {
            count++;
        }
    }

    /// <summary>
    /// 좀비만 생성하는 좀비 전용 공장
    /// </summary>
    public class ZombieFactory : IMonsterFactory
    {
        //(몬스터) 생성 메서드
        public Monster CreateMonster()
        {
            return new Zombie();
        }

        //좀비 어떤거
        public void AddSomething()
        {
            Debug.Log("Add Something");
        }
    }

    /// <summary>
    /// 고블린만 생성하는 고블린 전용 공장
    /// </summary>
    public class GoblinFactory : IMonsterFactory
    {
        //..

        //(몬스터) 생성 메서드
        public Monster CreateMonster()
        {
            return new Goblin();
        }

        //...
    }
}