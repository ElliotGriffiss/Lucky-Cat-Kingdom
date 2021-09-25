using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform FollowTransform;
    [SerializeField] private float FollowSpeed;
	[Space]
	[SerializeField] private Vector3 CameraOffset;
	[SerializeField] private float MaxYValue;
	[SerializeField] private float MinYValue;

	private void Update()
	{
		Vector3 desiredPosition = FollowTransform.position + CameraOffset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, FollowSpeed);

		if (smoothedPosition.y > MaxYValue)
        {
			smoothedPosition.y = MaxYValue;
		}
		else if (smoothedPosition.y < MinYValue)
        {
			smoothedPosition.y = MinYValue;
		}

		transform.position = smoothedPosition;
	}

	public void SetCameraPositionImmediate()
	{
		Vector3 desiredPosition = FollowTransform.position + CameraOffset;
		transform.position = desiredPosition;
	}
}
