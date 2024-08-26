using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBossCircleIceShards : MonoBehaviour
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

        CBossTriggerRelay[] relays = GetComponentsInChildren<CBossTriggerRelay>();
        foreach (CBossTriggerRelay relay in relays)
        {
            relay.parentScript = this;
        }
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

        // �߰����� ���
        Vector3 midPoint = (startPoint + targetPosition) / 2f;

        // Y���� ���� �ݿ��� ��θ� �����ϱ� ���� ������ ���
        float heightOffset = Mathf.Sin(curveProgress * Mathf.PI) * curveRadius;
        Vector3 curvedPosition = Vector3.Lerp(startPoint, targetPosition, curveProgress);
        curvedPosition.y = startPoint.y + heightOffset;

        // ���� ����ü�� ���ο� � ��ġ�� �̵�
        transform.position = curvedPosition;
    }

    // �ڽ� ��ü�� Ʈ���� �̺�Ʈ�� ó���ϴ� �޼���
    public void OnChildTriggerEnter(Collider other)
    {
        // �÷��̾���� �浹 ó��
        if (other.CompareTag("Player"))
        {
            CEnemySoundManager.Instance.PlayBossSound(2, transform.position);
            if (other.TryGetComponent<CPlayerController>(out CPlayerController playerController))
            {
                // �÷��̾�� ������ ����
                playerController.Hit(damage);

                // ���� ��ġ�� ��ƼŬ ����Ʈ ����
                if (hitParticlePrefab != null)
                {
                    GameObject particleEffect = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
                    Destroy(particleEffect, 2f); // ���� �ð� ����
                }
            }
        }
        // �� �Ǵ� �ٸ� ��ü���� �浹 ó��
        else if (other.CompareTag("Wall"))
        {
            // ��ƼŬ ����Ʈ ����
            if (hitParticlePrefab != null)
            {
                GameObject particleEffect = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
                Destroy(particleEffect, 2f); // ���� �ð� ����
            }

        }
    }
}