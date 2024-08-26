using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAmmo : MonoBehaviour
{
    #region protected ����
    [SerializeField]
    protected GameObject[] oBullets;
    [SerializeField]
    protected List<AudioClip> soundEffect;

    protected EWeapon equipWeaponType;
    protected int nBulletMaxCount;
    protected int nBulletNowCount;
    #endregion

    /// <summary>
    /// źâ ��� �Ҹ�
    /// </summary>
    public AudioClip SoundGrab
    {
        get
        {
            return soundEffect[0];
        }
    }

    /// <summary>
    /// źâ ���� �Ҹ�
    /// </summary>
    public AudioClip SoundRelease
    {
        get
        {
            return soundEffect[1];
        }
    }

    /// <summary>
    /// ���� Ammo�� ������ �� �ִ� Weapon�� Ÿ��
    /// </summary>
    public EWeapon EquipWeaponType
    {
        get
        {
            return equipWeaponType;
        }
    }

    /// <summary>
    /// źâ�� �ִ� �Ѿ� ����
    /// </summary>
    public int BulletMaxCount
    {
        get
        {
            return nBulletMaxCount;
        }
    }

    /// <summary>
    /// źâ�� ���� �Ѿ� ����
    /// </summary>
    public int BulletNowCount
    {
        get
        {
            return nBulletNowCount;
        }
    }

    /// <summary>
    /// ���� �Ѿ��� ����
    /// </summary>
    public float RemainBulletPercent
    {
        get
        {
            return nBulletNowCount / (float)nBulletMaxCount;
        }
    }

    /// <summary>
    /// ���� �Ѿ��� ���� ������ ���δ�.
    /// </summary>
    public void DecreaseBulltCount()
    {
        nBulletNowCount--;
        oBullets[nBulletNowCount].SetActive(false);
    }
}
