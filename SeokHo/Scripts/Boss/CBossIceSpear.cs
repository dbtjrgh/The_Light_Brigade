using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBossIceSpear : MonoBehaviour
{
    public float speed; // â�� �ӵ�
    private Transform target;
    private float damage; // ������ ���� �߰�
    public GameObject hitParticlePrefab; // �浹 �� ����� ��ƼŬ �ý��� ������

    private void Start()
    {
        // 3�� �Ŀ� ��ü�� �ı�
        Destroy(gameObject, 3f);
    }

    // Ÿ���� �����ϴ� �޼���
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    // �������� �ʱ�ȭ�ϴ� �޼���
    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    void Update()
    {
        // Ÿ���� ������ â�� �ı�
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Ÿ���� ���� �̵�
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Ÿ�� �������� ȸ��
        transform.LookAt(target);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �浹 ó��
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<CPlayerController>(out CPlayerController playerController))
            {
                CEnemySoundManager.Instance.PlayBossSound(6, transform.position);
                CEnemySoundManager.Instance.PlayBossSound(7, transform.position);
                CEnemySoundManager.Instance.PlayBossSound(8, transform.position);
                // �÷��̾�� ������ ����
                playerController.Hit(damage);

                // ��ƼŬ ����Ʈ�� ���� ��ġ�� ȸ������ �ν��Ͻ�ȭ
                if (hitParticlePrefab != null)
                {
                    Vector3 particlePosition = transform.position;
                    particlePosition.y -= 1; // y�� ��ġ�� -1�� ����
                    GameObject particleEffect = Instantiate(hitParticlePrefab, particlePosition, Quaternion.identity);
                    Destroy(particleEffect, 2f); // ��ƼŬ ȿ���� ���� �ð� ����
                }

                // â ��ü�� ���� �ð� �Ŀ� �ı�
                Destroy(gameObject);
            }
        }
        // �� �Ǵ� �ٸ� ��ü���� �浹 ó��
        else if (other.CompareTag("Wall"))
        {
            // �� �÷��̾� ��ü�� �浹 �� ��ƼŬ ����Ʈ ���
            if (hitParticlePrefab != null)
            {
                CEnemySoundManager.Instance.PlayBossSound(7, transform.position);
                CEnemySoundManager.Instance.PlayBossSound(8, transform.position);
                Vector3 particlePosition = transform.position;
                particlePosition.y -= 1; // y�� ��ġ�� -1�� ����
                GameObject particleEffect = Instantiate(hitParticlePrefab, particlePosition, Quaternion.identity);
                Destroy(particleEffect, 2f); // ��ƼŬ ȿ���� ���� �ð� ����
            }

            // â ��ü�� ���� �ð� �Ŀ� �ı�
            Destroy(gameObject);
        }
    }
}
