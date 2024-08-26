using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItemSpawn : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    public Transform spawnPoint;

    public void SpawnItem(int index)
    {
        if (index >= 0 && index < prefabsToSpawn.Length)
        {
            Instantiate(prefabsToSpawn[index], spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogError("�ε����� Prefab �迭�� ������ ������ϴ�.");
        }
    }
}
