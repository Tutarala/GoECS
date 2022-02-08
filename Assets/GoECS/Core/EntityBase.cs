using System;
using System.Reflection;

namespace GoECS
{
	public interface IEntity
	{
		// public object GetComponent(Type t);
		public int GetID();
	}
}

