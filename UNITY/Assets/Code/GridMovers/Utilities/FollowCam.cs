using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
	public Transform Target;
	public Vector3 Offset = new Vector3(0f, 10f, 0f);

	protected void LateUpdate()
	{
		if(Target != null)
		{
			transform.position = Target.position + Offset;
		}
	}
	
}
