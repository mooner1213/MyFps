using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 마지막 문을 나갈때 걸리는 시퀀스 트리거 구현
    /// 시퀀스 내용 : 두번째 씬 나가지 구현
    /// </summary>
    public class FExitTrigger : SequenceTrigger
    {
        #region Variables
        //참조
        public SceneFader fader;
        //[SerializeField] private string loadToScene = "PlayScene03";
        [SerializeField] private int loadToSceneNumber = 4;
        [SerializeField] private string loadToScene = "MainMenu";        
        #endregion

        #region Custom Method
        protected override IEnumerator SequencePlay(GameObject player)
        {            
            //- 배경음 Stop
            //- 다음씬(PlayScene03)으로 이동
            
            AudioManager.Instance.StopBgm();

            //씬 클리어 처리...
            //TODO : 다음씬이 없어 게임 데이터 저장 생략
            //게임 데이터 저장 - 다음 씬번호, AmmoCount, Health 저장
            //PlayerStats.Instance.SceneNumber = loadToSceneNumber;
            //SaveLoad.SaveData();

            yield return new WaitForSeconds(0.1f);
            fader.FadeTo(loadToScene);
        }
        #endregion
    }
}