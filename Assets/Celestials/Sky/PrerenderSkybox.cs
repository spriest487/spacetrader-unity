using UnityEngine;
using System.Collections;

public class PrerenderSkybox : MonoBehaviour {
	void OnPreRender()
	{
		GL.ClearWithSkybox(true, GetComponent<Camera>());
	}

	void OnPreCull()
	{
		var starfield = GetComponentInChildren(typeof(Starfield));
		if (starfield)
		{
			starfield.transform.rotation = Quaternion.identity;
			starfield.transform.rotation = Quaternion.identity;
		}
	}
}
