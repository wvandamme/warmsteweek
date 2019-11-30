using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Rhinox.Utilities.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Transform), true)]
	public class TransformCompEditor : UnityEditor.Editor
	{
		Vector3 worldPos;
		SerializedProperty mPos;
		SerializedProperty mRot;
		SerializedProperty mScale;

		private static Vector3? positionClipboard = null;
		private static Quaternion? rotationClipboard = null;
		private static Vector3? scaleClipboard = null;

		private const int _buttonHeight = 16;

		private bool _initialized;
		
		void OnEnable()
		{
			Init();
		}

		private bool Init()
		{
			if (serializedObject.targetObject == null) return false;

			var t = serializedObject.targetObject as Transform;

			worldPos = t.position;
			mPos = serializedObject.FindProperty("m_LocalPosition");
			mRot = serializedObject.FindProperty("m_LocalRotation");
			mScale = serializedObject.FindProperty("m_LocalScale");

			return _initialized = true;
		}

		public override void OnInspectorGUI()
		{
			// if cannot initialize, draw the base gui
			if (!_initialized && !Init())
			{
				base.OnInspectorGUI();
			}
			
			var labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 15;

			serializedObject.Update();

			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope())
				{
					DrawPosition();
					DrawRotation();
					DrawScale();
				}
				
				// SirenixEditorGUI.VerticalLineSeparator();
				
				// using (new GUILayout.VerticalScope(GUILayout.Width(25)))
				// {
				// 	if (GUILayout.Button(new GUIContent("Center", "Move object to center of children without affecting children"), GUILayout.Height(_buttonHeight)))
				// 	{
				// 		foreach (Transform t in targets)
				// 		{
				// 			var bounds = t.GetComponentsInChildren<Collider>().GetCombinedBounds();
				// 			if (bounds == default(Bounds))
				// 				bounds = t.GetComponentsInChildren<MeshFilter>().GetCombinedBounds();
				// 			var newCenter = bounds.center;
				// 			// t.ShiftPivotTo(newCenter, true);
				// 		}
				// 	}
				// 	
				// 	DrawCopyPaste();
				// 	
				// 	if (GUILayout.Button(new GUIContent("Reset", "Reset scale without affecting children"), SirenixGUIStyles.Button, GUILayout.Height(_buttonHeight)))
				// 	{
				// 		// foreach (Transform t in targets)
				// 		// 	t.ShiftScaleTo(Vector3.one, true);
				// 	}
				// }
			}
			
			EditorGUIUtility.labelWidth = labelWidth;

			DrawWorldPosition();

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawPosition()
		{
			GUILayout.BeginHorizontal();
			bool reset = GUILayout.Button("P", GUILayout.Width(20f));
			EditorGUILayout.PropertyField(mPos.FindPropertyRelative("x"));
			EditorGUILayout.PropertyField(mPos.FindPropertyRelative("y"));
			EditorGUILayout.PropertyField(mPos.FindPropertyRelative("z"));
			GUILayout.EndHorizontal();

			if (reset) mPos.vector3Value = Vector3.zero;
		}

		private void DrawScale()
		{
			GUILayout.BeginHorizontal();
			{
				bool reset = GUILayout.Button("S", GUILayout.Width(20f));

				EditorGUILayout.PropertyField(mScale.FindPropertyRelative("x"));
				EditorGUILayout.PropertyField(mScale.FindPropertyRelative("y"));
				EditorGUILayout.PropertyField(mScale.FindPropertyRelative("z"));

				if (reset) mScale.vector3Value = Vector3.one;
			}
			GUILayout.EndHorizontal();
		}

		private void DrawWorldPosition()
		{
			if (worldPos == mPos.vector3Value) return;

			GUILayout.BeginHorizontal();

			// using (new GUILayout.DisabledGroup(true))
			// 	EditorGUILayout.Vector3Field("World Position", worldPos);

			GUILayout.EndHorizontal();
		}

		private void DrawCopyPaste()
		{
			var t = ((Transform) target);

			// using (new eUtility.HorizontalGroup())
			// {
			// 	if (GUILayout.Button(new GUIContent("C", "Copy"), SirenixGUIStyles.ButtonLeft, GUILayout.Height(_buttonHeight)))
			// 	{
			// 		positionClipboard = t.localPosition;
			// 		rotationClipboard = t.localRotation;
			// 		scaleClipboard = t.localScale;
			// 	}
// 
			// 	using (new eUtility.DisabledGroup(!positionClipboard.HasValue))
			// 	{
			// 		if (GUILayout.Button(new GUIContent("P", "Paste"), SirenixGUIStyles.ButtonRight, GUILayout.Height(_buttonHeight)))
			// 		{
			// 			Undo.RecordObjects(targets, "Paste Clipboard Values");
			// 			for (int i = 0; i < targets.Length; i++)
			// 			{
			// 				((Transform) targets[i]).localPosition = positionClipboard.Value;
			// 				((Transform) targets[i]).localRotation = rotationClipboard.Value;
			// 				((Transform) targets[i]).localScale = scaleClipboard.Value;
			// 			}
// 
			// 			GUI.FocusControl(null);
			// 		}
			// 	}
			// }
		}

		#region Rotation

		enum Axes : int
		{
			None = 0,
			X = 1,
			Y = 2,
			Z = 4,
			All = 7,
		}

		Axes CheckDifference(Transform t, Vector3 original)
		{
			Vector3 next = t.localEulerAngles;

			Axes axes = Axes.None;

			if (Differs(next.x, original.x)) axes |= Axes.X;
			if (Differs(next.y, original.y)) axes |= Axes.Y;
			if (Differs(next.z, original.z)) axes |= Axes.Z;

			return axes;
		}

		Axes CheckDifference(SerializedProperty property)
		{
			Axes axes = Axes.None;

			if (property.hasMultipleDifferentValues)
			{
				Vector3 original = property.quaternionValue.eulerAngles;

				foreach (Object obj in serializedObject.targetObjects)
				{
					axes |= CheckDifference(obj as Transform, original);
					if (axes == Axes.All) break;
				}
			}

			return axes;
		}

		/// <summary>
		/// Draw an editable float field.
		/// </summary>
		/// <param name="hidden">Whether to replace the value with a dash</param>
		/// <param name="greyedOut">Whether the value should be greyed out or not</param>

		static bool FloatField(string name, ref float value, bool hidden, bool greyedOut, GUILayoutOption opt)
		{
			float newValue = value;
			GUI.changed = false;

			if (!hidden)
			{
				if (greyedOut)
				{
					GUI.color = new Color(0.7f, 0.7f, 0.7f);
					newValue = EditorGUILayout.FloatField(name, newValue, opt);
					GUI.color = Color.white;
				}
				else
				{
					newValue = EditorGUILayout.FloatField(name, newValue, opt);
				}
			}
			else if (greyedOut)
			{
				GUI.color = new Color(0.7f, 0.7f, 0.7f);
				float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
				GUI.color = Color.white;
			}
			else
			{
				float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
			}

			if (GUI.changed && Differs(newValue, value))
			{
				value = newValue;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Because Mathf.Approximately is too sensitive.
		/// </summary>

		static bool Differs(float a, float b)
		{
			return Mathf.Abs(a - b) > 0.0001f;
		}

		void DrawRotation()
		{
			GUILayout.BeginHorizontal();
			{
				bool reset = GUILayout.Button("R", GUILayout.Width(20f));

				Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;

				visible.x = WrapAngle(visible.x);
				visible.y = WrapAngle(visible.y);
				visible.z = WrapAngle(visible.z);

				Axes changed = CheckDifference(mRot);
				Axes altered = Axes.None;

				GUILayoutOption opt = GUILayout.MinWidth(30f);

				if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, false, opt)) altered |= Axes.X;
				if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, false, opt)) altered |= Axes.Y;
				if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, false, opt)) altered |= Axes.Z;

				if (reset)
				{
					mRot.quaternionValue = Quaternion.identity;
				}
				else if (altered != Axes.None)
				{
					RegisterUndo("Change Rotation", serializedObject.targetObjects);

					foreach (Object obj in serializedObject.targetObjects)
					{
						Transform t = obj as Transform;
						Vector3 v = t.localEulerAngles;

						if ((altered & Axes.X) != 0) v.x = visible.x;
						if ((altered & Axes.Y) != 0) v.y = visible.y;
						if ((altered & Axes.Z) != 0) v.z = visible.z;

						t.localEulerAngles = v;
					}
				}
			}
			GUILayout.EndHorizontal();
		}

		static public void RegisterUndo(string name, params Object[] objects)
		{
			if (objects != null && objects.Length > 0)
			{
				Undo.RecordObjects(objects, name);

				foreach (Object obj in objects)
				{
					if (obj == null) continue;
					EditorUtility.SetDirty(obj);
				}
			}
		}

		static public float WrapAngle(float angle)
		{
			while (angle > 180f) angle -= 360f;
			while (angle < -180f) angle += 360f;
			return angle;
		}

		#endregion
		
		// To override FrameSelected
		public bool HasFrameBounds()
		{
			var t = ((Transform) target);
			
			if (t.gameObject.GetComponentInChildren<MeshRenderer>()) return false;
			if (t.gameObject.GetComponentInChildren<Collider>()) return false;
			
			if (t.gameObject.GetComponentInParent<MeshRenderer>()) return true;
			if (t.gameObject.GetComponentInParent<Collider>()) return true;

			return true;
		}

		public Bounds OnGetFrameBounds()
		{
			var t = ((Transform) target);
			// assuming it will not get here if there is a child mesh, hence not calculating that
			
			var mesh = t.gameObject.GetComponentInParent<MeshRenderer>();

			if (mesh != null)
				return mesh.bounds;
			
			var collider = t.gameObject.GetComponentInParent<Collider>();

			if (collider != null)
				return collider.bounds;
			
			// default small bounds
			return new Bounds(t.position, Vector3.one);
		}
	}
}