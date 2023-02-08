using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoECS
{
	public class World : MonoBehaviour
	{
		static World _world = null;
		public static World Get
		{
			get {
				if (_world == null)
				{
					var obj = new GameObject("GoECS World");
					DontDestroyOnLoad(obj);
					_world = obj.AddComponent<World>();
				}
				return _world;
			} 
		}

		public static T Com<T>()
		{
            if (_world == null || _world.gameObject == null) return default(T);
			return _world.gameObject.GetComponent<T>();
		}

		public readonly int ringMax = 10;
		Ring[] rings = new Ring[10];
		void Awake()
		{
			// Create Ring0	
			AddRing(0);
		}

		public Ring Ring(int index = 0)
		{
			if (index > 0 && index <= ringMax) return _world.rings[index];
			else return Ring0();
		}

		public Ring Ring0()
		{
			if (_world.rings[0] == null) AddRing(0);
			return _world.rings[0];
		}

		public T AddSingleton<T>() where T : UnityEngine.Component
		{
			T com;
			if (this.gameObject.TryGetComponent<T>(out com)) return default(T);
			return this.gameObject.AddComponent<T>();
		}

		public T GetSingleton<T>() where T : UnityEngine.Component
		{
			return this.gameObject.GetComponent<T>();
		}

		public void Execute<T>(Action<T> t)
		{
			t(this.GetComponent<T>());
		}

		public void Execute<T1, T2>(Action<T1, T2> t) where T1 : UnityEngine.Component where T2 : UnityEngine.Component
		{
			t(this.GetComponent<T1>(), this.GetComponent<T2>());
		}

		public void Execute<T1, T2, T3>(Action<T1, T2, T3> t) where T1 : UnityEngine.Component where T2 : UnityEngine.Component where T3 : UnityEngine.Component
		{
			t(this.GetComponent<T1>(), this.GetComponent<T2>(), this.GetComponent<T3>());
		}
		
		public void AddRing(int index)
		{
			if (index > ringMax) return;
			rings[index] = CreateRing(index);
		}

		public Ring CreateRing(int ringIndex)
		{
			var obj = new GameObject($"GoECS Ring{ringIndex}");
			var com = obj.AddComponent<Ring>();
			com.SetIndex(ringIndex);
			return com;
		}

		public void RemoveRing(int index)
		{
			if (index == 0)
			{
				Debug.Log("Cant Remove Ring0 !!!");
				return;	
			} else if (index < 0) return;

			var ring = rings[index];
			ring.Destroy();
			GameObject.Destroy(ring.gameObject);
		}
	}
}

