using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using System.Linq;

namespace GoECS
{
	public class Ring : MonoBehaviour
	{
		// temp code
		[Obsolete]
		public static Ring Instance()
		{
			return World.Get.Ring0();
		}

		public static Ring Get {
			get 
			{
				return World.Get.Ring0();
			}
		}

		List<GoEntity> list = new List<GoEntity>();
		List<SystemBase> systems = new List<SystemBase>();
		List<SystemBase> singletonSystems = new List<SystemBase>();

		List<ISystemOnUpdate> updateSystems = new List<ISystemOnUpdate>();

		void Update()
		{
			foreach (var sys in updateSystems)
			{
				(sys as ISystemOnUpdate).OnUpdate();
			}

		}

		int index = 0;
		public void SetIndex(int i)
		{
			index  = i;
		}

		public GoEntity AddGoEntity(GameObject go = null)
		{
			var obj = go;
			if (obj == null) 
			{
				obj = new GameObject();
			}

			var entity = obj.AddComponent<GoEntity>();
			this.list.Add(entity);


			foreach(var c in collection)
			{
				if (this.GetComponents(entity, c.Key))
				{
					c.Value.Add(entity);
				}
			}

			return entity;
		}

		public T AddSystem<T>(bool isSingleton = false) where T : SystemBase, new()
		{
			if (isSingleton && CheckSingletonSystem(typeof(T))) return null;

			var sys = new T();

			if (!isSingleton)
			{
				this.systems.Add(sys);
				// Add to Update List Do It Every Frame
				if (sys is ISystemOnUpdate) this.updateSystems.Add(sys as ISystemOnUpdate);
			} else
			{
				// Add to Update List Do It Every Frame
				if (sys is ISystemOnUpdate) 
				{
					this.singletonSystems.Add(sys);
					this.updateSystems.Add(sys as ISystemOnUpdate);
				}
			}

			return sys;
		}

		public bool CheckSingletonSystem(Type sysType)
		{
			foreach (var i in this.singletonSystems)
			{
				if (i.GetType().Equals(sysType)) return true;
			}

			return false;
		}

		public GoEntity[] GetGoEntities()
		{
			return this.list.ToArray();
		}

		public bool GetGoComponents(GoEntity entity, in Type[] tps, ref Component[] objs)
		{
			for (int i = 0; i < tps.Length; i++)
			{
				if (!entity.gameObject.TryGetComponent(tps[i], out objs[i])) return false;
				// Debug.Log($"[{entity.gameObject.name}] : {tps[i]} ==> {objs[i].GetType()}");
			}

			return true;
		}

		public bool GetComponents(GoEntity entity, in Type[] tps)
		{
			for (int i = 0; i < tps.Length; i++)
			{
				if (!entity.gameObject.GetComponent(tps[i])) return false;
				// Debug.Log($"[{entity.gameObject.name}] : {tps[i]} ==> {objs[i].GetType()}");
			}

			return true;
		}

		public void ExecuteSystem(SystemBase sys)
		{
		}

		void PrintSysFailedMatchedEntity(string sysName, GoEntity entity, Type[] types, ref object[] rets)
		{
			for (int index = 0; index < types.Length; index++)
			{
				if (rets[index] == null)		
				{
					Debug.Log($"**{sysName}** Cant Matched : {entity.gameObject.name} Missing ==> {types[index]}");
					break;
				}
			}
		}

		Dictionary<Type[], List<GoEntity>> collection = new Dictionary<Type[], List<GoEntity>>();
		List<GoEntity> GetGoEntities(Type[] types)
		{
			foreach(var c in collection)
			{
				if (types.All(c.Key.Contains) && types.Length == c.Key.Length) return c.Value;
			}

			List<GoEntity> entities = new List<GoEntity>();
			UnityEngine.Component[] sysParams = new UnityEngine.Component[types.Length];
			foreach(var entity in GetGoEntities())
			{
				if (!this.GetGoComponents(entity, types, ref sysParams)) continue;
				entities.Add(entity);
			}

			collection.Add(types, entities);
			return entities;
		}

		Dictionary<Type, List<GoEntity>> collection1 = new Dictionary<Type, List<GoEntity>>();
		List<GoEntity> GetGoEntities<T>()
		{
			return GetGoEntities(typeof(T));
		}

		List<GoEntity> GetGoEntities(Type type)
		{
			if (collection1.ContainsKey(type)) return collection1[type];
			List<GoEntity> entities = new List<GoEntity>();
			foreach(var entity in GetGoEntities())
			{

				if (!entity.gameObject.GetComponent(type)) continue;
				entities.Add(entity);
			}

			collection1.Add(type, entities);
			return entities;
		}


		public static bool compareTypes(Type[] arr1, Type[] arr2)
		{
			var q = from a in arr1 join b in arr2 on a equals b select a;
			bool flag = arr1.Length == arr2.Length && q.Count() == arr1.Length;
			return flag;//内容相同返回true,反之返回false。
		}

