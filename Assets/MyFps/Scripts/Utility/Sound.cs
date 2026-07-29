using UnityEngine;
using System;

namespace MyFps
{
    /// <summary>
    /// 사운드 데이터 속성 정의 직렬화된 클래스
    /// </summary>
    [Serializable]
    public class Sound
    {
        public string name;         //관리되는 사운드 이름

        public AudioClip clip;      //재생할 사운드 리소스 - 음원

        public float volume;        //재생 크기
        public float pitch;         //재생 속도

        public bool loop;           //반복 재생 여부
        public bool playOnAwake;    //처음 플레이 여부
                
        [HideInInspector] //public 이면서 외부에서는 볼수없는 속성
        public AudioSource audioSource; //속성을 플레이 시킬 오디오 소스
    }
}