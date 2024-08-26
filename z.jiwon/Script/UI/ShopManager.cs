using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public Button item1Button;
    public Button item2Button;
    public Button item3Button;

    public int item1Cost;
    public int item2Cost;
    public int item3Cost;

    public TMP_Text item1PriceText; // ������ 1 ���� ǥ�ÿ� TextMeshPro �ؽ�Ʈ
    public TMP_Text item2PriceText; // ������ 2 ���� ǥ�ÿ� TextMeshPro �ؽ�Ʈ
    public TMP_Text item3PriceText; // ������ 3 ���� ǥ�ÿ� TextMeshPro �ؽ�Ʈ
    public TMP_Text playerSoulText; // �÷��̾��� Soul ���� ǥ���� TextMeshPro �ؽ�Ʈ

    private CPlayerStats playerStats;

    void Start()
    {
        playerStats = FindObjectOfType<CPlayerStats>(); // �÷��̾� ���� ������Ʈ ã��
        UpdateButtonStates();
        UpdatePriceTexts();
        UpdateSoulText();
    }

    void UpdateButtonStates()
    {
        // �� ������ ��뿡 ���� ��ư Ȱ��ȭ ���� ����
        item1Button.interactable = playerStats.Soul >= item1Cost;
        item2Button.interactable = playerStats.Soul >= item2Cost;
        item3Button.interactable = playerStats.Soul >= item3Cost;
    }

    void UpdatePriceTexts()
    {
        item1PriceText.text = "Cost: " + item1Cost.ToString(); // ������ 1 ���� ������Ʈ
        item2PriceText.text = "Cost: " + item2Cost.ToString(); // ������ 2 ���� ������Ʈ
        item3PriceText.text = "Cost: " + item3Cost.ToString(); // ������ 3 ���� ������Ʈ
    }

    void UpdateSoulText()
    {
        playerSoulText.text = "Current Soul: " + playerStats.Soul.ToString(); // ���� Soul �� ������Ʈ
    }
}