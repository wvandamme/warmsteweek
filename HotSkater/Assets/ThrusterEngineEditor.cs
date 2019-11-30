/*#***************************************************************************
   Copyright (c) 2018 Wim Vandamme <wim dot vandamme at tgbsf dot com>

   Permission to use, copy, modify, and/or distribute this software for any
   purpose with or without fee is hereby granted, provided that the above
   copyright notice and this permission notice appear in all copies.

   THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
   WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
   MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY
   SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
   WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
   ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR
   IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

 *#***************************************************************************/

using UnityEditor;
using UnityEngine;

namespace Thruster
{
	[CustomEditor(typeof(ThrusterEngine))]
	public class ThrusterEngineEditor : Editor
	{

		private GUIContent logo;

		private void OnEnable()
		{
			logo = new GUIContent(Resources.Load<Texture2D>("tgbsf_logo"));
		}

		public override void OnInspectorGUI()
		{
			ThrusterEngine engine = (ThrusterEngine)target;

			engine.Name = EditorGUILayout.TextField("Name", engine.Name);
			engine.RealWorldRoot = EditorGUILayout.ObjectField("Real World Root", engine.RealWorldRoot, typeof(Transform), true) as Transform;
			EditorGUILayout.Space();

			//if (GUILayout.Button("http://www.thegoosebumpsfactory.com", GUI.skin.label)) Application.OpenURL("http://www.tgbsf.com");
			//if (GUILayout.Button(logo, GUI.skin.label, GUILayout.Height(100))) Application.OpenURL("http://www.tgbsf.com");
		}

		[MenuItem("GameObject/Thruster/Engine", false, 10)]
		static void CreateThrusterEngine(MenuCommand command_)
		{
			GameObject thruster = new GameObject("ThrusterEngine");
			GameObject root = new GameObject("Root");
			GameObjectUtility.SetParentAndAlign(root, thruster);
			GameObjectUtility.SetParentAndAlign(thruster, command_.context as GameObject);
			ThrusterEngine engine = thruster.AddComponent<ThrusterEngine>();
			engine.RealWorldRoot = root.transform;
			Undo.RegisterCreatedObjectUndo(thruster, "Create " + thruster.name);
			Selection.activeObject = thruster;
		}

	}
}