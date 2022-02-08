using System;
using System.Reflection;
using UnityEngine;

namespace GoECS
{
    public class Entity : IEntity
	{
		int id;
		public Entity(int id)
		{
			this.id = id;
		}

		public int GetID()
		{
			return id;
		}

		public object GetComponent(Type t)
		{
			Debug.Log(111);
			return null;
		}



		public void Test<T>() where T : new()
		{
			/*
			var b = new T();
			var t = b.GetType();
			var mInfo = t.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public);

			Debug.Log(mInfo.Name);

			var t1 = new T1();
			t1.a = 233;

			var trans = GameObject.Find("Test").transform;			

			t.InvokeMember("Execute", BindingFlags.Default | BindingFlags.InvokeMethod, null, b, new object[] { trans, t1 });
			*/
		}
	}
}
