using UnityEngine;

namespace GoECS
{
	public class AddToRing : MonoBehaviour
	{
		void Awake()
		{
			Ring.Get.AddGoEntity(this.gameObject);
		}
	}
}
