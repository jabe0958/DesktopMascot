using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Main : MonoBehaviour {

	[DllImport ("UnityTransparentizeBackgroundPlugin")]
	private static extern void TransparentizeBackground ();

	[RuntimeInitializeOnLoadMethod]
	static void Initialize() {
		#if UNITY_EDITOR
			// None
		#else
			TransparentizeBackground ();
		#endif
	}

}
