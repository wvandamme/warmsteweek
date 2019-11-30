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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Thruster
{
	public class ThrusterLogWindowEditor : UnityEditor.EditorWindow
	{

		private bool m_live = true;
		private string m_curr_selected;
		private string m_prev_selected;
		private UnityEngine.Vector2 m_scroll = UnityEngine.Vector2.zero;
		private string m_log_data;

		[UnityEditor.MenuItem("Window/Thruster/New Console")]
		public static void ShowThrusterLogWindow()
		{
			ThrusterLogWindowEditor window = CreateInstance<ThrusterLogWindowEditor>();
			window.titleContent = new UnityEngine.GUIContent("Thruster Console");
			window.Show();
		}

		public void OnGUI()
		{
			ThrusterEngine[] engines = UnityEngine.Object.FindObjectsOfType<ThrusterEngine>();

			if (engines.Length <= 0)
			{
				UnityEditor.EditorGUILayout.BeginVertical();
				UnityEditor.EditorGUILayout.Space();
				UnityEditor.EditorGUILayout.LabelField("No Thruster engines in the scene");
				UnityEditor.EditorGUILayout.EndVertical();
				return;
			}

			SortedDictionary<string, ThrusterEngine> map = new SortedDictionary<string, ThrusterEngine>();

			foreach (ThrusterEngine e in engines) map.Add(e.Name, e);

			string[] keys = map.Keys.ToArray();
			int pos = System.Math.Max(0, System.Array.IndexOf(keys, m_curr_selected));

			if (m_curr_selected != m_prev_selected)
			{
				m_log_data = "";
				m_prev_selected = m_curr_selected;
			}

			UnityEditor.EditorGUILayout.BeginVertical();
			UnityEditor.EditorGUILayout.Space();

			UnityEditor.EditorGUILayout.BeginHorizontal();
			m_live = UnityEditor.EditorGUILayout.Toggle(m_live, UnityEngine.GUILayout.MaxWidth(15));
			m_curr_selected = keys[UnityEditor.EditorGUILayout.Popup(pos, keys, UnityEngine.GUILayout.ExpandWidth(true))];
			UnityEditor.EditorGUILayout.EndHorizontal();

			m_scroll = UnityEditor.EditorGUILayout.BeginScrollView(m_scroll, true, true);

			UnityEngine.Texture2D black = new UnityEngine.Texture2D(1, 1);
			black.SetPixel(0, 0, new UnityEngine.Color(0, 0, 0));
			black.Apply();

			UnityEngine.GUIStyle log_style = new UnityEngine.GUIStyle(UnityEditor.EditorStyles.textArea);
			log_style.richText = true;
			log_style.wordWrap = false;
			log_style.stretchWidth = true;
			log_style.normal.background = black;

			if (m_live)
			{
				EngineObject.LogLine[] lines = map[m_curr_selected].GetLogLines();

				if (lines != null)
				{
					m_log_data = "";

					foreach (EngineObject.LogLine l in lines)
					{
						switch (l.severity)
						{
						case Native.Logger.Severity.Warning:
							m_log_data += "<color=#ffff00ff>";
							break;

						case Native.Logger.Severity.Error:
							m_log_data += "<color=#ff0000ff>";
							break;

						case Native.Logger.Severity.Info:
							m_log_data += "<color=#0000ffff>";
							break;

						default:
							m_log_data += "<color=#000000ff>";
							break;
						}

						m_log_data += l.severity + "[" + l.origin + "]: " + l.message + "</color>\n";
					}
				}

				UnityEngine.GUIContent content = new UnityEngine.GUIContent(m_log_data);
				UnityEngine.Vector2 size = log_style.CalcSize(content);
				UnityEditor.EditorGUILayout.LabelField(content, log_style, UnityEngine.GUILayout.MinWidth(size.x), UnityEngine.GUILayout.MinHeight(size.y), UnityEngine.GUILayout.ExpandHeight(true));
			}
			else
			{
				UnityEditor.EditorGUILayout.LabelField("");
			}

			UnityEditor.EditorGUILayout.EndScrollView();
			UnityEditor.EditorGUILayout.Space();
			UnityEditor.EditorGUILayout.EndVertical();
		}

		private void OnInspectorUpdate()
		{
			Repaint();
		}

	}
}