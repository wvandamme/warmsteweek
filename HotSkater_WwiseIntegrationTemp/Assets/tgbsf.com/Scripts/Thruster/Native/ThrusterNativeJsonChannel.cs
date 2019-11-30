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

		public class JsonChannel
		{

			public delegate void ReadDelegate(string json_);

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginEngineCreateJsonChannel(System.IntPtr plugin_, System.IntPtr engine_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineJsonChannelDestroy(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr channel_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineJsonChannelWrite(System.IntPtr plugin_, System.IntPtr channel_, string json_);

			public delegate void ReadDelegateC(System.IntPtr json_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineJsonChannelSetReadCallback(System.IntPtr plugin_, System.IntPtr channel_, System.IntPtr callback_);

			private System.IntPtr m_plugin = System.IntPtr.Zero;
			private System.IntPtr m_engine = System.IntPtr.Zero;
			private System.IntPtr m_impl = System.IntPtr.Zero;
			private ReadDelegate m_handler;
			private ReadDelegateC m_callback;

			public JsonChannel(System.IntPtr plugin_, System.IntPtr engine_)
			{
				m_plugin = plugin_;
				m_engine = engine_;
				m_impl = ThrusterNativePluginEngineCreateJsonChannel(m_plugin, m_engine);
				m_callback = new ReadDelegateC(Callback);
			}

			public void Destroy()
			{
				ThrusterNativePluginEngineJsonChannelDestroy(m_plugin, m_engine, m_impl);
				m_impl = System.IntPtr.Zero;
			}

			public void Write(string json_)
			{
				ThrusterNativePluginEngineJsonChannelWrite(m_plugin, m_impl, json_);
			}

			public void SetReadCallback(ReadDelegate handler_)
			{
				if (m_impl == System.IntPtr.Zero) return;

				m_handler = handler_;
				ThrusterNativePluginEngineJsonChannelSetReadCallback(m_plugin, m_impl, Marshal.GetFunctionPointerForDelegate(m_callback));
			}

			private void Callback(System.IntPtr json_)
			{
				try
				{
					m_handler(Marshal.PtrToStringAnsi(json_));
				}
				catch (System.Exception e_)
				{
					UnityEngine.Debug.LogException(e_);
				}
			}

		}

	}

}
