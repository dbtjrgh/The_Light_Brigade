using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class CPlayerController : MonoBehaviour
{
    #region private ����
    [SerializeField]
    ActionBasedController leftController;
    [SerializeField]
    GameObject oTunnelingVignette;
    [SerializeField]
    AudioClip hitSound;

    CPlayerStats playerStats;
    UIWeapon weaponUI;
    UIPlayerStats playerStatsUI;

    bool isInitModel;
    #endregion

    public UIPlayerStats PlayerStatsUI
    {
        get
        {
            if (playerStatsUI is null)
            {
                return null;
            }

            else
            {
                return playerStatsUI;
            }
        }
    }

    void Start()
    {
        playerStats = GetComponent<CPlayerStats>();

        isInitModel = false;
        Debug.Log(playerStats.HP);
    }

    void LateUpdate()
    {
        InitModel();
    }

    /// <summary>
    /// Left Controller�� �ִ� Model�� UI�� �Ҵ�޴´�.
    /// </summary>
    void InitModel()
    {
        if (isInitModel)
        {
            return;
        }

        if (leftController.model != null)
        {
            playerStatsUI = leftController.model.GetComponentInChildren<UIPlayerStats>();
            playerStatsUI.gameObject.SetActive(false);
            playerStatsUI.ChangeLifeCount(playerStats.Life, playerStats.MaxLife);
            playerStatsUI.ChangeHPText(playerStats.HP, playerStats.MaxHP);
            playerStatsUI.ChangeSoulText(playerStats.Soul);
            playerStatsUI.ChangeAmmoText(playerStats.Ammo);

            GetComponent<CharacterController>().height = 1.5f;

            isInitModel = true;
        }
    }

    /// <summary>
    /// �÷��̾� �ǰ�
    /// </summary>
    public void Hit(float damage)
    {
        CPlayerSoundManager.Instance.PlaySoundOneShot(hitSound);

        Debug.Log(playerStats.HP);
        playerStats.ChangeHP(playerStats.HP - (int)damage);
        playerStatsUI.ChangeHPText(playerStats.HP, playerStats.MaxHP);
        Debug.Log(damage);
        Debug.Log(playerStats.MaxHP);

        oTunnelingVignette.SetActive(true);
        if (playerStats.HP > 4)
        {
            Invoke("InActiveTunnelingVignette", 0.2f);
        }

        else
        {
            StopCoroutine("HealCooltime");
            StartCoroutine("HealCooltime");
        }


        if (playerStats.HP <= 0)
        {
            PlayerDie();
        }


        if (weaponUI is null)
        {
            return;
        }

        else
        {
            weaponUI.ChangeHPCount(playerStats.HP);
            weaponUI.ChangeHPUIColor((playerStats.HP >= 4) ? Color.white : Color.red);
        }
    }

    /// <summary>
    /// �÷��̾� ü�� ȸ��
    /// </summary>
    /// <param name="healAmount">ü�� ȸ����</param>
    public void Heal(int healAmount)
    {
        playerStats.ChangeHP(playerStats.HP + healAmount);
        playerStatsUI.ChangeHPText(playerStats.HP, playerStats.MaxHP);

        if (playerStats.HP > 4 && oTunnelingVignette.activeSelf)
        {
            Invoke("InActiveTunnelingVignette", 0.2f);
        }

        if (weaponUI is null)
        {
            return;
        }

        else
        {
            weaponUI.ChangeHPCount(playerStats.HP);
            weaponUI.ChangeHPUIColor((playerStats.HP >= 4) ? Color.white : Color.red);
        }
    }

    /// <summary>
    /// �÷��̾� �ִ� ü���� ������Ų��.
    /// </summary>
    /// <param name="amount">������ų ü�·�</param>
    public void IncreaseMaxHP(int amount)
    {
        playerStats.ChangeMaxHP(playerStats.MaxHP + amount);
        playerStats.ChangeHP(playerStats.HP + amount);
        PlayerStatsUI.ChangeHPText(playerStats.HP, playerStats.MaxHP);

        if (weaponUI is null)
        {
            return;
        }

        else
        {
            weaponUI.ChangeHPCount(playerStats.HP);
            weaponUI.ChangeHPUIColor((playerStats.HP >= 4) ? Color.white : Color.red);
        }
    }

    /// <summary>
    /// �÷��̾� �ҿ� �������� ������Ű��, UI Text�� �ٲٴ� �޼���
    /// </summary>
    /// <param name="soul">�ҿ� ȹ�淮</param>
    public void AddSoul(int soul)
    {
        playerStats.ChangeSoul(playerStats.Soul + soul);
        playerStatsUI.ChangeSoulText(playerStats.Soul);
    }

    /// <summary>
    /// WeaponUI�� ����Ѵ�.
    /// </summary>
    public void SetWeaponUI(UIWeapon ui)
    {
        if (ui is null)
        {
            weaponUI = null;
            return;
        }

        weaponUI = ui;
        weaponUI.gameObject.SetActive(true);

        weaponUI.ChangeHPCount(playerStats.HP);
        weaponUI.ChangeHPUIColor((playerStats.HP >= 4) ? Color.white : Color.red);
    }

    /// <summary>
    /// �÷��̾� �ǰݽ� ȭ���� �������� ȿ���� ��Ȱ��ȭ �Ѵ�.
    /// </summary>
    void InActiveTunnelingVignette()
    {
        oTunnelingVignette.SetActive(false);
    }

    /// <summary>
    /// �÷��̾� ü���� 5�̸� �� ��, ���� �ð��� ��ٸ� �� �ٽ� ü���� 5���� ȸ���ϴ� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    IEnumerator HealCooltime()
    {
        yield return new WaitForSeconds(10.0f);

        if (playerStats.HP < 5)
        {
            Heal(5 - playerStats.HP);
        }
    }

    /// <summary>
    /// �÷��̾ �׾��� �� ����Ǵ� �޼���
    /// </summary>
    void PlayerDie()
    {
        playerStats.DecreaseLife();
        playerStatsUI.ChangeLifeCount(playerStats.Life, playerStats.MaxLife);

        playerStats.ChangeHP(playerStats.MaxHP);
        Heal(playerStats.MaxHP);
        PlayerStatsUI.ChangeHPText(playerStats.HP, playerStats.MaxHP);

        if (playerStats.Life > 0)
        {
            SceneLoadManager.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        else
        {
            SceneLoadManager.Instance.LoadScene(0);
        }
    }
}
