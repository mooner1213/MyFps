using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyFps
{
    /// <summary>
    /// 게임데이터 파일에 저장하기/가져오기 - 이진화
    /// </summary>
    public static class SaveLoad
    {
        //데이터 저장하기
        public static void SaveData()
        {
            //파일이름, 경로 지정
            string path = Application.persistentDataPath + "/pData.dat";

            //저장할 데이터를 이진화 준비
            BinaryFormatter formatter = new BinaryFormatter();

            //파일 접근 - 없으면 새로 만들고 있으면 덮어쓰고
            FileStream fs = new FileStream(path, FileMode.Create);

            //저장할 데이터 준비
            PlayData playData = new PlayData();
            //Debug.Log($"Save sceneNumber: {playData.sceneNumber}");

            //준비된 데이터 이진화 저장
            formatter.Serialize(fs, playData);

            //파일 클로즈
            fs.Close();
        }

        //데이터 가져오기
        public static PlayData LoadData()
        {
            PlayData playData = null;

            //파일이름, 경로 지정
            string path = Application.persistentDataPath + "/pData.dat";

            //지정된 경로에 파일이 있는지 없는지 체크
            if(File.Exists(path))
            {
                //가져올 데이터를 이진화 준비
                BinaryFormatter formatter = new BinaryFormatter();

                //파일 접근 - 없으면 새로 만들고 있으면 덮어쓰고
                FileStream fs = new FileStream(path, FileMode.Open);

                //파일에 이진화로 저장된 데이터를 역이진화해서 가져온다
                playData = formatter.Deserialize(fs) as PlayData;
                //Debug.Log($"Load sceneNumber: {playData.sceneNumber}");

                //파일 클로즈
                fs.Close();
            }
            else
            {
                Debug.Log("저장된 파일 없음");
            }

            return playData;
        }
    }
}