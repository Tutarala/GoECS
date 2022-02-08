using UnityEngine;

namespace GoECS
{
	public class AddToRing : MonoBehaviour
	{
		void Awake()
		{
			Ring.Instance().AddGoEntity(this.gameObject);
		}
	}
}
