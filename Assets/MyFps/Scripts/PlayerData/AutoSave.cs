using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFps
{
    /// <summary>
    /// 플레이씬을 시작할때 자동으로 현재 씬 번호 저장하는 클래스
    /// </summary>
    public class AutoSave : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //씬 번호 저장
            SaveSceneNumber();
        }

        void SaveSceneNumber()
        {            
            int sceneNumber = SceneManager.GetActiveScene().buildIndex;
            Debug.Log($"Save sceneNumber: {sceneNumber}");

            //PlayerPrefs
            PlayerPrefs.SetInt("SceneNumber", sceneNumber);
        }
    }
}