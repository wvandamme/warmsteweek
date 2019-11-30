using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Thruster
{
	public class FlatScreen : Screen
	{
		[HideInInspector] public Rect viewport = new Rect(0, 0, 1, 1);
		public override string[] screensNames => new string[] { "Front" };
		public override Dictionary<string, FlatScreen> screens => new Dictionary<string, FlatScreen>() { { screensNames[0], this } };

#if UNITY_EDITOR
		[ExecuteInEditMode]
		void OnDrawGizmos()
		{
			var front = SceneView.currentDrawingSceneView == null ? true : Vector3.Dot(SceneView.currentDrawingSceneView.camera.transform.position - transform.position, transform.forward) < 0;

			Gizmos.color = front ? Color.green : Color.red;

			var size = transform.lossyScale / 2;
			var center = transform.position;
			var topLeft = center - transform.right * size.x + transform.up * size.y;
			var topRight = center + transform.right * size.x + transform.up * size.y;
			var bottomLeft = center - transform.right * size.x - transform.up * size.y;
			var bottomRight = center + transform.right * size.x - transform.up * size.y;

			Gizmos.DrawLine(topLeft, topRight);
			Gizmos.DrawLine(topRight, bottomRight);
			Gizmos.DrawLine(bottomRight, bottomLeft);
			Gizmos.DrawLine(bottomLeft, topLeft);

			if (front)
				return;

			Gizmos.DrawLine(bottomLeft, topRight);
			Gizmos.DrawLine(bottomRight, topLeft);
		}
#endif
	}
}