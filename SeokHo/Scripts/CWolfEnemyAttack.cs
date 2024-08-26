using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CWolfEnemyAttack : MonoBehaviour
{
    private Collider AttackPoint;
    private CWolfEnemy wolfEnemy;

    private void Awake()
    {
        wolfEnemy = GetComponentInParent<CWolfEnemy>();
        AttackPoint = GetComponent<Collider>();

    }
    private void Start()
    {
        AttackPoint.enabled = true;
    }
    public void StartAttack()
    {
        AttackPoint.isTrigger = true;
    }

    public void EndAttack()
    {
        AttackPoint.isTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� �÷��̾����� Ȯ��
        if (other.CompareTag("Player"))
        {
            // other.GetComponent<CPlayerController>();
            // �÷��̾� ��Ʈ�ѷ� ���� ��������
            if (other.TryGetComponent<CPlayerController>(out CPlayerController playerController))
            {
                wolfEnemy.damage = Random.Range(4.0f, 6.0f);
                float damage = wolfEnemy.damage;
                // �÷��̾ ���ظ� ����
                playerController.Hit(damage);
            }
        }
    }
}
