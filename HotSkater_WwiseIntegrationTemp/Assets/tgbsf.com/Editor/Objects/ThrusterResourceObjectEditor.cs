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

	[CustomEditor(typeof(ResourceObject.MonoBehaviour))]
	public class ResourceObjectEditor : Editor
	{

		private bool m_live = true;

		public void OnEnable()
		{
			ResourceObject.MonoBehaviour obj = (ResourceObject.MonoBehaviour)target;
			obj.EnablePreviewTexture();
		}
		public void OnDisable()
		{
			ResourceObject.MonoBehaviour obj = (ResourceObject.MonoBehaviour)target;
			obj.DisablePreviewTexture();
		}

		public override void OnInspectorGUI()
		{
			//EditorGUILayout.LabelField("Test2");
		}

		public override bool HasPreviewGUI()
		{
			return true;
		}

		public override void OnPreviewSettings()
		{
			m_live = GUILayout.Toggle(m_live, "live");
		}

		public override GUIContent GetPreviewTitle()
		{
			return new GUIContent("Thruster Resource");
		}

		public override void OnInteractivePreviewGUI(Rect window_, GUIStyle background_)
		{
			ResourceObject.MonoBehaviour obj = (ResourceObject.MonoBehaviour)target;
			EditorGUI.DrawTextureTransparent(window_, obj.GetPreviewTexture(), ScaleMode.ScaleToFit);
		}

		public override bool RequiresConstantRepaint()
		{
			return m_live;
		}

	}

}

