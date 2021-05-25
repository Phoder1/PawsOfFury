using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCount : MonoBehaviour
{
    [SerializeField] GameObject Prefab; 
    [SerializeField] int DefaultAmount; 

    public void Spawn() 
    {
        Spawn(DefaultAmount);
    }

    public void Spawn(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(Prefab, transform.position, Quaternion.identity);
        }
    }
}
