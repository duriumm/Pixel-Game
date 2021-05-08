using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
	public int AvailableItems { get; private set; }
	private List<GameObject> pool = new List<GameObject>();
	
	public ObjectPool(GameObject templateObject, int size)
	{
		AvailableItems = size;
		for (int i = 0; i < size; i++)
		{
			var obj = GameObject.Instantiate(templateObject);
			obj.SetActive(false);
            obj.hideFlags = HideFlags.HideInHierarchy;
            //GameObject.DontDestroyOnLoad(obj);
			pool.Add(obj);
		}
	}

	public GameObject Spawn(Vector2 pos)
	{
		//Find an inactive object
		foreach (var obj in pool)
		{
			if (!obj.activeInHierarchy)
			{
                obj.transform.position = pos;
                obj.SetActive(true);
                AvailableItems--;
                //Debug.Log("Spawn! " + AvailableItems);
                return obj;
			}
		}
		// No available object found, pick one anyway
        // Todo: return object which has been active the longest time
        return pool[0];
	}

	public void Destroy(GameObject obj)
	{
		if (obj == null)
			return;
		obj.SetActive(false);
		AvailableItems++;
		//Debug.Log("Destroy! " + AvailableItems);
	}
}
