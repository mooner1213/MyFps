using UnityEngine;

namespace MySample
{
    /// <summary>
    /// 1초마다 좌우로 바꾸어 이동한다
    /// dir변수 사용 1이면 오른쪽 이동 -1이면 왼쪽이동
    /// </summary>
    public class MoveWall : MonoBehaviour
    {
        [Header("Move")]
        [Tooltip("방향: 1 = 오른쪽, -1 = 왼쪽")]
        public int dir = 1; // 1: 오른쪽, -1: 왼쪽

        [Tooltip("이동 속도 (유닛/초)")]
        public float speed = 2f;

        [Tooltip("로컬(right) 기준으로 이동할지 여부. 체크 해제 시 월드(right) 기준")]
        public bool useLocalSpace = true;

        private void Start()
        {
            // dir 값 안전화: 0일 경우 기본 1로 설정
            if (dir == 0) dir = 1;

            // 1초마다 방향 전환
            InvokeRepeating(nameof(FlipDirection), 2f, 2f);
        }

        private void Update()
        {
            // dir은 1 또는 -1이어야 함
            int signedDir = dir > 0 ? 1 : -1;

            Vector3 moveDir = useLocalSpace ? transform.right : Vector3.right;
            transform.Translate(moveDir * signedDir * speed * Time.deltaTime, Space.World);
        }

        private void FlipDirection()
        {
            dir = -dir;
        }

        private void OnValidate()
        {
            // 인스펙터에서 실수로 0을 넣는걸 방지
            if (dir == 0) dir = 1;
        }
    }
}

/*
1초마다 좌우로 바꾸어 이동한다
dir변수 사용 1이면 오른쪽 이동 -1이면 왼쪽이동
스크립트 작성해줘


*/