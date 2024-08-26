using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerStats : MonoBehaviour
{
    #region private ����
    int nMaxHP;
    int nHP;
    int nSoul;
    int nMaxLife;
    int nLife;
    int nMaxAmmoCount;
    int nAmmoCount;
    #endregion

    /// <summary>
    /// �÷��̾� �ִ� ü��
    /// </summary>
    public int MaxHP
    {
        get
        {
            return nMaxHP;
        }
    }

    /// <summary>
    /// �÷��̾� ���� ü��
    /// </summary>
    public int HP
    {
        get
        {
            return nHP;
        }
    }

    /// <summary>
    /// �÷��̾� ��ȥ
    /// </summary>
    public int Soul
    {
        get
        {
            return nSoul;
        }
    }

    /// <summary>
    /// �÷��̾� �ִ� ���
    /// </summary>
    public int MaxLife
    {
        get
        {
            return nMaxLife;
        }
    }

    /// <summary>
    /// �÷��̾� ���
    /// </summary>
    public int Life
    {
        get
        {
            return nLife;
        }
    }

    /// <summary>
    /// �÷��̾ ������ �� �ִ� �ִ� źâ ����
    /// </summary>
    public int MaxAmmo
    {
        get
        {
            return nMaxAmmoCount;
        }
    }

    /// <summary>
    /// �÷��̾� ���� źâ ����
    /// </summary>
    public int Ammo
    {
        get
        {
            return nAmmoCount;
        }
    }

    void Start()
    {
        nMaxHP = 20;
        nHP = 20;
        nSoul = 0;
        nMaxLife = 2;
        nLife = 2;
        nMaxAmmoCount = 10;
        nAmmoCount = 0;
    }

    /// <summary>
    /// �÷��̾� ü���� �����Ѵ�.
    /// </summary>
    /// <param name="hp">������ ü��</param>
    public void ChangeHP(int hp)
    {
        if (hp > nMaxHP)
        {
            hp = nMaxHP;
        }

        else if (hp < 0)
        {
            hp = 0;
        }

        nHP = hp;
    }

    /// <summary>
    /// �÷��̾� �ִ� ü���� �����Ѵ�.
    /// </summary>
    /// <param name="maxHp"></param>
    public void ChangeMaxHP(int maxHp)
    {
        nMaxHP = maxHp;
    }

    /// <summary>
    /// �÷��̾� �ҿ﷮�� �����Ѵ�
    /// </summary>
    /// <param name="soul"></param>
    public void ChangeSoul(int soul)
    {
        nSoul = soul;
    }

    /// <summary>
    /// �÷��̾� ����� �ϳ� ���ش�.
    /// </summary>
    public void DecreaseLife()
    {
        nLife--;
    }
}
