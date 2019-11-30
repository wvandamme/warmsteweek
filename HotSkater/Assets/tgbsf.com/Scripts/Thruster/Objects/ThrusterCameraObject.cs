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

using System.Collections.Generic;

namespace Thruster
{

	public class CameraObject
	{

		public enum Type
		{
			Flat = 0,
			Dome = 1,
			Cave = 2,
			AugmentedReality = 3,
			ProjectionMapping = 4
		}

		public enum Mode
		{
			Mode2D = 0,
			Mode3D = 1
		}

		public class MonoBehaviourFlat2D : UnityEngine.MonoBehaviour
		{

			private string m_name;
			private UnityEngine.Transform m_transform;
			private Native.Engine m_engine;
			private Native.Display m_display;
			private Native.Resource m_resource;
			private UnityEngine.RenderTexture m_render_texture;
			private UnityEngine.Camera m_cam;

			public void Initialize(UnityEngine.Transform transform_, Native.Engine engine_, string id_)
			{
				m_name = System.Guid.NewGuid().ToString();
				m_transform = transform_;
				m_engine = engine_;

				m_display = m_engine.CreateDisplay(id_);
				Native.View v = m_engine.CreateView(m_name);
				m_display.SetPrimaryView(v);
				v.Destroy();

				m_cam = gameObject.AddComponent<UnityEngine.Camera>();
				m_cam.targetTexture = new UnityEngine.RenderTexture(m_display.GetWidth(), m_display.GetHeight(), 16);
				m_resource = m_engine.CreateResource(m_name, m_cam.targetTexture, null);
			}

			public void OnDestroy()
			{
				m_resource.Destroy();
				m_display.Destroy();

			}

			public void Update()
			{
				transform.position = m_transform.position;
				transform.rotation = m_transform.rotation;
				transform.localScale = m_transform.localScale;
			}

		}

		public class MonoBehaviourFlat3D : UnityEngine.MonoBehaviour
		{

			//private string m_name;
			private UnityEngine.Transform m_transform;

			public void Initialize(UnityEngine.Transform transform_, Native.Engine engine_, string lid_, string rid_)
			{
				//m_name = System.Guid.NewGuid().ToString();
				m_transform = transform_;
				Update();
			}

			public void Update()
			{
				transform.position = m_transform.position;
				transform.rotation = m_transform.rotation;
				transform.localScale = m_transform.localScale;
			}

		}

		private UnityEngine.GameObject m_object;

		public CameraObject(UnityEngine.Transform transform_, Native.Engine engine_, UnityEngine.GameObject parent_, Type type_, Mode mode_, string id_, string lid_, string rid_)
		{
			m_object = new UnityEngine.GameObject(transform_.name);
			m_object.hideFlags = UnityEngine.HideFlags.NotEditable;
			m_object.transform.SetParent(parent_.transform, false);

			if (type_ == Type.Flat)
			{
				if (mode_ == Mode.Mode2D)
				{
					m_object.AddComponent<MonoBehaviourFlat2D>().Initialize(transform_, engine_, id_);
				}
				else
				{
					m_object.AddComponent<MonoBehaviourFlat3D>().Initialize(transform_, engine_, lid_, rid_);
				}
			}

		}

		public void Destroy()
		{
			UnityEngine.GameObject.Destroy(m_object);
		}

	}

}

