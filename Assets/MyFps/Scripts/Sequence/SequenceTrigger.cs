using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 트리거에 의한 연출 구현하는 클래스들의 부모 클래스
    /// </summary>
    public class SequenceTrigger : MonoBehaviour
    {
        #region Unity Event Method
        private void OnTriggerEnter(Collider other)
        {
            //플레이어 체크
            if (other.gameObject.tag != "Player")
                return;

            //Debug.Log($"OnTriggerEnter: {other.gameObject.name}");
            StartCoroutine(SequencePlay(other.gameObject));

            //트리거 기능 비활성화
            transform.GetComponent<BoxCollider>().enabled = false;
        }
        #endregion

        #region Custom Method
        protected virtual IEnumerator SequencePlay(GameObject player)
        {
            yield return null;
        }
        #endregion
    }
}