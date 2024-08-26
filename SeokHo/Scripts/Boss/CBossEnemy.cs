using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum State
{
    IDLE,   // ��� ����
    CHASE,  // ���� ����
    ATTACK, // ���� ����
    DIE, // ��� ����
}

public class CBossEnemy : MonoBehaviour, IHittable
{
    #region ����
    // UI ���� ����
    public GameObject hpBarPrefab;  // HP �� ������
    public GameObject damageTextPrefab; // ������ �ؽ�Ʈ ������ 
    public Vector3 v3HpBar;  // HP �� ��ġ ������
    public Slider enemyHpbar; // ���� HP �� �����̴�
    private UIDamagePool damagePool; // ������ UI Ǯ
    private UIEnemyHpBar hpBarScript;
    private GameObject hpBarCanvas; // ���� HP �� ĵ����
    private Coroutine hideHpBarCoroutine;

    // ���� ���� ����
    public State state;  // ���� ����
    public float startingHealth; // ���� ü��
    private float health; //  ���� ü��
    private float spearDamage; // ���̽� â ���� ���ݷ�
    private float snowBallDamage; // ����� �� ���� ���ݷ�
    private float iceShardDamage; // ���� ���� ���� ���ݷ�
    private float IceCircleShardsDamage; // ���� ���� ���� ���ݷ�

    public float attackRange; // ���� ��Ÿ�
    public Transform target; // ���� ���
    private Animator AnimatorBoss;
    public Animator AnimatorBossWing;
    private float attackDelay = 5f; // ���� ����
    private float lastAttackTime; // ������ ���� ����
    private Vector3 moveToPosition = new Vector3(0, 0, 0); // ������ �̵��� ������ ��ġ
    private bool hasMovedToPosition = false; // ������ �̵� �Ϸ��ߴ��� üũ�ϴ� ����
    private float moveSpeed = 1.5f; // ���� �̵� �ӵ�
    private float rotationSpeed = 5f; // ���� ȸ�� �ӵ�
    public GameObject iceSpearPrefab; // ���̽� â ������
    public GameObject snowBallPrefab; // ����� �� ������
    public GameObject iceShardPrefab; // ���� ���� ������
    public GameObject iceShardsPrefab; // ���� ���� ���� ������
    public GameObject IceCircleShardsPrefab; // ���� ���� ���� ������
    public Animation iceSpear;
    public GameObject onPlayDefeateParticle;
    public GameObject onPlayDeathParticle;
    public GameObject onPlayBloodParticle;
    public GameObject soulPrefab; // ��ȥ ������


    private enum AttackType
    {
        SpearAttack,
        SnowBallAttack,
        CircleAttack,
        IceShardsAttack,
        HorizontalAttack
    }
    #endregion

