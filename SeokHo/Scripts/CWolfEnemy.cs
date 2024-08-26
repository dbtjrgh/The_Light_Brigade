using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class CWolfEnemy : MonoBehaviour, IHittable
{
    #region ����
    // UI ���� ����
    public GameObject hpBarPrefab; // HP �� ������
    public GameObject damageTextPrefab; // ������ �ؽ�Ʈ ������ 
    public Vector3 v3HpBar; // HP �� ��ġ ������
    public Slider enemyHpbar; // ���� HP �� �����̴�
    private UIDamagePool damagePool; // ������ UI Ǯ
    private UIEnemyHpBar hpBarScript;
    private GameObject hpBarCanvas; // ���� HP �� ĵ����
    private Coroutine hideHpBarCoroutine;

    // �� ������Ʈ ���� ����
    public State state; // ���� ����
    public float startingHealth; // ���� ü��
    private float health; // ���� ü��
    public float damage; // ���ݷ�
    public float attackRange; // ���� ��Ÿ�
    public float chaseRange; // ���� �Ÿ�
    public Transform target; // ���� ���
    private NavMeshAgent nmAgent; // ��� Ž���� ���� NavMeshAgent
    private Animator animatorEnemy; // �ִϸ�����
    private float attackDelay = 3f; // ���� ����
    private float lastAttackTime; // ������ ���� ����
    private bool canMove; // �������ɿ���
    private bool canWalk; // �ȱⰡ�ɿ���
    private bool SeePlayer = true; // ���� �÷��̾ ���� ��
    public CWolfEnemyAttack wolfEnemyAttack;
    public GameObject soulPrefab; // ��ȥ ������

    #endregion

    void Awake()
    {
        SetHpBar(); // HP �� ����
        setRigidbodyState(true); // Rigidbody ���� ����
        setColliderState(false); // Collider ���� ����

        damagePool = FindObjectOfType<UIDamagePool>(); // ������ Ǯ ã��
        nmAgent = GetComponent<NavMeshAgent>(); // NavMeshAgent ������Ʈ ��������
        animatorEnemy = GetComponent<Animator>(); // �ִϸ����� ������Ʈ ��������
        wolfEnemyAttack = GetComponentInChildren<CWolfEnemyAttack>();

        state = State.IDLE; // �ʱ� ���¸� IDLE�� ����
        StartCoroutine(StateMachine()); // ���� �ӽ� ����
    }

    #region ���� �ӽ�

    // ���� �ӽ� �ڷ�ƾ
    public IEnumerator StateMachine()
    {
        while (health > 0)
        {
            switch (state)
            {
                case State.IDLE:
                    yield return StartCoroutine(IDLE());
                    break;
                case State.CHASE:
                    yield return StartCoroutine(CHASE());
                    break;
                case State.ATTACK:
                    yield return StartCoroutine(ATTACK());
                    break;
            }
        }
        switch (state)
        {
            case State.DIE:
                yield return StartCoroutine(DIE());
                break;
        }
    }

    // ���� ��ȯ �޼���
    public void ChangeState(State newState)
    {
        if (state == newState)
        {
            return;
        }

        // ���� ���°� IDLE ������ ��, IDLE �ڷ�ƾ�� ����
        if (state == State.IDLE)
        {
            StopCoroutine(IDLE());
            nmAgent.isStopped = true; // �̵��� ���߰� ��
            nmAgent.SetDestination(transform.position); // ���� ��ġ�� ��ǥ�� �����Ͽ� ���߰� ��
        }

        state = newState;
    }

    private IEnumerator IDLE()
    {
        nmAgent.speed = 1;
        canMove = false;

        // �ִϸ����� ���¸� IDLE�� ����
        animatorEnemy.Play("Idle");

        while (state == State.IDLE)
        {
            // NavMeshAgent�� ��ΰ� ���ų� ���� �Ÿ��� ������
            if (!nmAgent.hasPath || nmAgent.remainingDistance < 0.5f)
            {
                // �ִϸ����� ���¸� WALK�� ����
                canWalk = true;

                // ������ ���� ����
                Vector3 randomDirection = Random.insideUnitSphere * 10f;
                randomDirection += transform.position;

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(randomDirection, out navHit, 3f, NavMesh.AllAreas))
                {
                    Vector3 finalPosition = navHit.position;

                    // ��ǥ ��ġ ���� �� �̵� ����
                    nmAgent.SetDestination(finalPosition);
                    nmAgent.isStopped = false;

                    // �̵� �������� ĳ���� ȸ��
                    while (nmAgent.pathPending || nmAgent.remainingDistance > 0.5f)
                    {
                        // ��ǥ ���� ���
                        Vector3 direction = (finalPosition - transform.position).normalized;
                        if (direction != Vector3.zero)
                        {
                            // ȸ�� ����
                            Quaternion targetRotation = Quaternion.LookRotation(direction);
                            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                        }

                        yield return null; // ������Ʈ�� ������ ������ ���
                    }

                    // ���� �� �̵� ���߱� �� IDLE �ִϸ��̼� ���
                    nmAgent.isStopped = true;
                    nmAgent.SetDestination(transform.position); // ���� ��ġ�� ��ǥ �����Ͽ� ����

                    // 50% Ȯ���� �ִϸ��̼� ���¸� ����
                    float randomChance = Random.value; // 0.0f ~ 1.0f ������ ���� ��
                    if (randomChance < 0.5f)
                    {
                        canWalk = false;
                        // IDLE ���¿��� 5�� ���� ���
                        animatorEnemy.Play("Eat");
                        yield return new WaitForSeconds(1f);
                    }
                    else
                    {
                        canWalk = false;
                        // IDLE ���¿��� 5�� ���� ���
                        animatorEnemy.Play("Idle");
                        yield return new WaitForSeconds(1f);
                    }
                }
            }

            yield return null; // �� ������ ���¸� Ȯ��
        }

        // IDLE ���¸� ��� ��, ������Ʈ�� ���߰� ��ǥ�� �ʱ�ȭ
        nmAgent.isStopped = true;
        nmAgent.SetDestination(transform.position);

        // �ִϸ����� ���¸� IDLE�� ����
        animatorEnemy.Play("Idle");
    }


    // CHASE ���� �ڷ�ƾ
    private IEnumerator CHASE()
    {
        if (SeePlayer)
        {   
            // ���� �÷��̾ ���� �� �Ҹ�
            WolfEnemySeePlayerSound();
            SeePlayer = false;
        }

        canWalk = false;
        nmAgent.speed = 3;
        nmAgent.isStopped = false;
        nmAgent.SetDestination(target.position);

        while (state == State.CHASE)
        {
            canMove = true;
            if (target == null)
            {
                ChangeState(State.IDLE);
                yield break;
            }
            transform.LookAt(target);
            // �÷��̾ ��� ����
            nmAgent.SetDestination(target.position);

            // ���� �Ÿ��� ����Ͽ� ���� ���� ���� �ִ��� Ȯ��
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // ���� ���� ���� ������ ATTACK ���·� ��ȯ
            if (distanceToTarget <= attackRange)
            {
                nmAgent.isStopped = true;
                ChangeState(State.ATTACK);
                yield break; // ATTACK ���·� ��ȯ �� CHASE �ڷ�ƾ ����
            }

            yield return null;
        }
    }

    // ATTACK ���� �ڷ�ƾ
    private IEnumerator ATTACK()
    {
        if (SeePlayer)
        {   
            // ���� �÷��̾ ���� �� �Ҹ�
            WolfEnemySeePlayerSound();
            SeePlayer = false;
        }

        canWalk = false;
        canMove = false;
        while (state == State.ATTACK)
        {
            if (target == null)
            {
                ChangeState(State.IDLE);
                yield break;
            }

            // ���� �Ÿ��� ����Ͽ� ���� ���� ���� �ִ��� Ȯ��
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange)
            {
                if (lastAttackTime + attackDelay <= Time.time)
                {
                    animatorEnemy.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                    yield return new WaitForSeconds(0.5f); // ���� �ִϸ��̼� �ð� ���
                }
            }
            else
            {
                ChangeState(State.CHASE);
                yield break; // CHASE ���·� ��ȯ �� ATTACK �ڷ�ƾ ����
            }

            yield return null;
        }
    }

    // DIE ���� �ڷ�ƾ
    private IEnumerator DIE()
    {
        WolfEnemyDieSound();
        // �� ��� ���� ó��
        nmAgent.isStopped = true;
        Destroy(hpBarCanvas, 1f);
        HandleDeath();

        // 1�� �Ŀ� ��ȥ ���� �� �̵� ����
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            GameObject soul = Instantiate(soulPrefab, transform.position, Quaternion.identity);
            StartCoroutine(MoveSoulToTarget(soul, target));
        }
        yield return null;
    }
    #endregion

    void Update()
    {
        // ���� ����� ���� ���ο� ���� �ٸ� �ִϸ��̼� ���
        animatorEnemy.SetBool("CanMove", canMove);
        animatorEnemy.SetBool("CanWalk", canWalk);
    }

    public void Hit(float damage)
    {
        ShowHpBar();

        // ���� �¾��� �� �������� ��
        if (state == State.IDLE)
        {
            // �÷��̾ ã�� ���� Ž��
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform; // �÷��̾��� ������ Ʈ������ ����
                ChangeState(State.CHASE);
            }
        }
        else if (state == State.CHASE)
        {
            if (target != null)
            {
                target = GameObject.FindGameObjectWithTag("Player")?.transform;
            }
        }

        GameObject damageUI = damagePool.GetObject();
        health -= damage;
        CheckHp();
        if (damagePool != null)
        {
            TextMeshProUGUI text = damageUI.GetComponent<TextMeshProUGUI>();
            text.text = (" - ") + damage.ToString();

            UIDamageText damageText = damageUI.GetComponent<UIDamageText>();
            damageText.Initialize(transform, Vector3.up * 2, damagePool);
        }
        if (health <= 0)
        {
            ChangeState(State.DIE);
        }
    }

    // ���� �ִϸ��̼��� �۵��ϸ� ȣ��
    public void startjumpAttack()
    {
        wolfEnemyAttack.StartAttack();
    }

    public void endJumpAttack()
    {
        wolfEnemyAttack.EndAttack();
    }

    // ��ȥ �̵� �ڷ�ƾ
    private IEnumerator MoveSoulToTarget(GameObject soul, Transform target)
    {
        float fRandX = Random.Range(-2.0f, 2.0f) + transform.position.x;
        float fRandY = Random.Range(1.5f, 3.0f);
        float fRandZ = Random.Range(-2.0f, 2.0f) + transform.position.z;

        Vector3 v3StartPosition = soul.transform.position;
        Vector3 v3MiddlePosition = new Vector3(fRandX, fRandY, fRandZ);

        float fTime = 0.0f;
        float fDuration = 1f;

        while (fTime <= fDuration)
        {
            Vector3 lerpPoint1 = Vector3.Lerp(v3StartPosition, v3MiddlePosition, fTime / fDuration);
            Vector3 lerpPoint2 = Vector3.Lerp(v3MiddlePosition, target.position, fTime / fDuration);
            soul.transform.position = Vector3.Lerp(lerpPoint1, lerpPoint2, fTime / fDuration);

            fTime += Time.deltaTime;
            yield return null;
        }

        soul.transform.position = target.position;
        Destroy(soul);
    }

    #region ���� ����

    private void WolfEnemySeePlayerSound()
    {
        CEnemySoundManager.Instance.PlayEnemySound(4, transform.position);
    }

    private void WolfEnemyJumpAttackSound()
    {
        CEnemySoundManager.Instance.PlayEnemySound(5, transform.position);
    }

    private void WolfEnemyDieSound()
    {
        CEnemySoundManager.Instance.PlayEnemySound(6, transform.position);
    }

    #endregion

    #region Ragdoll, Collider ����
    // Rigidbody ���� ���� �޼���
    void setRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = gameObject.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
    }

    // Collider ���� ���� �޼���
    void setColliderState(bool state)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
        gameObject.GetComponent<Collider>().enabled = true;
    }

    // �� ��� ó�� �޼���
    private void HandleDeath()
    {
        gameObject.GetComponent<Animator>().enabled = false;
        setRigidbodyState(false);
        setColliderState(true);
        Destroy(gameObject, 5f); // 5�� �Ŀ� ���� ������Ʈ �ı�
    }
    #endregion

    #region HPBar ����
    // HP Ȯ�� �� ������Ʈ �޼���
    private void CheckHp()
    {
        if (enemyHpbar != null)
        {
            enemyHpbar.value = health; // HP �� ������Ʈ
        }
    }

    // HP �� ���� �޼���
    private void SetHpBar()
    {
        health = startingHealth;
        // HP �ٰ� ���� �����̽� ĵ������ �����ǵ��� ����
        hpBarCanvas = new GameObject("EnemyHpBarCanvas");
        hpBarCanvas.transform.SetParent(transform);
        Canvas canvas = hpBarCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        CanvasScaler canvasScaler = hpBarCanvas.AddComponent<CanvasScaler>();
        canvasScaler.dynamicPixelsPerUnit = 10;

        GameObject hpBar = Instantiate(hpBarPrefab, canvas.transform);

        hpBarScript = hpBar.GetComponent<UIEnemyHpBar>();
        hpBarScript.trEnemy = transform;
        hpBarScript.v3offset = v3HpBar;
        enemyHpbar = hpBar.GetComponentInChildren<Slider>();
        enemyHpbar.maxValue = startingHealth;
        enemyHpbar.value = health;

        hpBarCanvas.SetActive(false);
    }

    // HP �� ǥ�� �� ����� �޼���
    private void ShowHpBar()
    {
        if (hideHpBarCoroutine != null)
        {
            StopCoroutine(hideHpBarCoroutine);
            hideHpBarCoroutine = null; // �ڷ�ƾ ������ �ʱ�ȭ�մϴ�.
        }

        // HP �ٰ� �ı��Ǿ����� Ȯ���մϴ�.
        if (hpBarCanvas != null)
        {
            hpBarCanvas.SetActive(true);
            hideHpBarCoroutine = StartCoroutine(HideHpBarAfterDelay(3f));
        }
    }

    // ���� �ð� �� HP �� ����� �ڷ�ƾ
    private IEnumerator HideHpBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // HP �ٰ� �ı��Ǿ����� Ȯ���մϴ�.
        if (hpBarCanvas != null)
        {
            hpBarCanvas.SetActive(false);
        }
    }
    #endregion

   
}
