using UnityEngine;

namespace MySample
{
    /// <summary>
    /// 몬스터 생성을 관리하는 클래스 : 심플 팩토리
    /// 하나의 공장에서 하나의 생산라인(생성함수)을 통해 모든 몬스터를 생성한다
    /// </summary>
    public class MonsterFactory
    {
        //슬라임 생성 갯수
        public int count = 0;

        //몬스터 생성 메서드
        public Monster CreateMonster(MonsterType monsterType)
        {
            switch (monsterType)
            {
                case MonsterType.M_Slime:
                    return new Slime();

                case MonsterType.M_Zombiem:
                    return new Zombie();

                case MonsterType.M_Goblin:
                    return new Goblin();

                case MonsterType.M_Skeleton:
                    return new Skeleton();
            }

            return null;
        }


        //좀비 어떤거
        public void AddSomething()
        {
            Debug.Log("Add Something");
        }

        //고블린 어떤거

        //

    }
}