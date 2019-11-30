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

namespace Thruster
{

	public class ResourceObject
	{

		public class MonoBehaviour : UnityEngine.MonoBehaviour
		{

			Native.Resource m_impl;

			public void EnablePreviewTexture()
			{
				m_impl.EnablePreviewTexture();
			}

			public void DisablePreviewTexture()
			{
				m_impl.DisablePreviewTexture();
			}

			public UnityEngine.Texture GetPreviewTexture()
			{
				return m_impl.GetPreviewTexture();
			}

			public MonoBehaviour SetImplementation(Native.Resource impl_)
			{
				m_impl = impl_;
				return this;
			}

		}

		private Native.Resource m_resource;
		private UnityEngine.GameObject m_object;

		public ResourceObject(Native.Engine engine_, UnityEngine.GameObject parent_, string name_, UnityEngine.Texture texture_, Native.ResourceDataImpl data_)
		{
			m_resource = engine_.CreateResource(name_, texture_, data_);
			m_object = new UnityEngine.GameObject(name_);
			m_object.hideFlags = UnityEngine.HideFlags.NotEditable;
			m_object.transform.SetParent(parent_.transform, false);
			m_object.AddComponent<MonoBehaviour>().SetImplementation(m_resource);
		}

		public void Destroy()
		{
			UnityEngine.GameObject.Destroy(m_object);
			m_resource.Destroy();
		}

	}

}


