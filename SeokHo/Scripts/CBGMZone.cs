using System.Collections;
using UnityEngine;

public class CBGMZone : MonoBehaviour
{
    public AudioClip bgmClip; // ����� BGM Ŭ��
    private AudioSource audioSource;
    public float fadeDuration = 1.0f; // ���̵� ��/�ƿ� ���� �ð�

    private void Start()
    {
        // AudioSource ������Ʈ�� �߰��ϰ� ����
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bgmClip;
        audioSource.loop = true; // BGM �ݺ� ��� ����
        audioSource.volume = 0f; // �ʱ� ������ 0���� ����

        // �÷��̾ ���� �ȿ� �ִ��� Ȯ��
        Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // �÷��̾ ���� �ȿ� ������ BGM ���̵� ��
                StartCoroutine(FadeIn(audioSource, fadeDuration));
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾ ������ ������ BGM ���̵� ��
            if (!audioSource.isPlaying)
            {
                StartCoroutine(FadeIn(audioSource, fadeDuration));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �÷��̾ ������ ������ BGM ���̵� �ƿ�
            StartCoroutine(FadeOut(audioSource, fadeDuration));
        }
    }

    private IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float startVolume = 0f;
        audioSource.volume = startVolume;
        audioSource.Play();

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume += Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = 1.0f; // ������ �ִ�� ����
    }

    private IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // ���� ���̵� ���� ���� �ʱ� ���� ����
    }
}
