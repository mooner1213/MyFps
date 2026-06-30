using UnityEngine;
using System.Collections;

namespace MyFps
{
    /// <summary>
    /// 嚥≪뮆???怨대럵???怨밴묶 ??욧탢??
    /// </summary>
    public enum EnemyState
    {
        Idle = 0,
        Walk = 1,
        Attack = 2,
        Death = 3
    }

    /// <summary>
    /// 嚥≪뮆???怨몄뱽 ?온?귐뗫릭???????
    /// </summary>
    public class Robot : MonoBehaviour
    {
        #region Variables
        [Header("State Settings")]
        [SerializeField] private EnemyState currentState = EnemyState.Idle;

        [Header("AI Settings")]
        [SerializeField] private string targetTag = "Player";        // ?곕뗄?????????볥젃
        [SerializeField] private float detectionRange = 5f;          // 揶쏅Ŋ? ??援끿뵳?
        [SerializeField] private float attackRange = 1.5f;           // ?⑤벀爰???뽰삂 ??援끿뵳?(疫꿸퀡??1.5)
        [SerializeField] private float escapeAttackRange = 2.2f;     // ?⑤벀爰?餓λ쵎??獄??곕뗄????而???援끿뵳?(??됰뮞???봺??뽯뮞)
        [SerializeField] private float moveSpeed = 5f;               // ??猷???얜즲 (?遺쎈럡??鍮? 5f)
        [SerializeField] private float rotationSpeed = 5f;           // ???읈 ??얜즲

        [Header("Combat Settings")]
        [SerializeField] private float maxHealth = 20f;              // ??筌ｋ???(?遺쎈럡??鍮? 20)
        [SerializeField] private float currentHealth;
        [SerializeField] private float damage = 5f;                 // ?⑤벀爰??(?遺쎈럡??鍮? 5)
        [SerializeField] private float attackDuration = 2.0f;        // ?⑤벀爰?雅뚯눊由?(?遺쎈럡??鍮? 2.0??

        [Header("Audio Settings")]
        [SerializeField] private AudioClip jumpscareTune;           // ?源놁삢 ?????(JumpscareTune)
        [SerializeField] private AudioSource audioSource;

        private Animator animator;
        private Transform playerTarget;
        private CharacterController characterController;             // 甕????궢 獄쎻뫗???筌?Ŧ????뚢뫂?껅에?살쑎
        private float attackTimer = 0f;                              // ??? ?⑤벀爰???볦퍢 ??????
        private float damageCooldown = 0f;                           // ?怨?筌왖 揶쎛???묅뫀?????????
        private float startDelayTimer = 1.0f;                        // ?源놁삢 ??1????疫???????
        private bool isDead = false;
        private bool applyDeathOffset = false;                       // ??彛?????쑴竊????쎈늄??癰귣똻???怨몄뒠 ???삋域?
        private float currentDeathYOffset = 0f;                      // ??쇰뻻揶???彛?Y????띿뺏 ??쎈늄??
        private bool hasDealtDamageThisAttack = false;               // ?꾩옱 怨듦꺽 二쇨린?먯꽌 ?곕?吏瑜??낇삍?붿? ?щ?
        #endregion

        #region Unity Event Methods
        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            // ?룐뫂??筌뤴뫁??Root Motion)??곗쨮 ?紐낅퉸 ?⑤벀爰??醫딅빍筌롫뗄???獄쏆꼶?????紐껋삏??쎈쨲 ?袁⑺뒄揶쎛 獄쎛?귐됰뮉 ?袁⑷맒??獄쎻뫗???몃빍??
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }

