using UnityEngine;

namespace GoECS.Test
{
    public class TestSystemOnUpdate : SystemBase, ISystemOnUpdate
	{
		float speed = 0.1f;

		public void OnUpdate()
		{
			Ring.Get.Execute((Transform trans, BoxCollider box) => {
				trans.transform.Rotate(new Vector3(speed, 0, 0), Space.Self);
				if (box == null) Debug.Log("Box is None!!!");
				else box.isTrigger = true;
			});
		}
	}


    public class TestSystemOnUpdateA : SystemBase, ISystemOnUpdate
	{
		float speed = 0.2f;

		public void OnUpdate()
		{
			Ring.Get.Execute((TestScaleTag tag, Transform trans, BoxCollider box) =>{
				trans.transform.Rotate(new Vector3(speed, 0, 0), Space.Self);
				if (box == null) Debug.Log("Box is None!!!");
				else box.isTrigger = true;
			});
		}
	}

    public class TestSystemOnUpdateB : SystemBase, ISystemOnUpdate
	{
		float speed = 0.1f;

		public void OnUpdate()
		{
			Ring.Get.Execute((TestHitTag tag, Transform trans, BoxCollider box) =>{
				trans.transform.Rotate(new Vector3(speed, 0, 0), Space.Self);
				if (box == null) Debug.Log("Box is None!!!");
				else box.isTrigger = true;
			});
		}
	}


	public class TestSystemOnOnce : SystemBase, ISystemOnUpdate
	{
		float scale = 0.1f;

		public void OnUpdate()
		{
			Ring.Get.Execute((TestScaleTag tag, Transform trans) => {
				trans.transform.localScale = new Vector3(1, scale, 1);
			});
		}
	}


	public class TestSystemMainCamera : SystemBase, ISystemOnUpdate
	{
		float scale = 0.1f;

		public void OnUpdate()
		{
			Ring.Get.Execute((GoEntity entity, Camera mainC, Transform trans) => {
				if (UnityEngine.Camera.main != mainC) return;
				trans.transform.localScale = new Vector3(1, scale, 1);
			});
		}
	}

	public class TestSystemTouchInput : SystemBase, ISystemOnUpdate
	{
		void click(Vector2 pos)
		{
			Ray ray = Camera.main.ScreenPointToRay(pos);

			var hits = Physics.RaycastAll(ray);
			foreach(var hit in hits)
			{
				Debug.Log(hit.transform.gameObject.name);
				Ring.Get.Execute((GoEntity entity, TestHitTag t) => {
					Ring.Get.RemoveComponentSafe<TestHitTag>(entity);
				});
				Ring.Get.AddGoComponent<TestHitTag>(hit.transform.gameObject);
			}
		}
        void handleTouch()
        {
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				this.click(Input.GetTouch(0).position);
			}

			if (Input.GetMouseButtonUp(0))
			{
				this.click(Input.mousePosition);
			}
        }

		public void OnUpdate()
		{
			this.handleTouch();
		}
	}
}

