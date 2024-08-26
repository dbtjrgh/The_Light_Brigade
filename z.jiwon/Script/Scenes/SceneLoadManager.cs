using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    private static SceneLoadManager instance;
    public static SceneLoadManager Instance
    {
        get { return instance; }
    }

    public GameObject player;
    public Dictionary<int, Transform> spawnPoints = new Dictionary<int, Transform>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0 || scene.buildIndex == 6)
        {
            return;
        }

        string spawnPointName = "";
        switch (scene.buildIndex)
        {
            case 1:
                spawnPointName = "spawnPointTp";
                break;
            case 2:
                spawnPointName = "spawnPointM1";
                break;
            case 3:
                spawnPointName = "spawnPointM2";
                break;
            case 4:
                spawnPointName = "spawnPointSt";
                break;
            case 5:
                spawnPointName = "spawnPointBs";
                break;
        }

        GameObject spawnPointGO = GameObject.Find(spawnPointName);
        if (spawnPointGO != null)
        {
            spawnPoints[scene.buildIndex] = spawnPointGO.transform;
            UpdatePlayerPosition(scene.buildIndex);
        }
        else
        {
            Debug.LogError("Spawn point not found for scene " + scene.buildIndex);
        }
    }

    public void LoadScene(int index)
    {
        StartCoroutine(LoadScenes(index));
    }

    private IEnumerator LoadScenes(int index)
    {
        // 'LoadingScene'�� ���� ������� �ε�
        SceneManager.LoadScene("LoadingScene");
        yield return null;  // �� ������ ���

        // ���� ���� �񵿱� ������� �ε�
        //AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(index);
        SceneManager.LoadScene(index);
        //yield return new WaitUntil(() => sceneLoad.isDone);

        // �ε� ���� �ε�� ��, ��ε� �õ� ���� �ε� ���¸� üũ
        //Scene loadingScene = SceneManager.GetSceneByName("LoadingScene");
        //if (loadingScene.isLoaded)
        //{
        //    AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync("LoadingScene");
        //    yield return new WaitUntil(() => unloadOperation.isDone);
        //}
        //else
        //{
        //    Debug.LogError("Loading scene is not loaded and cannot be unloaded.");
        //}
    }

    private void UpdatePlayerPosition(int sceneIndex)
    {
        if (player != null && spawnPoints.ContainsKey(sceneIndex))
        {
            player.transform.position = spawnPoints[sceneIndex].position;
        }
    }
}