    void Awake()
    {
        SetHpBar();

        damagePool = FindObjectOfType<UIDamagePool>();

        AnimatorBoss = GetComponent<Animator>();

        // Ÿ�� �ڵ� �Ҵ�
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform.GetChild(0);
            }
        }

        state = State.IDLE;
        StartCoroutine(StateMachine());
    }

    void Update()
    {
        // �÷��̾ �Ĵٺ��� ȸ��
        if (state == State.ATTACK && target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // �÷��̾���� �Ÿ� ����
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget > attackRange)
            {
                Vector3 moveDirection = directionToTarget * moveSpeed * Time.deltaTime;
                transform.position += moveDirection;
            }
        }
    }

    #region ���� �ӽ�
    public IEnumerator StateMachine()
    {
        while (health > 0)
        {
            switch (state)
            {
                case State.IDLE:
                    yield return StartCoroutine(IDLE());
                    break;
                case State.ATTACK:
                    yield return StartCoroutine(ATTACK());
                    break;
            }
        }
    }

    public void ChangeState(State newState)
    {
        state = newState;
    }

    private IEnumerator IDLE()
    {
        while (state == State.IDLE)
        {
            yield return null;
        }
    }

    private IEnumerator ATTACK()
    {
        // �̵��� ��ǥ ��ġ ����
        if (!hasMovedToPosition)
        {
            moveToPosition = transform.position + new Vector3(0, 15f, 10f); // ���� ��ġ
            StartCoroutine(MoveToPosition());
            yield return null; // �̵��� �Ϸ�� ������ ��ٸ�
        }

        while (state == State.ATTACK)
        {
            if (target != null)  // target�� null�� �ƴ��� Ȯ��
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (distanceToTarget <= attackRange)
                {
                    if (lastAttackTime + attackDelay <= Time.time)
                    {
                        transform.LookAt(target.GetChild(0));
                        lastAttackTime = Time.time;

                        // ���� Ÿ�� ���� �� �ִϸ��̼� Ʈ����
                        AttackType attackType = (AttackType)Random.Range(0, 4); // 0���� 4������ ���� ��
                        switch (attackType)
                        {
                            case AttackType.SpearAttack:
                                // SpearAttack �ִϸ��̼� �۵��� CreateIceSpear ȣ��
                                AnimatorBoss.SetTrigger("SpearAttack");
                                break;
                            case AttackType.SnowBallAttack:
                                AnimatorBoss.SetTrigger("SnowBallAttack");
                                break;
                            case AttackType.CircleAttack:
                                AnimatorBoss.SetTrigger("CircleAttack");
                                break;
                            case AttackType.IceShardsAttack:
                                AnimatorBoss.SetTrigger("IceShardsAttack");
                                break;
                            case AttackType.HorizontalAttack:
                                AnimatorBoss.SetTrigger("HorizontalAttack");
                                break;
                        }

                        // �ִϸ��̼� ���� �ð� ���
                        yield return new WaitForSeconds(6f); // 6�� ���
                    }
                }
            }
            yield return null;
        }
    }

    #endregion



    public void Hit(float damage)
    {
        ShowHpBar();

        // ���� ó��
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
            AnimatorBoss.SetTrigger("Death");

            return; // ü���� 0 ������ ��� �� �̻��� ó���� �ߴ�
        }

        // ��� ������ �� Hit�� ���� ���·� ��ȯ
        if (state == State.IDLE)
        {
            AnimatorBoss.SetTrigger("StartAttack");
            AnimatorBossWing.SetTrigger("StartAttack");

            if (!hasMovedToPosition)
            {
                // �̵��� ��ǥ ��ġ ���� (��: ������ ���� ��ġ���� �ణ ������ ��ġ)
                moveToPosition = transform.position + new Vector3(0, 3f, 7f); // ���� ��ġ
                StartCoroutine(MoveToPosition());
            }
            else
            {
                // �̵��� �Ϸ�� �� ���� ���·� ��ȯ
                ChangeState(State.ATTACK);
            }
        }
        // ���� ������ �� Hit��
        else if (state == State.ATTACK)
        {
            float randomChance = Random.value;

            if (randomChance < 0.3f) // 30% Ȯ���� ���� �뽬
            {
                AnimatorBoss.SetTrigger("QuickDash_Left");
            }
            else if (randomChance < 0.6f) // �߰� 30% Ȯ���� ������ �뽬
            {
                AnimatorBoss.SetTrigger("QuickDash_Right");
            }
            // 40% Ȯ���� �ƹ��͵� ���� ����
            else
            {
                // �ƹ� �ൿ�� ���� ����
            }
        }
    }

    #region Animation ����

    private IEnumerator MoveToPosition()
    {
        hasMovedToPosition = true;

        // �̵� ����
        while (Vector3.Distance(transform.position, moveToPosition) > 0.5f)
        {
            Vector3 moveDirection = (moveToPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            yield return null;
        }

        // �̵� �Ϸ� �� ��ġ ����
        transform.position = moveToPosition;

        // �̵� �� Attack ���·� ��ȯ
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform.GetChild(0);
            }
        }

        ChangeState(State.ATTACK); // �̵��� �Ϸ�� �� ���� ��ȯ
    }

    // 1.���� ���� IceSpear ����
    public void CreateIceSpear()
    {
        // IceSpear �������� ����ִ��� Ȯ��
        if (iceSpearPrefab == null)
        {
            Debug.LogWarning("IceSpearPrefab�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 2.0f;

        // IceSpear �������� ���� ��ġ���� ����
        GameObject iceSpear = Instantiate(iceSpearPrefab, spawnPosition, Quaternion.identity);

        // IceSpear �� ����� Ÿ�� ����
        CBossIceSpear iceSpearScript = iceSpear.GetComponent<CBossIceSpear>();
        if (iceSpearScript != null)
        {
            spearDamage = Random.Range(7.0f, 9.0f);
            iceSpearScript.Initialize(spearDamage);
            iceSpearScript.SetTarget(target);
        }
    }
    public void SpearAttack()
    {
        iceSpear.Play("SpearAttack");
    }

    // 2.���� ���� SnowBall ����
    public void CreateSnowBall()
    {
        // SnowBall �������� ����ִ��� Ȯ��
        if (snowBallPrefab == null)
        {
            Debug.LogWarning("SnowBallPrefab�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 4.4f;

        // SnowBall �������� ���� ��ġ���� ����
        GameObject snowBall = Instantiate(snowBallPrefab, spawnPosition, Quaternion.identity);

        // SnowBall �� ����� Ÿ�� ����
        CBossSnowBall snowBallScript = snowBall.GetComponent<CBossSnowBall>();
        if (snowBallScript != null)
        {
            snowBallDamage = Random.Range(7.0f, 9.0f);
            snowBallScript.Initialize(snowBallDamage);
            snowBallScript.SetTarget(target);
        }
    }

    // 3.���� ���� �������� ���ư��� ���� ����ü�� ����
    public void CreateHorizoniceShard()
    {
        // iceShardPrefab�� �Ҵ�Ǿ����� Ȯ��
        if (iceShardPrefab == null)
        {
            Debug.LogWarning("iceShardPrefab�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        // ���� ��ġ ���� (���� ��ġ���� y������ 2.0f ��)
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 2.0f;

        // Ÿ�� ��ġ�� ���� ������ ĸó
        Vector3 targetPosition = target.position;

        // ���� ����ü�� �ʱ� ȸ������ (90, 90, 90)���� ����
        Quaternion initialRotation = Quaternion.Euler(90, 90, 90);

        // ���� ����ü �������� ���� ��ġ�� �����ϰ� �ʱ� ȸ���� ����
        GameObject iceShard = Instantiate(iceShardPrefab, spawnPosition, initialRotation);

        // ���� ����ü�� ����� Ÿ�� ����
        CBossHorizonIceShard iceShardScript = iceShard.GetComponent<CBossHorizonIceShard>();
        if (iceShardScript != null)
        {
            iceShardDamage = Random.Range(4.0f, 6.0f);
            iceShardScript.Initialize(iceShardDamage);
            iceShardScript.SetTarget(targetPosition);
        }
    }


    // 4.���� ���� ���� ���� ����ü�� ����
    public void CreateIceShards()
    {
        if (iceShardsPrefab == null)
        {
            Debug.LogWarning("iceShardsPrefab�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        Vector3 spawnPosition = transform.position;
        spawnPosition.y -= 5f;


        // IceShards �������� ���� ��ġ���� ����
        GameObject IceShards = Instantiate(iceShardsPrefab, transform.position, Quaternion.identity);

        // IceShards �� ����� Ÿ�� ����
        CBossIceShards IceShardsScript = IceShards.GetComponent<CBossIceShards>();
        if (IceShardsScript != null)
        {
            IceShardsScript.SetTarget(target);
        }
    }

    // 5.���� ���� �� ������ ���� ����ü ����
    public void CreateIceCircleShards()
    {
        // iceShardPrefab�� �Ҵ�Ǿ����� Ȯ��
        if (IceCircleShardsPrefab == null)
        {
            Debug.LogWarning("IceCircleShardsPrefab�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        // Ÿ�� ��ġ�� ���� ������ ĸó
        Vector3 targetPosition = target.position;

        // ���� ����ü�� �ʱ� ȸ������ (90, 90, 90)���� ����
        Quaternion initialRotation = Quaternion.Euler(90, 90, 90);

        // ���� ����ü �������� ���� ��ġ�� �����ϰ� �ʱ� ȸ���� ����
        GameObject iceCircleShardsPrefab = Instantiate(IceCircleShardsPrefab, transform.position, initialRotation);

        // ���� ����ü�� ����� Ÿ�� ����
        CBossCircleIceShards iceShardScript = iceCircleShardsPrefab.GetComponent<CBossCircleIceShards>();
        if (iceShardScript != null)
        {
            IceCircleShardsDamage = Random.Range(4.0f, 6.0f);
            iceShardScript.Initialize(IceCircleShardsDamage);
            iceShardScript.SetTarget(targetPosition);
        }
    }

    // �׾��� �� ��ƼŬ �ִϸ��̼� �̺�Ʈ
    public void OnPlayDeathParticles()
    {
        Vector3 particlePosition = transform.position;
        particlePosition.y += 1f; // y�� ��ġ�� ����

        GameObject OnPlayBloodParticle = Instantiate(onPlayBloodParticle, particlePosition, Quaternion.identity);
        Destroy(OnPlayBloodParticle, 5f); // ��ƼŬ ȿ���� ���� �ð� ����
        GameObject OnPlayDeathParticle = Instantiate(onPlayDeathParticle, particlePosition, Quaternion.identity);
        Destroy(OnPlayDeathParticle, 3.5f); // ��ƼŬ ȿ���� ���� �ð� ����
        GameObject OnPlayDefeateParticle = Instantiate(onPlayDefeateParticle, particlePosition, Quaternion.identity);
        Destroy(OnPlayDefeateParticle, 5f); // ��ƼŬ ȿ���� ���� �ð� ����
    }

    // �׾��� �� �ִϸ��̼� �̺�Ʈ
    public void DieAnimation()
    {

        Destroy(hpBarCanvas, 1f);
        StartCoroutine(DieTime());
    }

    // ������Ʈ ��Ȱ��ȭ���� �ð�
    private IEnumerator DieTime()
    {
        // 1�� �Ŀ� ��ȥ ���� �� �̵� ����
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            GameObject soul = Instantiate(soulPrefab, transform.position, Quaternion.identity);
            StartCoroutine(MoveSoulToTarget(soul, target));
        }
        // 3�� ���
        yield return new WaitForSeconds(3.5f);
        gameObject.SetActive(false);

        Destroy(gameObject, 2.0f); // �� �� �Ŀ� ���� ������Ʈ �ı�
    }

    #endregion

    #region ���� ����

    private void BossDodgeSound()
    {
        CEnemySoundManager.Instance.PlayBossSound(0, transform.position);
    }
    private void BossThrowShardSound()
    {
        CEnemySoundManager.Instance.PlayBossSound(1, transform.position);
    }
    private void BossSpearCreateSound()
    {
        CEnemySoundManager.Instance.PlayBossSound(4, transform.position);
    }
    private void BossSpearThrowSound()
    {
        CEnemySoundManager.Instance.PlayBossSound(5, transform.position);
    }
    private void BossSnowBallSound()
    {
        CEnemySoundManager.Instance.PlayBossSound(9, transform.position);
    }
    private void BossDeathSound()
    {
        CEnemySoundManager.Instance.PlayBossSound(11, transform.position);
    }

    #endregion

    #region HPBar ����
    private void CheckHp()
    {
        if (enemyHpbar != null)
        {
            enemyHpbar.value = health;
        }
    }

    private void SetHpBar()
    {
        health = startingHealth;
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
}
