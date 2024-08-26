using System.Collections;
using UnityEngine;

public class TriggerCanvasDisplay : MonoBehaviour
{
    public Canvas canvas; // ǥ���� Canvas ����
    public float displayDuration = 5.0f; // Canvas�� ǥ���� ���� �ð�
    public float fadeDuration = 1.0f; // ���̵� �ƿ� ȿ���� ���� �ð�

    private void Start()
    {
        if (canvas != null)
        {
            canvas.gameObject.SetActive(false); // �ʱ⿡�� Canvas�� ��Ȱ��ȭ
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canvas != null)
        {
            StartCoroutine(DisplayAndFadeCanvas()); // Player�� Ʈ���ſ� ������ �ڷ�ƾ ����
        }
    }

    private IEnumerator DisplayAndFadeCanvas()
    {
        canvas.gameObject.SetActive(true); // Canvas Ȱ��ȭ
        yield return new WaitForSeconds(displayDuration); // ������ �ð� ���� ���

        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>(); // CanvasGroup�� ������ �߰�
        }

        float elapsedTime = 0.0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration); // Canvas ���̵� �ƿ�
            yield return null;
        }

        canvas.gameObject.SetActive(false); // ���̵� �ƿ� �� Canvas ��Ȱ��ȭ
    }
}
