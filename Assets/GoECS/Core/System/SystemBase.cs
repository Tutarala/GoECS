using System;
using System.Reflection;
using UnityEngine;
namespace GoECS
{
    public class SystemBase 
    {

    }

	public interface ISystemOnUpdate
	{
        public void OnUpdate();
	}
}
