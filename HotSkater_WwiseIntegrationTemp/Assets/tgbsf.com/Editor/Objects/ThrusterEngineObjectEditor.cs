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

	[CustomEditor(typeof(EngineObject.MonoBehaviour))]
	public class EngineObjectEditor : Editor
	{

		private bool show_json = false;
		private string wjson;
		private string rjson;

		public void OnEnable()
		{
			EngineObject.MonoBehaviour obj = (EngineObject.MonoBehaviour)target;
			obj.JsonAvailable.AddListener(IncomingJson);
		}


		public void OnDisable()
		{
			EngineObject.MonoBehaviour obj = (EngineObject.MonoBehaviour)target;
			obj.JsonAvailable.RemoveListener(IncomingJson);
		}


		public override void OnInspectorGUI()
		{
			EngineObject.MonoBehaviour obj = (EngineObject.MonoBehaviour)target;

			show_json = EditorGUILayout.Foldout(show_json, "Json Channel", true);

			if (show_json)
			{
				EditorGUILayout.BeginVertical();
				wjson = EditorGUILayout.TextArea(wjson, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(50));
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("WRITE"))
				{
					GUI.FocusControl(null);
					obj.WriteJson(wjson);

				}

				if (GUILayout.Button("READ"))
				{
					GUI.FocusControl(null);
					rjson = obj.ReadJson();
				}

				EditorGUILayout.EndHorizontal();
				EditorGUILayout.LabelField(rjson, EditorStyles.textArea);
				EditorGUILayout.LabelField(string.Join("\n", obj.PeekJson()), EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.MaxHeight(100));
				EditorGUILayout.EndVertical();

			}

		}

		private void IncomingJson()
		{
			Repaint();
		}

	}

}

