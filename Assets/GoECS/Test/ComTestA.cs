using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoECS.Test
{
	public class ComTestA : ComSingleton<ComTestA>
	{
		void Awake()
		{
			Debug.Log("ComTestA Awake.");
		}

		void Start()
		{
			Debug.Log("ComTestA Start.");
		}
	}
}
