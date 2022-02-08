using UnityEngine;
using System.Collections;

namespace GoECS.Test
{
    public class Test : MonoBehaviour
	{
		public GameObject testCube;
		public GameObject testCube1;
		void Start()
		{
			/*
			var en = Ring.Instance().AddGoEntity(GameObject.Instantiate(this.testCube));
			var sys = new TestSystemOnUpdateA();
			en.GetEComponents(sys.GetTypes());
			en.GetEComponents(sys.GetTypes());
			en.GetEComponents(sys.GetTypes());
			en.GetEComponents(sys.GetTypes());
			return;
			*/
			World.Get.AddSingleton<ComTestA>();
			World.Get.AddSingleton<ComTestB>();

			ComTestB.Get.PrintB();
			int count = 0;
			for (int i = 0; i < count; i++)
			{
				var go = GameObject.Instantiate(this.testCube);
				if (i % 2 == 0) go.AddComponent<TestScaleTag>();
				else if (i % 1 == 0) go.AddComponent<TestHitTag>();

				Ring.Get.AddGoEntity(go);
				go.transform.position = new Vector3(UnityEngine.Random.Range(0, 200), UnityEngine.Random.Range(0, 200), UnityEngine.Random.Range(0, 200));
			}

			Ring.Get.AddGoEntity(this.testCube1);
			// StartCoroutine(this.TestDestroy());

			Ring.Get.AddSystem<TestSystemOnUpdateB>();
			Ring.Get.AddSystem<TestSystemOnUpdateA>();

			Ring.Get.Execute((TestScaleTag tag, Transform trans) => {
				trans.transform.localScale = new Vector3(1, 0.1f, 1);
			});

			Ring.Get.AddSystem<TestSystemTouchInput>();
			Ring.Get.AddSystem<TestSystemMainCamera>();
			
		}

		IEnumerator TestDestroy()
		{
			yield return new WaitForSeconds(1);
			Debug.Log("Destroy testCube1");
			Destroy(this.testCube1);
		}

		void Update()
		{

		}
	}
}

