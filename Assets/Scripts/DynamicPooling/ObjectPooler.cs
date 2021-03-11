using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;

public class ObjectPooler : MonoSingleton<ObjectPooler>
{
    Dictionary<GameObject,ObjectPool> poolsDict;
    void CreatePool()
    {

    }
    public void CreateObject(GameObject gameObject)
    {

    }
    public void DestroyObject(GameObject gameObject)
    {

    }
}
public class ObjectPool
{
    GameObject obj;
    public ObjectPool(GameObject gameObject)
    {
        this.obj = gameObject;
    }
    public void CreateObject()
    {
    }
}
