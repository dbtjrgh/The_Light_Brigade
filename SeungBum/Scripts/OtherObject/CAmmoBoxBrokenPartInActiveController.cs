using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAmmoBoxBrokenPartInActiveController : MonoBehaviour
{
    void OnEnable()
    {
        Invoke("InActiveBrokenPart", 2.0f);
    }

    /// <summary>
    /// �μ��� ���ظ� ��Ȱ��ȭ�ϴ� �޼���
    /// </summary>
    void InActiveBrokenPart()
    {
        gameObject.SetActive(false);
    }
}