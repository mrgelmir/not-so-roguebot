using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	public Transform Target;

	public void LateUpdate()
	{
		if(Target != null)
		{
			// TODO smoothing and panning etc
			Vector3 newPos = Target.position;
			newPos.y = transform.position.y;
			transform.position = newPos;
        }
	}
}
