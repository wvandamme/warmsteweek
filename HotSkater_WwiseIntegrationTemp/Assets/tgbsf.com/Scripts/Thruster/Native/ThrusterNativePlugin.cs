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

		public class Plugin
		{

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginOpen(bool debug_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginClose(System.IntPtr plugin_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginTick(System.IntPtr plugin_);

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginGetRenderEventFunc();

			private System.IntPtr m_plugin = System.IntPtr.Zero;
			private System.IntPtr m_render = System.IntPtr.Zero;

			public bool Open(bool debug_)
			{
				m_plugin = ThrusterNativePluginOpen(debug_);
				m_render = ThrusterNativePluginGetRenderEventFunc();
				return (m_plugin != System.IntPtr.Zero);
			}

			public void Close()
			{
				if (m_plugin != System.IntPtr.Zero)
				{
					ThrusterNativePluginClose(m_plugin);
					m_plugin = System.IntPtr.Zero;
					m_render = System.IntPtr.Zero;
				}
			}

			public void Tick()
			{
				UnityEngine.Profiling.Profiler.BeginSample("Thruster.Native.Plugin.Tick()");

				if (m_plugin != System.IntPtr.Zero)
				{
					ThrusterNativePluginTick(m_plugin);
				}

				UnityEngine.Profiling.Profiler.EndSample();
			}

			public void Render()
			{
				if (m_render != System.IntPtr.Zero)
				{
					UnityEngine.GL.IssuePluginEvent(m_render, 0);
				}
			}

			public Engine CreateEngine(string name_)
			{
				return new Engine(m_plugin, name_);
			}

		}

	}

}