		// List<UnityEngine.Component> sysParams = new List<UnityEngine.Component>();
		public GoEntity GetGoEntity(GameObject go)
		{
			return go.GetComponent<GoEntity>();
		}

		public void AddGoComponent<T>(GameObject go) where T : UnityEngine.Component
		{
			var entity = GetGoEntity(go);
			if (entity == null) Debug.LogWarning($"Cant Add {typeof(T)} To A GameObject Without Become A GoEntity !!!");
			AddGoComponent<T>(entity);
		}

		public T AddGoComponent<T>(GoEntity entity) where T : UnityEngine.Component
		{
			UnityEngine.Component com = null;
			if (!entity.gameObject.TryGetComponent(typeof(T), out com))
			{
				com = entity.gameObject.AddComponent<T>();
				UpdateCollection<T>(entity);
			}
			return com as T;
		}

		void UpdateCollection<T>(GoEntity entity) where T : UnityEngine.Component
		{
			var t = typeof(T);
			foreach(var c in collection)
			{
				if (c.Key.Contains(t) && this.GetComponents(entity, c.Key))
				{
					c.Value.Add(entity);
				}
			}
			
			foreach(var c in collection1)
			{
				if (c.Key == t && entity.gameObject.GetComponent(t))
				{
					c.Value.Add(entity);
				}
			}
		}

		public void RemoveComponentSafe<T>(GoEntity entity) where T : UnityEngine.Component
		{
			var t = typeof(T);
			StartCoroutine(this.RemoveComponent(t, entity));
		}

		public void RemoveComponent<T>(GoEntity entity) where T : UnityEngine.Component
		{
			var t = typeof(T);
			Destroy(entity.gameObject.GetComponent<T>());
			this.RemoveComponentInC(t, entity);
		}

		IEnumerator RemoveComponent(Type t, GoEntity entity)
		{
			yield return new WaitForEndOfFrame();
			Destroy(entity.gameObject.GetComponent(t));
			this.RemoveComponentInC(t, entity);
		}


		void RemoveComponentInC(Type t, GoEntity entity)
		{
			foreach(var c in collection)
			{
				if (c.Key.Contains(t))
				{
					c.Value.Remove(entity);
				}
			}
			
			foreach(var c in collection1)
			{
				if (c.Key == t)
				{
					c.Value.Remove(entity);
				}
			}
		}

		void RemoveGoEntityAtList(List<GoEntity> l, GoEntity entity)
		{
			int lastIndex = l.Count - 1;
			int index = l.IndexOf(entity);
			if (index < 0) return;

			l[index] = l[lastIndex];
			l.RemoveAt(lastIndex);
		}

		public void RemoveGoEntity(GoEntity entity, bool destroy = false)
		{
			foreach (var c in collection)
			{
				RemoveGoEntityAtList(c.Value, entity);
			}

			foreach (var c in collection1)
			{
				RemoveGoEntityAtList(c.Value, entity);
			}

			RemoveGoEntityAtList(list, entity);
			if (destroy) GameObject.Destroy(entity.gameObject);
		}

		public GoEntity GetEntity(int id)
		{
			foreach (var entity in GetGoEntities())
			{
				if (entity.GetID() == id) return entity;
			}
			return null;
		}	

		public T GetSingleton<T>()
		{
			var entities = GetGoEntities(typeof(T));
			return entities.Count == 0 ? default(T) : entities[0].GetComponent<T>();
		}

		public void Execute<T>(Action<T> t)
		{
			var entities = GetGoEntities(typeof(T));
			foreach(var entity in entities)
			{
				t(entity.GetComponent<T>());
			}
		}

		public void Execute<T1, T2>(Action<T1, T2> t) where T1 : UnityEngine.Component where T2 : UnityEngine.Component
		{
			var types = new Type[2] { typeof(T1), typeof(T2) };
			var entities = GetGoEntities(types);
			foreach(var entity in entities)
			{
				t(entity.GetComponent<T1>(), entity.GetComponent<T2>());
			}
		}

		public void Execute<T1, T2, T3>(Action<T1, T2, T3> t) where T1 : UnityEngine.Component where T2 : UnityEngine.Component where T3 : UnityEngine.Component
		{
			var types = new Type[3] { typeof(T1), typeof(T2), typeof(T3) };
			var entities = GetGoEntities(types);
			foreach(var entity in entities)
			{
				t(entity.GetComponent<T1>(), entity.GetComponent<T2>(), entity.GetComponent<T3>());
			}
		}

		public static void DumpTypes(Type[] types)
		{
			string str = "";
			foreach(var t in types)
			{
				str += t + " | ";
			}
			Debug.Log(str);
		}

		public void Destroy()
		{
			foreach (var entity in list)
			{
				GameObject.Destroy(entity.gameObject);
			}

			list.Clear();
			collection.Clear();
			collection1.Clear();
		}

		/*
		public bool RemoveComponent<T>() where T : UnityEngine.Component
		{
			foreach (var entity in GetGoEntities())
			{
				if (entity.RmComponent(typeof(T)) != null) 
				{
					return true;
				}
			}
			return false;
		}
		*/
	}
}

