using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CWeaponController : MonoBehaviour
{
    #region private ����
    XRGrabInteractable grabInteractable;
    CWeapon weapon;
    CAmmo nowEquipAmmo;

    ActionBasedController leftController;
    ActionBasedController rightController;

    CHitExplosionParticlePool hitExplosionParticlePool;

    [SerializeField]
    UIWeapon weaponUI;

    Vector3 modelStartPosition;
    bool isFireReady;
    #endregion

    #region public ����
    public Transform bulletTransform;
    public Transform modelTransform;
    public GameObject oMuzzleParticle;
    public XRSocketInteractor ammoSoketInteractor;
    #endregion

    void OnEnable()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        hitExplosionParticlePool = FindObjectOfType<CHitExplosionParticlePool>();
        weapon = GetComponent<CWeapon>();
        nowEquipAmmo = null;

        grabInteractable.activated.AddListener(Fire);

        ammoSoketInteractor.selectEntered.AddListener(AddAmmo);
        ammoSoketInteractor.selectExited.AddListener(RemoveAmmo);

        isFireReady = true;

        modelStartPosition = modelTransform.localPosition;
    }

    void OnDisable()
    {
        if (grabInteractable is not null)
        {
            grabInteractable.activated.RemoveListener(Fire);
        }

        if (ammoSoketInteractor is not null)
        {
            ammoSoketInteractor.selectEntered.RemoveListener(AddAmmo);
            ammoSoketInteractor.selectExited.RemoveListener(RemoveAmmo);
        }
    }

    /// <summary>
    /// WeaponUI Class ��ȯ
    /// </summary>
    public UIWeapon WeaponUI
    {
        get
        {
            return weaponUI;
        }
    }

    /// <summary>
    /// ���⸦ ���� ������ �׷����� �� leftController�� �Ҵ�
    /// </summary>
    /// <param name="args"></param>
    public void GrabLeftController(SelectEnterEventArgs args)
    {
        leftController = args.interactorObject.transform.parent.GetComponent<ActionBasedController>();
    }

    /// <summary>
    /// ���⸦ ������ ������ �׷����� �� rightController�� �Ҵ�
    /// </summary>
    /// <param name="args"></param>
    public void GrabRightController(SelectEnterEventArgs args)
    {
        rightController = args.interactorObject.transform.parent.GetComponent<ActionBasedController>();
    }

    /// <summary>
    /// ���� ���� �׷��� Release���� �� leftController�� ����
    /// </summary>
    public void ReleaseLeftController()
    {
        leftController = null;
    }

    /// <summary>
    /// ������ ���� �׷��� Release���� �� rightController�� ����
    /// </summary>
    public void ReleaseRightController()
    {
        rightController = null;
    }

    /// <summary>
    /// �Ѿ��� �߻��ϱ����� �޼���
    /// </summary>
    /// <param name="eventArgs">ActiveEvent</param>
    public void Fire(ActivateEventArgs eventArgs)
    {
        if (!isFireReady)
        {
            return;
        }

        if (nowEquipAmmo is not null && nowEquipAmmo.BulletNowCount > 0)
        {
            CPlayerSoundManager.Instance.PlaySoundOneShot(weapon.SoundShot);
            nowEquipAmmo.DecreaseBulltCount();

            weaponUI.ChangeBulletCount(nowEquipAmmo.BulletNowCount);
            weaponUI.ChangeBulletUIColor((nowEquipAmmo.RemainBulletPercent >= 0.4f) ? Color.white : Color.red);
            Debug.LogFormat("�Ѿ� �߻�! ���� �Ѿ� ���� : {0}", nowEquipAmmo.BulletNowCount);

            RaycastHit ray;
            if (Physics.Raycast(bulletTransform.position, bulletTransform.forward, out ray, float.MaxValue))
            {
                if (ray.transform.TryGetComponent<IHittable>(out IHittable hit))
                {
                    hit.Hit(weapon.Damage);
                    hitExplosionParticlePool.ActiveParticle(ray.point);
                }
            }

            Recoil();
            Haptic(0.5f);
            ActiveMuzzleParticle();
            StartCoroutine(ShootCoolTime());
        }

        else
        {
            CPlayerSoundManager.Instance.PlaySoundOneShot(weapon.SoundEmptyShot);

            RaycastHit ray;

            if (Physics.Raycast(bulletTransform.position, bulletTransform.forward, out ray, float.MaxValue))
            {
                if (ray.transform.TryGetComponent<IHittable>(out IHittable hit))
                {
                    hit.Hit(weapon.Damage);
                    hitExplosionParticlePool.ActiveParticle(ray.point);
                }
            }

            Haptic(0.1f);
            Debug.LogFormat("���� �Ѿ� ������ ����!");
        }
    }

    /// <summary>
    /// �� �߻� ��Ÿ���� ��ٸ��� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator ShootCoolTime()
    {
        isFireReady = false;

        yield return new WaitForSeconds(weapon.ShootCoolTime);

        isFireReady = true;
    }

    /// <summary>
    /// ��Ʈ�ѷ��� ������Ű�� ���� �޼���
    /// </summary>
    public void Haptic(float duration)
    {
        if (leftController is not null)
        {
            Debug.Log("���� �� ���� �߻�!");
            leftController.SendHapticImpulse(0.8f, duration);
        }

        if (rightController is not null)
        {
            Debug.Log("������ �� ���� �߻�!");
            rightController.SendHapticImpulse(0.8f, duration);
        }
    }

    /// <summary>
    /// źâ�� �߰��Ѵ�.
    /// </summary>
    /// <param name="args">SelectEtnerEvent</param>
    public void AddAmmo(SelectEnterEventArgs args)
    {
        nowEquipAmmo = args.interactableObject.transform.GetComponent<CAmmo>();
        nowEquipAmmo.GetComponent<Rigidbody>().isKinematic = false;
    }

    /// <summary>
    /// źâ�� �����Ѵ�.
    /// </summary>
    /// <param name="args">SelectExitEvent</param>
    public void RemoveAmmo(SelectExitEventArgs args)
    {
        nowEquipAmmo = null;
    }

    /// <summary>
    /// ���� �����̰� Slide ���� �� ���� (������)
    /// </summary>
    public void Slide()
    {
        if (nowEquipAmmo is not null)
        {
            CPlayerSoundManager.Instance.PlaySoundOneShot(weapon.SoundReload);

            weapon.Reload(nowEquipAmmo.BulletNowCount);

            weaponUI.ChangeBulletCount(nowEquipAmmo.BulletNowCount);
            weaponUI.ChangeBulletUIColor((nowEquipAmmo.RemainBulletPercent >= 0.4f) ? Color.white : Color.red);
            Debug.LogFormat("���� �Ϸ� : {0}", nowEquipAmmo.BulletNowCount);
        }

        else
        {
            Debug.LogFormat("���� ����");
        }
    }

    /// <summary>
    /// Trigger�� �ѱ� �ݵ��� �ֱ����� Coroutine�� �����ϴ� �޼���
    /// </summary>
    public void Recoil()
    {
        Debug.Log("Recoil Work");
        StartCoroutine(RecoilStart());
    }

    /// <summary>
    /// �ѿ� �ݵ��� �ش�.
    /// </summary>
    /// <returns></returns>
    IEnumerator RecoilStart()
    {
        float fStartTime = 0.0f;
        while (fStartTime <= weapon.RecoilTime)
        {
            modelTransform.Translate(Vector3.forward * -3.0f * Time.deltaTime);
            modelTransform.Rotate(Vector3.right * -60.0f * Time.deltaTime);

            fStartTime += Time.deltaTime;

            yield return null;
        }

        fStartTime = 0.0f;
        while (fStartTime <= weapon.RecoilTime)
        {
            modelTransform.Translate(Vector3.forward * 3.0f * Time.deltaTime);
            modelTransform.Rotate(Vector3.right * 60.0f * Time.deltaTime);

            fStartTime += Time.deltaTime;

            yield return null;
        }

        modelTransform.SetLocalPositionAndRotation(modelStartPosition, Quaternion.identity);
    }

    /// <summary>
    /// Fire�� ����� ��ƼŬ�� Ű�� ���� �޼���
    /// </summary>
    public void ActiveMuzzleParticle()
    {
        oMuzzleParticle.SetActive(true);
        Invoke("InActiveMuzzleParticle", 0.1f);
    }

    /// <summary>
    /// Active�� Muzzle ��ƼŬ�� �ٽ� InActive ��Ű�� �޼���
    /// </summary>
    public void InActiveMuzzleParticle()
    {
        oMuzzleParticle.SetActive(false);
    }
}