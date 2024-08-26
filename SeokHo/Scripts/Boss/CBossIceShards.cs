using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBossIceShards : MonoBehaviour
{
    private Transform target;
    public GameObject hitParticlePrefab; // �浹 �� ����� ��ƼŬ �ý��� ����

    private void Start()
    {
         // 5�� �Ŀ� ��ü�� �ı�
         Destroy(gameObject, 3f);
    }

    // Ÿ���� �����ϴ� �޼���
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    void Update()
    {
        // Ÿ���� ������ ������Ʈ�� �ı�
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }


        // Ÿ�� �������� ȸ��
        transform.LookAt(target);
    }

    //private void OnParticleTrigger()
    //{
    //    Debug.Log("����");
    //    // �÷��̾�� �浹 ó��
    //    if (CompareTag("Player"))
    //    {
    //        if (TryGetComponent<CPlayerController>(out CPlayerController playerController))
    //        {
    //            // �÷��̾�� ������ ����
    //            playerController.Hit(damage);

    //            // ��ƼŬ ����Ʈ�� ���� ��ġ�� ȸ������ �ν��Ͻ�ȭ
    //            if (hitParticlePrefab != null)
    //            {
    //                GameObject particleEffect = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
    //                Destroy(particleEffect, 2f); // ��ƼŬ ȿ���� ���� �ð� ����
    //            }
    //        }
    //    }
    //    // �� �Ǵ� �ٸ� ��ü���� �浹 ó��
    //    else if (CompareTag("Wall"))
    //    {
    //        // �� �÷��̾� ��ü�� �浹 �� ��ƼŬ ����Ʈ ���
    //        if (hitParticlePrefab != null)
    //        {
    //            GameObject particleEffect = Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);
    //            Destroy(particleEffect, 2f); // ��ƼŬ ȿ���� ���� �ð� ����
    //        }
    //    }
    //}
}
