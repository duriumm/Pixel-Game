using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ObjectPool usage example:

//public class EnemyAttack : MonoBehaviour
//{
//	public GameObject AttackParticleTemplate;
//	private static ObjectPool attackParticlePool;
//	void Awake()
//	{
//		if (attackParticlePool == null)
//			attackParticlePool = new ObjectPool(AttackParticleTemplate, 10);
//	}
//	...
//	spawnedObject = attackParticlePool.Spawn(pos);
//	attackParticlePool.Destroy(spawnedObject);

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
            GameObject.DontDestroyOnLoad(obj);
			pool.Add(obj);
		}
	}

	public GameObject Spawn(Vector3 pos)
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
		return null;
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
