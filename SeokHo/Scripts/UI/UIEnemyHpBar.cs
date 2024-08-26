using UnityEngine;

public class UIEnemyHpBar : MonoBehaviour
{
    private RectTransform recttrHp; // �ڽ��� RectTransform ������ ����
    public Vector3 v3offset = Vector3.zero; // HpBar ��ġ ������
    public Transform trEnemy; // �� ĳ������ ��ġ

    void Start()
    {
        recttrHp = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (trEnemy != null)
        {
            // ���� ���� �����ǿ� �������� ���� ���� ����
            Vector3 worldPosition = trEnemy.position + v3offset;

            // RectTransform�� ��ġ�� ���� ���������� ����
            recttrHp.position = worldPosition;

            // HP �ٰ� ī�޶� ���ϵ��� ����
            transform.LookAt(Camera.main.transform);
        }
    }
}
