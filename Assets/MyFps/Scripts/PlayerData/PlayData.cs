using System;

namespace MyFps
{
    /// <summary>
    /// 파일에 저장할 게임 플레이 데이터 목록/정의 직렬화된 클래스
    /// </summary>
    [Serializable]
    public class PlayData
    {
        public int sceneNumber;     //플레이씬 번호
        public float health;        //플레이어 체력
        public int ammoCount;       //탄환 갯수

        //etc...

        //생성자
        public PlayData()
        {
            sceneNumber = PlayerStats.Instance.SceneNumber;
            health = PlayerStats.Instance.Health;
            ammoCount = PlayerStats.Instance.AmmoCount;
        }
    }
}