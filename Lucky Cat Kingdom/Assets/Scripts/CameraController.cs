using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform FollowTransform;
    [SerializeField] private float FollowSpeed;
	[Space]
	[SerializeField] private Vector3 CameraOffset;

	private void Update()
	{
		Vector3 desiredPosition = FollowTransform.position + CameraOffset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, FollowSpeed);
		transform.position = smoothedPosition;
	}
	public void TeleportCameraToTarget()
	{
		Vector3 desiredPosition = FollowTransform.position + CameraOffset;
		transform.position = desiredPosition;
	}
}
