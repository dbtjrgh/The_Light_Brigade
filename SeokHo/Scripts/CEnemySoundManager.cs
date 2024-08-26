using UnityEngine;

public class CEnemySoundManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static CEnemySoundManager Instance { get; private set; }

    // ����� �ҽ� ������ (AudioSource�� ���Ե� ������)
    public AudioSource audioSourcePrefab;
    // ���� ���� Ŭ�� �迭
    public AudioClip[] enemySounds;

    public AudioClip[] BossSounds;

   

    private void Awake()
    {
        // audioSourcePrefab = GetComponent<AudioSource>();
        // �ν��Ͻ��� �������� �ʴ� ���, ���� �ν��Ͻ��� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ���� �� ������Ʈ�� �ı����� ����
        }
        else
        {
            // �ߺ� �ν��Ͻ��� �ı�
            Destroy(gameObject);
        }
    }

    // ���� ���带 ����ϴ� �޼ҵ�
    public void PlayEnemySound(int soundIndex, Vector3 position)
    {
        // ���� �ε����� �迭 ������ ����� �ʵ��� Ȯ��
        if (soundIndex < 0 || soundIndex >= enemySounds.Length)
        {
            Debug.LogWarning("���� �ε����� ������ ������ϴ�");
            return;
        }

        // ���ο� ����� �ҽ� ������ �ν��Ͻ� ����
        AudioSource audioSource = Instantiate(audioSourcePrefab, position, Quaternion.identity);
        audioSource.clip = enemySounds[soundIndex]; // ���� Ŭ�� �Ҵ�
        audioSource.spatialBlend = 1.0f; // 3D ����� ����
        audioSource.Play(); // ���� ���

        // ���� ��� �� ����� �ҽ� ������Ʈ�� �ɼǿ� ���� �ı�
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    // ���� ���带 ����ϴ� �޼ҵ�
    public void PlayBossSound(int soundIndex, Vector3 position)
    {
        // ���� �ε����� �迭 ������ ����� �ʵ��� Ȯ��
        if (soundIndex < 0 || soundIndex >= BossSounds.Length)
        {
            Debug.LogWarning("���� �ε����� ������ ������ϴ�");
            return;
        }

        // ���ο� ����� �ҽ� ������ �ν��Ͻ� ����
        AudioSource audioSource = Instantiate(audioSourcePrefab, position, Quaternion.identity);
        audioSource.clip = BossSounds[soundIndex]; // ���� Ŭ�� �Ҵ�
        audioSource.spatialBlend = 1.0f; // 3D ����� ����
        audioSource.Play(); // ���� ���

        // ���� ��� �� ����� �ҽ� ������Ʈ�� �ɼǿ� ���� �ı�
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
