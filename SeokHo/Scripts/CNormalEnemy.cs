using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;



public class CNormalEnemy : MonoBehaviour, IHittable
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

    // �Ѿ� ���� ����
    public GameObject bulletPrefab; // �߻�ü ������
    public Transform bulletSpawnPoint; // �߻�ü ���� ��ġ
    public BulletPool bulletPool; // �Ѿ� Ǯ ����

    // �� ������Ʈ ���� ����
    public State state; // ���� ����
    public float startingHealth; // ���� ü��
    private float health; // ���� ü��
    private float damage; // ���ݷ�
    public float attackRange; // ���� ��Ÿ�
    public float chaseRange; // ���� �Ÿ�
    public Transform target; // ���� ���
    private NavMeshAgent nmAgent; // ��� Ž���� ���� NavMeshAgent
    private Animator animatorEnemy; // �ִϸ�����
    private float attackDelay = 5f; // ���� ����
    private float bulletSpeed = 10f; // �߻�ü �ӵ�
    private float lastAttackTime; // ������ ���� ����
    private bool canMove; // �������ɿ���
    private bool SeePlayer = true; // ���� �÷��̾ ���� ��
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

        state = State.IDLE; // �ʱ� ���¸� IDLE�� ����
        StartCoroutine(StateMachine()); // ���� �ӽ� ����
    }
    void Update()
    {
        // ���� ����� ���� ���ο� ���� �ٸ� �ִϸ��̼� ���
        animatorEnemy.SetBool("CanMove", canMove);
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

    // IDLE ���� �ڷ�ƾ
    private IEnumerator IDLE()
    {
        canMove = false;

        // Walk �ִϸ��̼� ���¸� �ݺ������� �����ϴ� �ڷ�ƾ ����
        StartCoroutine(ToggleWalkAnimation());

        while (state == State.IDLE)
        {
            // NavMeshAgent�� ��ΰ� ���ų� ���� �Ÿ��� ������
            if (!nmAgent.hasPath || nmAgent.remainingDistance < 0.5f)
            {
                // ���� ���� ����
                Vector3 randomDirection = Random.insideUnitSphere * 10f;
                randomDirection += transform.position;

                NavMeshHit navHit;
                if (NavMesh.SamplePosition(randomDirection, out navHit, 3f, NavMesh.AllAreas))
                {
                    Vector3 finalPosition = navHit.position;
                    // ��ǥ ��ġ ���� �� �̵� ����
                    nmAgent.SetDestination(finalPosition);
                    nmAgent.isStopped = false;

                    // ������Ʈ�� ��ǥ ��ġ�� ������ ������ ���
                    while (nmAgent.pathPending || nmAgent.remainingDistance > 0.5f)
                    {
                        yield return null; // ������Ʈ�� ������ ������ ���
                    }

                    // ���� �� �̵� ���� �� IDLE �ִϸ��̼� ���
                    nmAgent.isStopped = true;
                    nmAgent.SetDestination(transform.position); // ���� ��ġ�� ��ǥ ���� �� ����

                    // 50% Ȯ���� �ִϸ��̼� ���� ����
                    float randomChance = Random.value; // 0.0f���� 1.0f ������ ���� ��
                    if (randomChance < 0.5f)
                    {
                        // Walk �ִϸ��̼��� true�� �����ϰ� 5�� ���� �ȱ�
                        animatorEnemy.SetBool("Walk", true);
                        nmAgent.isStopped = false;
                        Vector3 walkDirection = Random.insideUnitSphere * 5f;
                        walkDirection += transform.position;

                        if (NavMesh.SamplePosition(walkDirection, out navHit, 3f, NavMesh.AllAreas))
                        {
                            nmAgent.SetDestination(navHit.position);
                        }

                        yield return new WaitForSeconds(5f);
                        animatorEnemy.SetBool("Walk", false);
                        nmAgent.isStopped = true;
                    }
                }
            }

            yield return null; // �� ������ ���¸� Ȯ��
        }

        // IDLE ���¸� ���� �� ������Ʈ�� ���߰� ��ǥ�� �ʱ�ȭ
        nmAgent.isStopped = true;
        nmAgent.SetDestination(transform.position);
    }

    private IEnumerator ToggleWalkAnimation()
    {
        while (true)
        {
            // Walk �ִϸ��̼� ���¸� ����
            bool currentState = animatorEnemy.GetBool("Walk");
            animatorEnemy.SetBool("Walk", !currentState);
            if (Random.value < 0.5f)
            {
                // KickFoot �ִϸ��̼� Ʈ����
                animatorEnemy.SetTrigger("KickFoot");
                yield return new WaitForSeconds(1f);
            }
            // 5�� ���
            yield return new WaitForSeconds(5f);
        }
    }


    // CHASE ���� �ڷ�ƾ
    private IEnumerator CHASE()
    {
        if(SeePlayer)
        {   // ���� �÷��̾ ���� �� �Ҹ�
            NormalEnemySeePlayerSound();
            SeePlayer = false;
        }
        canMove = true;
        nmAgent.isStopped = false;
        nmAgent.SetDestination(target.position);

        while (state == State.CHASE)
        {
            if (target == null)
            {
                ChangeState(State.IDLE);
                yield break;
            }
            transform.LookAt(target.GetChild(0));
            // �÷��̾ ��� ����
            nmAgent.SetDestination(target.position);

            // ���� �Ÿ��� ����Ͽ� ���� ���� ���� �ִ��� Ȯ��
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // ���� ���� ���� ������ ATTACK ���·� ��ȯ
            if (distanceToTarget <= attackRange)
            {
                nmAgent.isStopped = true;
                ChangeState(State.ATTACK);
            }

            yield return null;
        }
    }

    // ATTACK ���� �ڷ�ƾ
    private IEnumerator ATTACK()
    {
        animatorEnemy.SetTrigger("Attack"); //  ���ݸ�� �ִϸ��̼� Ʈ����

        if (SeePlayer)
        {   // ���� �÷��̾ ���� �� �Ҹ�
            NormalEnemySeePlayerSound();
            SeePlayer = false;
        }

        canMove = false;
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
                transform.LookAt(target.GetChild(0));

                lastAttackTime = Time.time;
                animatorEnemy.SetTrigger("Shoot"); // �߻�ü �߻� �ִϸ��̼� Ʈ����
                yield return new WaitForSeconds(0.5f); // ���� �ִϸ��̼� �ð� ���
            }
        }
        else
        {
            ChangeState(State.CHASE);
        }

        yield return null;
    }

    // DIE ���� �ڷ�ƾ
    private IEnumerator DIE()
    {
        NolmalEnemyDieSound(); // �� ��� ���� ó��
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

    private float randomOffset = 0.1f; // ���� ������ �߰�
    // �߻�ü �߻� �޼���
    public void ShootBullet()
    {
        GameObject bullet = bulletPool.GetBullet(); // Ǯ���� �Ѿ� ��������
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.transform.rotation = Quaternion.identity;

        // ��ǥ�� ��ġ�� ���� ������ �߰�
        Vector3 randomTargetPosition = target.position;
        randomTargetPosition.x += Random.Range(-randomOffset, randomOffset);
        randomTargetPosition.y += Random.Range(-randomOffset, 0.0f);
        randomTargetPosition.z += Random.Range(-randomOffset, randomOffset);

        // �Ѿ� ���� ���
        Vector3 direction = (randomTargetPosition - bulletSpawnPoint.position).normalized;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSpeed;

        CBullet bulletScript = bullet.GetComponent<CBullet>();
        damage = Random.Range(3.0f, 5.0f);
        bulletScript.Initialize(damage, bulletPool);
    }

    // �¾��� �� �޼���
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
                target = playerObject.transform.GetChild(0); // �÷��̾��� ������ Ʈ������ ����
                attackRange = 100f;
                ChangeState(State.ATTACK);
            }
        }
        else if (state == State.CHASE)
        {
            if (target != null)
            {
                target = GameObject.FindGameObjectWithTag("Player")?.transform.GetChild(0);
                attackRange = 100f;
                ChangeState(State.ATTACK);
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

    private void NormalEnemyShotSound()
    {
        CEnemySoundManager.Instance.PlayEnemySound(0, transform.position);
    }

    private void NormalEnemyFootStebSound()
    {
        CEnemySoundManager.Instance.PlayEnemySound(1, transform.position);
    }
    private void NormalEnemySeePlayerSound()
    {
        CEnemySoundManager.Instance.PlayEnemySound(2, transform.position);
    }
    private void NolmalEnemyDieSound()
    {
        CEnemySoundManager.Instance.PlayEnemySound(3, transform.position);
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
