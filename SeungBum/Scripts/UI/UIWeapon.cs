using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIWeapon : MonoBehaviour
{
    #region private ����
    [SerializeField]
    Image imgBullet;
    [SerializeField]
    Image imgHP;
    [SerializeField]
    TMP_Text tmpBulletCount;
    [SerializeField]
    TMP_Text tmpHP;
    #endregion

    /// <summary>
    /// �Ѿ� ���� Text�� �����Ѵ�.
    /// </summary>
    /// <param name="count">���� �Ѿ� ����</param>
    public void ChangeBulletCount(int count)
    {
        tmpBulletCount.text = count.ToString();
    }

    /// <summary>
    /// �Ѿ� UI(Image, TMP)�� Color�� �����Ѵ�.
    /// </summary>
    /// <param name="color">������ ��</param>
    public void ChangeBulletUIColor(Color color)
    {
        imgBullet.color = color;
        tmpBulletCount.color = color;
    }

    /// <summary>
    /// HP Text�� �����Ѵ�.
    /// </summary>
    /// <param name="count">���� HP</param>
    public void ChangeHPCount(int count)
    {
        tmpHP.text = count.ToString();
    }

    /// <summary>
    /// HP UI(Image, TMP)�� Color�� �����Ѵ�.
    /// </summary>
    /// <param name="color">������ ��</param>
    public void ChangeHPUIColor(Color color)
    {
        imgHP.color = color;
        tmpHP.color = color;
    }
}