using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeapon
{
    GEWEHR = 1,
    STG44 = 5
}

[System.Serializable]
public class CWeapon : CInteractable
{
    #region protected ����
    [SerializeField]
    protected List<AudioClip> soundEffect;

    protected EWeapon weaponType;

    protected float fShootCoolTime;
    protected float fDamage;
    protected float fRecoilTime;
    protected int nBulletMaxCount;
    protected int nBulletNowCount;
    #endregion

    /// <summary>
    /// Interaction�� ������ ��ü�� ������ �ִ� ���� ActionNumber�� �� (Hand Animation�� �����ϱ� ���Ѱ�), ���� ������ ����
    /// </summary>
    public override int ActionNumber
    {
        get
        {
            return nActionNumber;
        }

        set
        {
            nActionNumber = value;
        }
    }

    /// <summary>
    /// ���� ���� �Ѿ��� ����
    /// </summary>
    public int BulletCount
    {
        get
        {
            return nBulletNowCount;
        }
    }

    /// <summary>
    /// ���� ������
    /// </summary>
    public float Damage
    {
        get
        {
            float randomDamage = 0.0f;

            switch (weaponType)
            {
                case EWeapon.GEWEHR:
                    randomDamage = Random.Range(fDamage - 2.5f, fDamage + 2.5f);
                    break;

                case EWeapon.STG44:
                    randomDamage = Random.Range(fDamage - 1.0f, fDamage + 1.0f);
                    break;
            }

            randomDamage = Mathf.Floor(randomDamage * 10.0f) / 10.0f;

            return randomDamage;
        }
    }

    /// <summary>
    /// Weapon�� ����
    /// </summary>
    public EWeapon WeaponType
    {
        get
        {
            return weaponType;
        }

        protected set
        {
            weaponType = value;
        }
    }

    /// <summary>
    /// �� �߻� ��Ÿ��
    /// </summary>
    public float ShootCoolTime
    {
        get
        {
            return fShootCoolTime;
        }
    }

    /// <summary>
    /// �ѱ��� �ݵ��� ���۵Ǵ� �ð�
    /// </summary>
    public float RecoilTime
    {
        get
        {
            return fRecoilTime;
        }
    }

    /// <summary>
    /// �� ��� �Ҹ�
    /// </summary>
    public AudioClip SoundShot
    {
        get
        {
            return soundEffect[0];
        }
    }

    /// <summary>
    /// �� Trigger Grab �Ҹ�
    /// </summary>
    public AudioClip SoundTriggerGrab
    {
        get
        {
            return soundEffect[1];
        }
    }

    /// <summary>
    /// �� Barrel Grab �Ҹ�
    /// </summary>
    public AudioClip SoundBarrelGrab
    {
        get
        {
            return soundEffect[2];
        }
    }

    /// <summary>
    /// �� Bolt Grab �Ҹ�
    /// </summary>
    public AudioClip SoundBoltGrab
    {
        get
        {
            return soundEffect[3];
        }
    }

    /// <summary>
    /// ���� �Ҹ�
    /// </summary>
    public AudioClip SoundReload
    {
        get
        {
            return soundEffect[4];
        }
    }

    /// <summary>
    /// �Ѿ� ���� �� ��� �Ҹ�
    /// </summary>
    public AudioClip SoundEmptyShot
    {
        get
        {
            return soundEffect[5];
        }
    }

    /// <summary>
    /// źâ In �Ҹ�
    /// </summary>
    public AudioClip SoundAmmoInSound
    {
        get
        {
            return soundEffect[6];
        }
    }

    /// <summary>
    /// źâ Out �Ҹ�
    /// </summary>
    public AudioClip SoundAmmoOutSound
    {
        get
        {
            return soundEffect[7];
        }
    }

    /// <summary>
    /// �� ���� �Ҹ�(�κ��丮 In�̶� �Ȱ�����?)
    /// </summary>
    public AudioClip SoundReleaseWeapon
    {
        get
        {
            return soundEffect[8];
        }
    }

    /// <summary>
    /// �ѱ� ������. ���� BulletCount ���� Max�� �����.
    /// </summary>
    public void Reload(int nBulletCount)
    {
        if (nBulletCount >= nBulletMaxCount)
        {
            nBulletNowCount = nBulletMaxCount;
        }

        else
        {
            nBulletNowCount = nBulletCount;
        }
    }

    /// <summary>
    /// ���� �Ѿ��� ���� ������ ���δ�. Fire ���� �� ����
    /// </summary>
    public void DecreaseBulletCount()
    {
        nBulletNowCount--;
    }
}