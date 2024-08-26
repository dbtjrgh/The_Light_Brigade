using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CChasePlayer : MonoBehaviour
{
    CNormalEnemy normalenemy;
    CWolfEnemy wolfenemy;

    private void Start()
    {
        gameObject.GetComponent<Collider>().enabled = true;
        normalenemy = transform.parent.GetComponent<CNormalEnemy>();
        wolfenemy = transform.parent.GetComponent<CWolfEnemy>();
    }

    // Ž�� �Ÿ� 15f
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // normalenemy�� wolfenemy�� null�� �ƴ� ���� ���¸� ����
        if (normalenemy != null)
        {
            normalenemy.ChangeState(State.CHASE);
            normalenemy.target = other.transform.GetChild(0);
        }

        if (wolfenemy != null)
        {
            wolfenemy.ChangeState(State.CHASE);
            wolfenemy.target = other.transform;
        }
    }
}
