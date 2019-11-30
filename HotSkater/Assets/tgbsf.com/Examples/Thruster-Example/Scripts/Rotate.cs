using UnityEngine;

public class Rotate : MonoBehaviour
{
	public Vector3 speed = Vector3.one * 30.0f;

	void Update()
	{
		transform.Rotate(speed * Time.deltaTime);
	}
}
