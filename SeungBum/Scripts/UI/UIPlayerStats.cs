using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerStats : MonoBehaviour
{
    #region private ����
    [SerializeField]
    TMP_Text tmpLife;
    [SerializeField]
    TMP_Text tmpHP;
    [SerializeField]
    TMP_Text tmpSoul;
    [SerializeField]
    TMP_Text tmpAmmo;
    #endregion

    /// <summary>
    /// Player Stats UI�� Life Text ����
    /// </summary>
    /// <param name="nowLifeCount">�÷��̾� ���� ���</param>
    /// <param name="maxLifeCount">�÷��̾� �ִ� ���</param>
    public void ChangeLifeCount(int nowLifeCount, int maxLifeCount)
    {
        tmpLife.text = $"{nowLifeCount}/{maxLifeCount}";
    }

    /// <summary>
    /// Player Stats UI�� HP Text ����
    /// </summary>
    /// <param name="nowHPCount">�÷��̾� ���� ü��</param>
    /// <param name="maxHPCount">�÷��̾� �ִ� ü��</param>
    public void ChangeHPText(int nowHPCount, int maxHPCount)
    {
        tmpHP.text =
            (nowHPCount >= 4) ?
            $"<color=white>{nowHPCount}</color>/{maxHPCount}"
            :
            $"<color=red>{nowHPCount}</color>/{maxHPCount}";
    }

    /// <summary>
    /// Player Stats UI�� Soul Text ����
    /// </summary>
    /// <param name="soulCount">�÷��̾� ���� ��ȥ</param>
    public void ChangeSoulText(int soulCount)
    {
        tmpSoul.text = soulCount.ToString();
    }

    /// <summary>
    /// Player Stats UI�� Ammo Text ����
    /// </summary>
    /// <param name="ammoCount">�÷��̾� ���� źâ</param>
    public void ChangeAmmoText(int ammoCount)
    {
        tmpAmmo.text = ammoCount.ToString();
    }
}