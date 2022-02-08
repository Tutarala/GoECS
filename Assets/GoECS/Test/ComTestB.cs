using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoECS.Test
{
	public class ComTestB : ComSingleton<ComTestB>
	{
		void Awake()
		{
			Debug.Log("ComTestB Awake.");
		}
		void Start()
		{
			Debug.Log("ComTestB Start.");
		}

		public void PrintB()
		{
			Debug.Log("ComTestV Print.");
		}
	}
}
