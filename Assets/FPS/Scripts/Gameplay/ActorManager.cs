using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.FPS.Game
{
    /// <summary>
    /// 씬 내 존재하는 모든 Actor(플레이어, 적, NPC 등)의 목록을 중앙 관리하는 매니저 클래스
    /// </summary>
    public class ActorManager : MonoBehaviour
    {
        #region Variables
        private static ActorManager instance;

        [Header("Actor List")]
        [Tooltip("현재 씬에 활성화된 모든 액터들의 리스트")]
        [SerializeField] private List<Actor> actorList = new List<Actor>();

        // 싱글톤 프로퍼티
        public static ActorManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<ActorManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("ActorManager");
                        instance = go.AddComponent<ActorManager>();
                    }
                }
                return instance;
            }
        }

        // 전체 액터 읽기 전용 목록
        public IReadOnlyList<Actor> ActorList => actorList;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Custom Method
        /// <summary>
        /// 새로운 액터를 리스트에 등록합니다.
        /// </summary>
        public void RegisterActor(Actor actor)
        {
            if (actor != null && !actorList.Contains(actor))
            {
                actorList.Add(actor);
            }
        }

        /// <summary>
        /// 액터를 리스트에서 등록 해제합니다.
        /// </summary>
        public void UnregisterActor(Actor actor)
        {
            if (actor != null && actorList.Contains(actor))
            {
                actorList.Remove(actor);
            }
        }

        /// <summary>
        /// 특정 진영/팀(affiliation)에 속한 모든 살아있는 액터를 반환합니다.
        /// </summary>
        public List<Actor> GetActorsByAffiliation(int affiliation)
        {
            List<Actor> result = new List<Actor>();
            foreach (Actor a in actorList)
            {
                if (a != null && !a.IsDead && a.Affiliation == affiliation)
                {
                    result.Add(a);
                }
            }
            return result;
        }

        /// <summary>
        /// 특정 기준 위치에서 가장 가까운 타겟 액터를 찾습니다.
        /// </summary>
        /// <param name="fromPosition">기준 위치</param>
        /// <param name="targetAffiliation">찾을 대상의 진영 (-1이면 진영 상관없이 탐색)</param>
        public Actor GetClosestActor(Vector3 fromPosition, int targetAffiliation = -1)
        {
            Actor closestActor = null;
            float minDistanceSq = float.MaxValue;

            foreach (Actor a in actorList)
            {
                if (a == null || a.IsDead) continue;
                if (targetAffiliation >= 0 && a.Affiliation != targetAffiliation) continue;

                float distSq = (a.transform.position - fromPosition).sqrMagnitude;
                if (distSq < minDistanceSq)
                {
                    minDistanceSq = distSq;
                    closestActor = a;
                }
            }

            return closestActor;
        }
        #endregion
    }
}