            // 甕????궢 獄쎻뫗? 獄????쟿??곷선 ?겸뫖猷??닌뗭겱???袁る퉸 CharacterController揶쎛 ??곸몵筌??癒?짗 ?곕떽???몃빍??
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = gameObject.AddComponent<CharacterController>();
                characterController.center = new Vector3(0f, 1f, 0f); // 2m ?醫롮삢 疫꿸퀣? 餓λ쵐釉???쇱젟
                characterController.radius = 0.35f;                    // 獄쏆꼷???
                characterController.height = 2f;                       // ?誘れ뵠
            }

            // ??삳탵?????뮞 ?癒?짗 ?醫딅뼣
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
        }

        private void OnEnable()
        {
            isDead = false;
            currentHealth = maxHealth;
            startDelayTimer = 1.0f; // ?源놁삢????1?λ뜃而???疫???쇱젟
            damageCooldown = 0f;    // 筌??⑤벀爰????怨?筌왖 筌앸맩??揶쎛??롫즲嚥??λ뜃由??

            // ???깆옣 JumpscareTune ?ъ슫?? Resources ?대뜑?먯꽌 吏곸젒 濡쒕뱶
            if (jumpscareTune == null)
            {
                jumpscareTune = Resources.Load<AudioClip>("JumpscareTune");
                if (jumpscareTune == null)
                {
                    Debug.LogWarning("[Robot] JumpscareTune audio clip not found in Resources!");
                }
            }

            if (audioSource != null && jumpscareTune != null)
            {
                audioSource.PlayOneShot(jumpscareTune);
                Debug.Log("[Robot] Spawn JumpscareTune played successfully.");
            }
        }

        private void Start()
        {
            UpdateAnimatorState();

            // ?⑤벀爰?餓λ쵎????援끿뵳?? ?⑤벀爰???뽰삂 ??援끿뵳?????臾롪탢??揶쏆늿?앾쭖??癒?짗??곗쨮 癰귣똻?쇿첎???쇱젟
            if (escapeAttackRange <= attackRange)
            {
                escapeAttackRange = attackRange + 0.7f;
            }
        }

        private void Update()
        {
            // ???쟿??곷선 ??野??癒?짗 筌≪뼐由?
            if (playerTarget == null)
            {
                GameObject playerObj = GameObject.FindWithTag(targetTag);
                if (playerObj != null)
                {
                    playerTarget = playerObj.transform;
                }
            }

            // ???쟿??餓λ쵐???袁⑤빜 ???뮉 ?袁⑥쟿????낅쑓??꾨뱜 椰꾨?瑗?? (?癒?탵?????뮞??????
            if (!Application.isPlaying)
            {
                UpdateAnimatorState();
                return;
            }

            // AI ?怨밴묶 ??낅쑓??꾨뱜
            UpdateAIState();
        }

        // ?紐꾨뮞??됯숲?癒?퐣 揶쏅???癰귘궗野껋?釉????癒?탵??筌뤴뫀諭?癒?퐣??筌앸맩???醫딅빍筌롫뗄???뤿퓠 獄쏆꼷???롫즲嚥?筌ｌ꼶??
        private void OnValidate()
        {
            UpdateAnimatorState();
        }
        #endregion

        #region Custom Methods
        /// <summary>
        /// ?袁⑹삺 AI ?怨밴묶 ??낅쑓??꾨뱜 獄??곕뗄???⑤벀爰?嚥≪뮇彛???쎈뻬
        /// </summary>
        private void UpdateAIState()
        {
            // ??彛??怨밴묶 ????낅쑓??꾨뱜 ??쎄땁
            if (isDead) return;

            // 1. ?源놁삢 ??1?λ뜃而???뽰쁽????ф묾??怨쀭뀱
            if (startDelayTimer > 0f)
            {
                startDelayTimer -= Time.deltaTime;
                SetState(EnemyState.Idle);
                return;
            }

            if (playerTarget == null)
            {
                SetState(EnemyState.Idle);
                return;
            }

            float distance = Vector3.Distance(transform.position, playerTarget.position);

            // [?⑤벀爰?餓λ쵐???? ?⑤벀爰??醫딅빍筌롫뗄???륁뵠 ??멸텊 ???돱筌왿궗 ??삘뀲 ?怨밴묶嚥≪뮇???袁⑹뵠???醫됲닊

            if (currentState == EnemyState.Attack)
            {
                attackTimer -= Time.deltaTime;
                RotateTowardsPlayer();

                float animLen = GetAnimatorClipLength("Attack");
                float maxLen = (animLen > 0f) ? animLen : attackDuration;
                float damageHitTiming = maxLen - 0.4f;

                if (attackTimer <= damageHitTiming && !hasDealtDamageThisAttack)
                {
                    if (distance <= attackRange)
                    {
                        PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>() ?? playerTarget.GetComponentInChildren<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damage);
                            Debug.Log("[Robot] Sync hit dealt damage.");
                        }
                    }
                    hasDealtDamageThisAttack = true;
                }

                if (attackTimer <= 0f)
                {
                    if (distance <= attackRange) ReplayAttackAnimation();
                    else SetState(EnemyState.Walk);
                }
                return;
            }

            switch (currentState)
            {
                case EnemyState.Idle:
                    if (distance <= detectionRange)
                    {
                        SetState(EnemyState.Walk);
                    }
                    break;

                case EnemyState.Walk:
                    if (distance <= attackRange)
                    {
                        SetState(EnemyState.Attack);
                    }
                    else
                    {
                        // ???쟿??곷선 獄쎻뫚堉??곗쨮 ???읈??렽??怨대럡 ?곕뗄????猷?
                        MoveTowardsPlayer();
                    }
                    break;

                case EnemyState.Death:
                    // ??彛??怨밴묶?癒?퐣???袁ⓓ??臾믩씜????묐뻬??? ??놁벉
                    break;
            }
        }

        private void MoveTowardsPlayer()
        {
            RotateTowardsPlayer();

            if (characterController != null)
            {
                // ???쟿??곷선???館釉?獄쎻뫚堉?甕겸돧苑??④쑴沅?(Y???誘れ뵠???얜똻???랁???묐즸 ??猷욑쭕??⑥쥓??
                Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);
                Vector3 direction = (targetPosition - transform.position).normalized;

                // SimpleMove??????怨몄몵嚥?餓λ쵎?????釉??랁???됱몵沃샕嚥???묐즸 甕겸돧苑ｏ쭕??④퉲鍮????猷??쀪땁??덈뼄.
                characterController.SimpleMove(direction * moveSpeed);
            }
            else
            {
                // Fallback (?뚢뫂?껅에?살쑎揶쎛 ?臾먮짗??? ??놁뱽 野껋럩??疫꿸퀣???紐껋삏??쎈쨲 ??猷??臾먮짗)
                Vector3 targetPosition = new Vector3(playerTarget.position.x, transform.position.y, playerTarget.position.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Y?????읈???⑥쥙???筌????쟿??곷선??獄쏅뗀?よ퉪?猷꾣에??봔??뺤쓦野????읈??쀪땁??덈뼄.
        /// </summary>
        private void RotateTowardsPlayer()
        {
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0f; // ?怨대럵??疫꿸퀣??????野껉퍔??獄쎻뫗?

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// ?⑤벀爰??醫딅빍筌롫뗄???륁뵠 ??멸텆 ????쇰뻻 ?⑤벀爰????뺣즲????揶쏅벡?ｆ에??醫딅빍筌롫뗄???륁뱽 筌ｌ꼷?ч겫???????臾볥릭???????㎫몴?揶쏄퉮???몃빍??
        /// ?⑤벀爰??醫딅빍筌롫뗄???륁뵠 ??멸텆 ????쇰뻻 ?⑤벀爰????뺣즲????揶쏅벡?ｆ에??醫딅빍筌롫뗄???륁뱽 筌ｌ꼷?ч겫???????臾볥릭??????€?㎫몴?揶쏄퉮???몃빍??
        /// </summary>
        private void ReplayAttackAnimation()
        {
            float duration = GetAnimatorClipLength("Attack");
            attackTimer = (duration > 0f) ? duration : attackDuration;
            hasDealtDamageThisAttack = false; // 타격 플래그 리셋

            if (animator != null)
            {
                animator.Play("Attack", 0, 0f);
            }
        }

        /// <summary>
        /// ?醫딅빍筌롫뗄????뚢뫂?껅에?살쑎????쇰선 ??덈뮉 ??????已?餓??諭????쇱뜖??? 筌띲끉臾??롫뮉 ?醫딅빍筌롫뗄???疫뀀챷?좂몴?獄쏆꼹???몃빍??
        /// </summary>
        private float GetAnimatorClipLength(string clipName)
        {
            if (animator == null || animator.runtimeAnimatorController == null) return 0f;

            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.Equals(clipName, System.StringComparison.OrdinalIgnoreCase) || 
                    clip.name.Contains(clipName))
                {
                    return clip.length;
                }
            }
            return 0f;
        }

        /// <summary>
        /// ?袁⑹삺 ?怨밴묶(currentState)???醫딅빍筌롫뗄???筌띲끆而삭퉪???"EnemyState")????쇱젟??몃빍??
        /// </summary>
        private void UpdateAnimatorState()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                if (animator == null)
                {
                    animator = GetComponentInChildren<Animator>();
                }
            }

            if (animator != null)
            {
                if (animator.runtimeAnimatorController == null)
                {
                    Debug.LogWarning($"[Robot] {gameObject.name}??Animator ?뚮똾猷??곕뱜??Controller揶쎛 ?醫딅뼣??뤿선 ??? ??녿뮸??덈뼄! ?癒?탵?怨쀫퓠??Robot.controller????뺤삋域밸챸釉???醫딅뼣??雅뚯눘苑??");
                }
                else
                {
                    animator.SetInteger("EnemyState", (int)currentState);
                }
            }
        }

        /// <summary>
        /// ?紐??癒?퐣 ?怨대럵???怨밴묶??癰궰野껋?釉????????롫뮉 筌롫뗄苑??
        /// </summary>
        public void SetState(EnemyState newState)
        {
            if (isDead && newState != EnemyState.Death) return;

            currentState = newState;
            UpdateAnimatorState();

            // ?⑤벀爰??怨밴묶嚥????뿯?????λ뜃由?????€????쇱젟 獄?筌??醫딅빍筌롫뗄?????源?
            if (currentState == EnemyState.Attack)
            {
                float duration = GetAnimatorClipLength("Attack");
                attackTimer = (duration > 0f) ? duration : attackDuration;
                damageCooldown = 0f; // ?⑤벀爰?筌욊쑴??筌앸맩??筌??怨?筌왖€??雅뚯눖猷꾣에???쇱젟
                hasDealtDamageThisAttack = false; // 타격 플래그 초기화

                if (animator != null)
                {
                    animator.Play("Attack", 0, 0f);
                }
            }
        }

        /// <summary>
        /// 亦낅슣????爰??源녿퓠 筌띿쉸釉?????紐꾪뀱??롫뮉 ??④봄 筌ｌ꼶????λ땾
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (isDead) return;

            currentHealth -= amount;
            Debug.Log($"[Robot] Took {amount} damage. Current Health: {currentHealth}");

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            SetState(EnemyState.Death);
            Debug.Log("[Robot] Robot is dead.");

            // ??CapsuleCollider???곕떽???랁???由곁몴???뱀삂??띿쓺 餓κ쑴??
            //    ????덈뮉 ?怨밴묶???誘れ뵠(2m)??域밸챶?嚥??癒?늺 ?⑤벊夷??筌롫뗄?⒴첎? ??筌??類?쓺 ???嚥?
            //    ?꾩뮆????묊몴??袁⑼폒 ??뱀삂??띿쓺 ?곕벡???뤿연 Rigidbody揶쎛 ?癒?염??살쓦野?獄쏅뗀???揶쎛??깅툩?袁⑥쨯 ??
            CapsuleCollider cap = gameObject.GetComponent<CapsuleCollider>();
            if (cap == null)
            {
                cap = gameObject.AddComponent<CapsuleCollider>();
            }
            cap.center = new Vector3(0f, 0.15f, 0f); // 獄쏅뗀???揶쎛繹먯빓苡???쇱젟
            cap.radius = 0.3f;
            cap.height = 0.3f; // ?袁⑼폒 ??뱀삂??띿쓺

            // ??Animator??Root Motion????쑵??源딆넅??뤿연 ?얠눖??Rigidbody)揶쎛 ??삵닏??븍뱜 ?袁⑺뒄????뽯선??????덈즲嚥???
            if (animator != null)
            {
                animator.applyRootMotion = false;
            }

            // ??CharacterController ??쑵??源딆넅 (Rigidbody?? ?⑤벊???븍뜃?)
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            // ??Rigidbody ?곕떽? ??餓λ쵎???곗쨮 獄쏅뗀????袁⑹읈???????깆벉
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation; // ??뚯뵠筌ｌ꼶???대????삳빍筌왖 ??낅즲嚥????읈 ??쀫립

            // ????彛?????쑴竊???됰뜄?/筌롫뗄?? Y ?ル슦紐?癰귣똻?????삋域???뽮쉐??(LateUpdate?癒?퐣 ?袁⑥쟿????살쒔??깆뵠??
            applyDeathOffset = true;
            currentDeathYOffset = 0f;

            // 5???????늾 筌ｌ꼶??
            Destroy(gameObject, 5f);
        }

        // ?좊땲硫붿씠????뼱?곌린 ?고쉶??LateUpdate Y異?蹂댁젙
        private void LateUpdate()
        {
            if (applyDeathOffset)
            {
                if (currentDeathYOffset > -0.85f)
                {
                    currentDeathYOffset -= Time.deltaTime * 0.9f;
                    if (currentDeathYOffset < -0.85f) currentDeathYOffset = -0.85f;
                }
                int childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child != null)
                    {
                        Vector3 lPos = child.localPosition;
                        child.localPosition = new Vector3(lPos.x, lPos.y + currentDeathYOffset, lPos.z);
                    }
                }
            }
        }
        #endregion
    }
}
