using System.Collections;
using UnityEngine;

public class CBossHorizonIceShard : MonoBehaviour
{
    public float speed; // ���� ����ü �ӵ�
    private Vector3 targetPosition; // Ÿ�� ��ġ
    private float damage; // ������ ��
    public GameObject hitParticlePrefab; // �浹 �� ����� ��ƼŬ �ý��� ������
    private Vector3 startPoint; // ����ü�� ���� ��ġ
    private float curveProgress = 0f; // � ���൵

    private void Start()
    {
        // 3�� �Ŀ� ��ü�� �ı�
        Destroy(gameObject, 3f);
    }

    // Ÿ�� ��ġ ���� �޼���
    public void SetTarget(Vector3 targetPos)
    {
        targetPosition = targetPos;
        startPoint = transform.position; // Ÿ���� ������ �� ���� ��ġ ���
    }

    // ������ �ʱ�ȭ �޼���
    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    void Update()
    {
        // �ݿ��� ��� ���
        curveProgress += speed * Time.deltaTime;
        float curveRadius = Vector3.Distance(startPoint, targetPosition) / 2f;

        // ���� ��ġ�� Ÿ�� ��ġ�� �߰��� ���
        Vector3 midPoint = (startPoint + targetPosition) / 2f;
        midPoint.y = startPoint.y; // �߰����� ���̴� ���� ��ġ�� �����ϰ� ����

        // �ݿ��� ��θ� �����ϱ� ���� ������ ���
        Vector3 offset = Vector3.Cross(targetPosition - startPoint, Vector3.up).normalized * curveRadius;

        // ������ ����Ͽ� � ���� ��ġ ���
        Vector3 curvedPosition = Vector3.Lerp(Vector3.Lerp(startPoint, midPoint + offset, curveProgress), Vector3.Lerp(midPoint + offset, targetPosition, curveProgress), curveProgress);

        // ���� ����ü�� ���ο� � ��ġ�� �̵�
        transform.position = curvedPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾���� �浹 ó��
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<CPlayerController>(out CPlayerController playerController))
            {
                CEnemySoundManager.Instance.PlayBossSound(2, transform.position);
                // �÷��̾�� ������ ����
                playerController.Hit(damage);

                // ���� ��ġ�� ��ƼŬ ����Ʈ ����
                if (hitParticlePrefab != null)
                {
                    GameObject particleEffect = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
                    Destroy(particleEffect, 2f); // ���� �ð� ����
                }

                // ���� ����ü �ı�
                Destroy(gameObject);
            }
        }
        // �� �Ǵ� �ٸ� ��ü���� �浹 ó��
        else if (other.CompareTag("Wall"))
        {
            CEnemySoundManager.Instance.PlayBossSound(3, transform.position);
            // ��ƼŬ ����Ʈ ����
            if (hitParticlePrefab != null)
            {
                GameObject particleEffect = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
                Destroy(particleEffect, 2f); // ���� �ð� ����
            }

            // ���� ����ü �ı�
            Destroy(gameObject);
        }
    }
}