using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GoECS
{
#if UNITY_EDITOR
	public class DisplayOnly : PropertyAttribute
	{

	}

	[CustomPropertyDrawer(typeof(DisplayOnly))]
	public class ReadOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
#endif

	public interface IGoEntityID
	{
		int CreateID();
	}

    public class GoEntity : MonoBehaviour, IEntity
	{
#if UNITY_EDITOR
		[DisplayOnly]
#endif
		[SerializeField]
		int id;
		public int GetID() { return id; }
		// public void SetID(int id) { this.id = id; }
		//
		
		void OnDestroy()
		{
			Debug.LogWarning($"{this.gameObject.name} Destroy !!! Dont Destroy GoEntity Without Using Ring Method [RemoveGoEntity].");
		}

		int ring = 0;
		bool isInit = false;
		public void Init(int ringIndex, int id)
		{
			if (isInit) return;
			ring = ringIndex;
			isInit = true;

			this.id = id;
		}

		public Component RmComponent(Type t)
		{
			var com = this.gameObject.GetComponent(t);
			if (com != null) 
			{
				GameObject.Destroy(com);
			} 

			return com;
		}

		public T RmComponent<T>() where T : UnityEngine.Component
		{
			var com = this.gameObject.GetComponent<T>();
			if (com != null) 
			{
				GameObject.Destroy(com);
			} 
			return com;
		}

		List<Type> comTps = new List<Type>();
		List<Type> comNoTps = new List<Type>();
		Dictionary<Type, UnityEngine.Component> components = new Dictionary<Type, Component>();
		UnityEngine.Component GetEComponent(Type t)
		{
			var j = comNoTps.IndexOf(t);
			if (j >= 0) return null;

			var i = comTps.IndexOf(t);
			if (i >= 0) return components[t];

			UnityEngine.Component com = null;
			if (!this.gameObject.TryGetComponent(t, out com)) 
			{
				this.comNoTps.Add(t);
				return null;
			}

			comTps.Add(t);
			components.Add(t, com);

			return com;
		}

		Dictionary<Type[], List<UnityEngine.Component>> collection = new Dictionary<Type[], List<UnityEngine.Component>>();
		[Obsolete]
		public UnityEngine.Component[] GetEComponents(in Type[] tps)
		{
			foreach(var c in collection)
			{
				if (tps.All(c.Key.Contains) && tps.Length == c.Key.Length) return c.Value.ToArray();
			}

			List<UnityEngine.Component> tmpComs = new List<Component>();
			tmpComs.Clear();
			for (int i = 0; i < tps.Length; i++)
			{
				UnityEngine.Component com;
				this.gameObject.TryGetComponent(tps[i], out com);
				if (com == null) return null;
				// Debug.Log($"[{entity.gameObject.name}] : {tps[i]} ==> {objs[i].GetType()}");

				tmpComs.Add(com);
			}

			if (tmpComs.Count > 0) collection.Add(tps, tmpComs);

			return collection[tps].ToArray();
		}

		[Obsolete]
		public void RemoveEComponent<T>() where T : UnityEngine.Component
		{
			var t = typeof(T);
			var delCom = this.gameObject.GetComponent<T>();  
			var list = new List<Type[]>();
			foreach(var c in collection)
			{
				if (c.Key.Contains(t))
				{
					list.Add(c.Key);
				}
			}
			foreach(var i in list) collection.Remove(i);

			Destroy(delCom);
		}

		[Obsolete]
		public T AddEComponent<T>() where T : UnityEngine.Component
		{
			return this.gameObject.AddComponent<T>();
		}

		public void GoName()
		{
			Debug.Log(gameObject.name);
		}
		

	}
}

