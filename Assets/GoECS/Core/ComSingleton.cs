using UnityEngine;
namespace GoECS {
    public class ComSingleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Get {
			get {
				return World.Get.GetSingleton<T>();
			} 
		}
	}

}
