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

using System.Runtime.InteropServices;

namespace Thruster
{

	namespace Native
	{

		public class Resource
		{

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginEngineCreateResource(System.IntPtr plugin_, System.IntPtr engine_, string name_, System.IntPtr callback_, System.IntPtr preview_, System.IntPtr data_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineResourceEnablePreviewTexture(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr resource_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineResourceDisablePreviewTexture(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr resource_);

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginEngineResourceGetPreviewTexturePtr(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr resource_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineResourceDestroy(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr resource_);

			private delegate System.IntPtr GetResourcePtrDelegate();

			private System.IntPtr m_plugin = System.IntPtr.Zero;
			private System.IntPtr m_engine = System.IntPtr.Zero;
			private GetResourcePtrDelegate m_callback = null;
			private GetResourcePtrDelegate m_preview = null;
			private System.IntPtr m_impl = System.IntPtr.Zero;
			private UnityEngine.Texture m_texture = null;
			private UnityEngine.Texture m_preview_texture = null;

			public Resource(System.IntPtr plugin_, System.IntPtr engine_, string name_, UnityEngine.Texture texture_, ResourceDataImpl data_)
			{
				m_plugin = plugin_;
				m_engine = engine_;
				m_callback = new GetResourcePtrDelegate(GetResourcePtr);
				m_preview = new GetResourcePtrDelegate(GetPreviewPtr);
				m_impl = ThrusterNativePluginEngineCreateResource(m_plugin, m_engine, name_, Marshal.GetFunctionPointerForDelegate(m_callback), Marshal.GetFunctionPointerForDelegate(m_preview), (data_ != null) ? data_.GetImpl() : System.IntPtr.Zero);
				m_texture = texture_;

			}

			public void Destroy()
			{
				ThrusterNativePluginEngineResourceDestroy(m_plugin, m_engine, m_impl);
				m_impl = System.IntPtr.Zero;
			}

			public System.IntPtr GetResourcePtr()
			{
				try
				{
					UnityEngine.Profiling.Profiler.BeginSample("Thruster.Native.Resource.GetResourcePtr()");
					System.IntPtr ret = m_texture.GetNativeTexturePtr();
					UnityEngine.Profiling.Profiler.EndSample();
					return ret;
				}
				catch (System.Exception e_)
				{
					UnityEngine.Debug.LogException(e_);
				}

				return System.IntPtr.Zero;
			}

			public System.IntPtr GetPreviewPtr()
			{
				try
				{
					UnityEngine.Profiling.Profiler.BeginSample("Thruster.Native.Resource.GetPreviewPtr()");
					System.IntPtr ret = (m_preview_texture != null) ? m_preview_texture.GetNativeTexturePtr() : System.IntPtr.Zero;
					UnityEngine.Profiling.Profiler.EndSample();
					return ret;
				}
				catch (System.Exception e_)
				{
					UnityEngine.Debug.LogException(e_);
				}

				return System.IntPtr.Zero;
			}

			public void EnablePreviewTexture()
			{
				if (m_plugin == System.IntPtr.Zero) return;

				m_preview_texture = new UnityEngine.Texture2D(m_texture.width, m_texture.height, UnityEngine.TextureFormat.ARGB32, false);
				ThrusterNativePluginEngineResourceEnablePreviewTexture(m_plugin, m_engine, m_impl);
			}

			public void DisablePreviewTexture()
			{
				if (m_plugin == System.IntPtr.Zero) return;

				ThrusterNativePluginEngineResourceDisablePreviewTexture(m_plugin, m_engine, m_impl);
				m_preview_texture = null;
			}

			public UnityEngine.Texture GetPreviewTexture()
			{
				if (m_preview_texture != null)
				{
					return m_preview_texture;
				}

				return UnityEngine.Texture2D.whiteTexture;
			}

		}

	}

}

