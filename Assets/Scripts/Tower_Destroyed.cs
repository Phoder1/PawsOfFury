using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Destroyed : MonoBehaviour
{
    [SerializeField]
    GameObject Tower_Destroyed_PF;

    public void CreatTowerDestroyed()
    {
        Instantiate(Tower_Destroyed_PF, transform.position, Quaternion.identity);
    }
}
