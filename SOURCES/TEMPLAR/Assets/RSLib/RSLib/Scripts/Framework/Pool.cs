namespace RSLib.Framework
{
	using System.Collections.Generic;
	using UnityEngine;

	public class Pool : Singleton<Pool>
	{
		[SerializeField] private PooledObject[] _pooledObjects = null;

		private Dictionary<int, Queue<GameObject>> _poolByGameObject = new Dictionary<int, Queue<GameObject>>();
		private Dictionary<string, Queue<GameObject>> _poolById = new Dictionary<string, Queue<GameObject>>();
		private Transform _poolTransform;

		/// <summary>Clears all the pooled objects.</summary>
		public static void Clear()
        {
			Instance._poolByGameObject.Clear();
			Instance._poolById.Clear();
        }

		/// <summary>Gets a pooled gameObject using a GameObject reference, and creates a new pool if none has been found.</summary>
		/// <param name="gameObject">Reference gameObject to find a pooled instance of.</param>
		/// <returns>Instance of the gameObject.</returns>
		public static GameObject Get(GameObject gameObject)
		{
			int poolKey = gameObject.GetInstanceID();

			if (!Instance._poolByGameObject.ContainsKey(poolKey))
			{
				Instance.LogWarning("Trying to get a pooled object that has not been pooled, creating new pool of 10 objects.");
				GenerateNewPool(new PooledObject(gameObject, 10));
			}

			GameObject result = Instance._poolByGameObject[poolKey].Dequeue();
			Instance._poolByGameObject[poolKey].Enqueue(result);
			result.SetActive(true);

			return result.gameObject;
		}

		/// <summary>Gets a pooled gameObject using an ID, and returns null if no pool has been found.</summary>
		/// <param name="id">Reference ID to find a pool.</param>
		/// <returns>Instance of a gameObject of the pool corresponding to the ID.</returns>
		public static GameObject Get(string id)
		{
			if (!Instance._poolById.ContainsKey(id))
			{
				Instance.LogError("Trying to get a pooled object with ID that has not been pooled.");
				return null;
			}

			GameObject result = Instance._poolById[id].Dequeue();
			Instance._poolById[id].Enqueue(result);
			result.SetActive(true);

			return result.gameObject;
		}

		/// <summary>Sends back a gameObject to the pool, setting the pool transform as its parent, and setting it inactive.</summary>
		/// <param name="gameObject">GameObject to send back to pool.</param>
		public void SendBackToPool(GameObject gameObject)
		{
			gameObject.transform.SetParent(_poolTransform);
			gameObject.SetActive(false);
		}

		/// <summary>Sends back a transform's gameObject to the pool, setting the pool transform as its parent, and setting it inactive.</summary>
		/// <param name="transform">Transform to send the gameObject back to pool.</param>
		public void SendBackToPool(Transform transform)
		{
			transform.SetParent(_poolTransform);
			transform.gameObject.SetActive(false);
		}

		protected override void Awake()
		{
			base.Awake();

			if (!IsValid)
				return;

			_poolTransform = transform;
			Initialize();
		}

		/// <summary>
		/// Creates a new pool using an instance of PooledObject, which is a class containing pooling datas for each pooled object.
		/// PooledObjects should be set in the inspector but they can be added later if needed.
		/// </summary>
		/// <param name="pooledObject">Pooled object to create a pool from.</param>
		public static void GenerateNewPool(PooledObject pooledObject)
		{
			UnityEngine.Assertions.Assert.IsNotNull(pooledObject.GameObject);
			UnityEngine.Assertions.Assert.IsTrue(pooledObject.Quantity > 0);

			if (Instance._poolByGameObject.ContainsKey(pooledObject.GameObject.GetInstanceID()))
			{
				Instance.LogWarning("Trying to create a pool of an object that has already been pooled.");
				return;
			}

			Queue<GameObject> newPool = new Queue<GameObject>(pooledObject.Quantity);

			for (int i = 0; i < pooledObject.Quantity; ++i)
			{
				GameObject newObject = Instantiate(pooledObject.GameObject, Instance._poolTransform);
				newObject.gameObject.SetActive(false);
				newPool.Enqueue(newObject);
			}

			Instance._poolByGameObject.Add(pooledObject.GameObject.GetInstanceID(), newPool);
			Instance._poolById.Add(pooledObject.Id, newPool);
		}

		/// <summary>Creates pools using the pooled objects setup beforehand.</summary>
		private void Initialize()
		{
			for (int pooledObjectIndex = _pooledObjects.Length - 1; pooledObjectIndex >= 0; --pooledObjectIndex)
				GenerateNewPool(_pooledObjects[pooledObjectIndex]);

			Log($"Initialized with {_pooledObjects.Length} objects.", gameObject);
		}

		[System.Serializable]
		public class PooledObject
		{
			[SerializeField] private string _id = "";
			[SerializeField] private GameObject _gameObject = null;
			[SerializeField] private int _quantity = 10;

			public PooledObject(GameObject gameObject, int quantity)
			{
				_gameObject = gameObject;
				_id = gameObject.name;
				_quantity = quantity;
			}

			public GameObject GameObject => _gameObject;

			public string Id => _id;

			public int Quantity => _quantity;
		}
	}
}