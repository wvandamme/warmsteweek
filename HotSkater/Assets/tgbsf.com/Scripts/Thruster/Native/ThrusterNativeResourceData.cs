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

		public class ResourceDataImpl
		{

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginEngineCreateResourceData(System.IntPtr plugin_, System.IntPtr engine_, int size_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineResourceDataDestroy(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr data_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineResourceDataPresent(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr data_, System.IntPtr ptr_);

			private System.IntPtr m_plugin = System.IntPtr.Zero;
			private System.IntPtr m_engine = System.IntPtr.Zero;
			private System.IntPtr m_impl = System.IntPtr.Zero;

			public ResourceDataImpl(System.IntPtr plugin_, System.IntPtr engine_, int size_)
			{
				m_plugin = plugin_;
				m_engine = engine_;
				m_impl = ThrusterNativePluginEngineCreateResourceData(m_plugin, m_engine, size_);
			}

			public virtual void Destroy()
			{
				ThrusterNativePluginEngineResourceDataDestroy(m_plugin, m_engine, m_impl);
				m_impl = System.IntPtr.Zero;
			}

			public System.IntPtr GetImpl()
			{
				return m_impl;
			}

			public void Present(System.IntPtr ptr_)
			{
				ThrusterNativePluginEngineResourceDataPresent(m_plugin, m_engine, m_impl, ptr_);
			}

		}

		public class ResourceData<T> : ResourceDataImpl
		{

			private System.IntPtr ptr;

			public ResourceData(System.IntPtr plugin_, System.IntPtr engine_) :
				base(plugin_, engine_, Marshal.SizeOf<T>())
			{
				ptr = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
			}

			public void Set(T data_)
			{
				Marshal.StructureToPtr<T>(data_, ptr, false);
				Present(ptr);
			}

			public override void Destroy()
			{
				base.Destroy();
				Marshal.FreeHGlobal(ptr);
			}

		}

	}

}

