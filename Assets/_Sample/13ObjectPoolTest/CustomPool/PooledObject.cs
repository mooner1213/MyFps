using UnityEngine;

namespace MySample
{
    /// <summary>
    /// 풀에 저장하는 오브젝트 정의
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        //오브젝트가 저장되는 풀
        private ObjectPool pool;
        public ObjectPool Pool { 
            get { return pool; }
            set { pool = value; }
        }

        //쓰임이 다하면 풀에 다시 돌아가기
        public void Release()
        {
            Pool.ReturnToPool(this);
        }
    }
}