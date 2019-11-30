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

		public class Logger
		{

			public enum Severity
			{
				Error = 0,
				Warning = 1,
				Info = 2,
				Debug = 3
			}

			public delegate void NewLineDelegate(Severity severity_, string origin_, string msg_);

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginEngineCreateLogger(System.IntPtr plugin_, System.IntPtr engine_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineLoggerDestroy(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr logger_);

			private delegate void NewLineDelegateC(Severity severity_, System.IntPtr origin_, System.IntPtr msg_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineLoggerSetNewLineCallback(System.IntPtr plugin_, System.IntPtr logger_, System.IntPtr callback_);

			private System.IntPtr m_plugin = System.IntPtr.Zero;
			private System.IntPtr m_engine = System.IntPtr.Zero;
			private System.IntPtr m_impl = System.IntPtr.Zero;
			private NewLineDelegate m_handler;
			private NewLineDelegateC m_callback;

			public Logger(System.IntPtr plugin_, System.IntPtr engine_)
			{
				m_plugin = plugin_;
				m_engine = engine_;
				m_impl = ThrusterNativePluginEngineCreateLogger(m_plugin, m_engine);
				m_callback = new NewLineDelegateC(Callback);
			}

			public void Destroy()
			{
				ThrusterNativePluginEngineLoggerDestroy(m_plugin, m_engine, m_impl);
				m_impl = System.IntPtr.Zero;
			}

			public void SetNewLineCallback(NewLineDelegate handler_)
			{
				if (m_impl == System.IntPtr.Zero) return;

				m_handler = handler_;
				ThrusterNativePluginEngineLoggerSetNewLineCallback(m_plugin, m_impl, Marshal.GetFunctionPointerForDelegate(m_callback));
			}

			public void Callback(Severity severity_, System.IntPtr origin_, System.IntPtr msg_)
			{
				try
				{
					m_handler(severity_, Marshal.PtrToStringAnsi(origin_), Marshal.PtrToStringAnsi(msg_));
				}
				catch (System.Exception e_)
				{
					UnityEngine.Debug.LogException(e_);
				}
			}

		}

	}

}

