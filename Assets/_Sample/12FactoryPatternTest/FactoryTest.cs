using UnityEngine;

namespace MySample
{
    /// <summary>
    /// 몬스터 생성 예제
    /// </summary>
    public class FactoryTest : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            /*//슬라임 생성, 공격
            Slime slime = new Slime();
            slime.Attack();
            //좀비 생성, 공격
            Zombie zombie = new Zombie();
            zombie.Attack();*/

            /*//메서드(CreateMonster())를 이용한 몬스터 생성
            //슬라임 생성, 공격
            Monster slime = CreateMonster(MonsterType.M_Slime);
            slime.Attack();
            //좀비 생성, 공격
            Monster zombie = CreateMonster(MonsterType.M_Zombiem);
            zombie.Attack();*/

            /*//심플 팩토리(MonsterFactory)를 이용한 몬스터 생성
            MonsterFactory monsterFactory = new MonsterFactory();

            //슬라임 생성, 공격
            Monster slime = monsterFactory.CreateMonster(MonsterType.M_Slime);
            monsterFactory.count++;
            slime.Attack();
            //좀비 생성, 공격
            Monster zombie = monsterFactory.CreateMonster(MonsterType.M_Zombiem);
            zombie.Attack();
            monsterFactory.AddSomething();*/

            //팩토리 메서드 패턴
            //슬라임 생성, 공격
            SlimeFactory slimeFactory = new SlimeFactory();
            Monster slime = slimeFactory.CreateMonster();
            slimeFactory.SlimeCount();
            slime.Attack();

            //좀비 생성, 공격
            ZombieFactory zombieFactory = new ZombieFactory();
            Monster zombie = zombieFactory.CreateMonster();
            zombie.Attack();
            zombieFactory.AddSomething();

            //
            GoblinFactory goblinFactory = new GoblinFactory();
            Monster goblin = goblinFactory.CreateMonster();
            goblin.Attack();

        }

        #region Custom Method
        //몬스터 생성 메서드
        private Monster CreateMonster(MonsterType monsterType)
        {
            switch(monsterType)
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
        #endregion
    }
}