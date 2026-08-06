using UnityEngine;
using System.Collections.Generic;

namespace MySample
{
    /// <summary>
    /// 오브젝트 풀을 관리하는 클래스
    /// 풀 만들기, 풀에서 오브젝트 꺼내기, 오브젝트 풀에 넣기
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        #region Variables
        //풀의 크기 (저장할 수 있는 오브젝트의 갯수)
        [SerializeField] private int initPoolSize;
        public int InitPoolSize => initPoolSize;

        //저장하는 오브젝트의 프리팹
        public PooledObject objectToPool;

        //저장하는 풀 (자료구조: stack)
        private Stack<PooledObject> stack;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            //풀 만들기
            SetupPool();
        }
        #endregion

        #region Custom Method
        //풀 만들기
        private void SetupPool()
        {
            //저장할 오브젝트 체크
            if (objectToPool == null)
                return;

            stack = new Stack<PooledObject>();

            //풀에 저장되는 오브젝트 객체 변수
            PooledObject instance = null;

            for (int i = 0; i < initPoolSize; i++)
            {
                //프리팹으로 저장할 오브젝트 생성
                instance = Instantiate(objectToPool);
                instance.Pool = this;                   //My 풀 저장
                instance.gameObject.SetActive(false);   //비활성화
                stack.Push(instance);
            }
        }

        //풀에서 오브젝트 꺼내기
        public PooledObject GetPooledObject()
        {
            //오브젝트 체크
            if (objectToPool == null)
                return null;

            //풀 체크 - 꺼낼 오브젝트가 없을때 새로 생성
            if(stack.Count == 0)
            {                
                PooledObject newInstance = Instantiate(objectToPool);
                newInstance.Pool = this;
                return newInstance;
            }

            //풀에 오브젝트가 있으면 풀에서 꺼내기
            PooledObject nextObject = stack.Pop();
            nextObject.gameObject.SetActive(true);   //활성화
            return nextObject;
        }

        //오브젝트 풀에 넣기
        public void ReturnToPool(PooledObject pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
            stack.Push(pooledObject);
        }
        #endregion
    }
}