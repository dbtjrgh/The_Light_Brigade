using System.Collections;
using UnityEngine;

public class CBossSnowBall : MonoBehaviour
{
    public float speed; // ����캼�� �̵� �ӵ�
    private Transform target; // ��ǥ �÷��̾��� Transform
    private float damage; // ����캼�� ������
    public GameObject hitParticlePrefab; // �浹 �� ����� ��ƼŬ �ý��� ������
    public Material snowBallMaterial; // ����캼�� ����
    private Renderer snowBallRenderer; // ����캼�� Renderer ������Ʈ

    private bool isVisible = false; // ����캼�� ���̴��� ����


    private void Start()
    {
        // ����캼�� Renderer ������Ʈ�� ������
        snowBallRenderer = GetComponent<Renderer>();

        if (snowBallRenderer == null)
        {
            Debug.LogWarning("����캼�� Renderer ������Ʈ�� �����ϴ�.");
            return;
        }

        // ����캼�� 1.4�� ���� ���� 0���� 1�� ����
        StartCoroutine(HandleVisibilityAndMovement());
        // 3�� �Ŀ� ��ü�� �ı�
        Destroy(gameObject, 3f);
    }

    // ��ǥ ���� �޼���
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    // ������ �ʱ�ȭ �޼���
    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    private void Update()
    {
        // ����캼�� ������ ������ ������Ʈ�� ���� ����
        if (!isVisible) return;

        // ��ǥ�� ������ ����캼 ����
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // ��ǥ�� ���� �̵�
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // ��ǥ�� ���� ȸ��
        transform.LookAt(target);
    }

    private void OnTriggerEnter(Collider other)
    {

        // �÷��̾�� �浹 ó��
        if (other.CompareTag("Player"))
        {
            CEnemySoundManager.Instance.PlayBossSound(10, transform.position);
            if (other.TryGetComponent<CPlayerController>(out CPlayerController playerController))
            {
                // �÷��̾�� ������ ����
                playerController.Hit(damage);

                // �浹 �� ��ƼŬ ȿ�� ����
                if (hitParticlePrefab != null)
                {
                    Vector3 particlePosition = transform.position;
                    particlePosition.y -= 1; // y�� ��ġ�� -1�� ����
                    GameObject particleEffect = Instantiate(hitParticlePrefab, particlePosition, Quaternion.identity);
                    Destroy(particleEffect, 2f); // ��ƼŬ ȿ���� ���� �ð� ����
                }

                // ����캼 ����
                Destroy(gameObject);
            }
        }
        // ���̳� �ٸ� ������Ʈ�� �浹 ó��
        else if (other.CompareTag("Wall"))
        {
            CEnemySoundManager.Instance.PlayBossSound(10, transform.position);
            // �浹 �� ��ƼŬ ȿ�� ����
            if (hitParticlePrefab != null)
            {
                Vector3 particlePosition = transform.position;
                particlePosition.y -= 1; // y�� ��ġ�� -1�� ����
                GameObject particleEffect = Instantiate(hitParticlePrefab, particlePosition, Quaternion.identity);
                Destroy(particleEffect, 2f); // ��ƼŬ ȿ���� ���� �ð� ����
            }

            // ����캼 ����
            Destroy(gameObject);
        }
    }

    // ���� ó���� ���� �ڷ�ƾ
    private IEnumerator HandleVisibilityAndMovement()
    {
        // ����캼�� ���� ���� ���·� ����
        if (snowBallMaterial != null && snowBallRenderer != null)
        {
            snowBallRenderer.material = snowBallMaterial;
            Color color = snowBallRenderer.material.color;
            color.a = 0f; // ���� ����
            snowBallRenderer.material.color = color;

            // 1.4�� ���� ���� 0���� 1�� ����
            float elapsedTime = 0f;
            while (elapsedTime < 1.4f)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / 1.4f);
                snowBallRenderer.material.color = color;
                yield return null;
            }

            // ���� 1�� ����
            color.a = 1f;
            snowBallRenderer.material.color = color;
        }

        // ����캼�� ���̰� ����
        isVisible = true;
    }
}
