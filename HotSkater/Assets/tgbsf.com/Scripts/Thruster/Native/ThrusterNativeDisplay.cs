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
using System.Text;

namespace Thruster
{

	namespace Native
	{

		public class Display
		{

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginEngineAttachDisplay(System.IntPtr plugin_, System.IntPtr engine_, string id_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineDisplayDestroy(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr display_);

			[DllImport("ThrusterNativePlugin")]
			private static extern bool ThrusterNativePluginDisplaySetPrimaryView(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr display_, System.IntPtr view_);

			[DllImport("ThrusterNativePlugin")]
			private static extern bool ThrusterNativePluginDisplayClearPrimaryView(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr display_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginDisplayGetName(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr display_, StringBuilder name_, int length_);

			[DllImport("ThrusterNativePlugin")]
			private static extern int ThrusterNativePluginDisplayGetWidth(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr display_);

			[DllImport("ThrusterNativePlugin")]
			private static extern int ThrusterNativePluginDisplayGetHeight(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr display_);

			private System.IntPtr m_plugin = System.IntPtr.Zero;
			private System.IntPtr m_engine = System.IntPtr.Zero;
			private System.IntPtr m_display = System.IntPtr.Zero;

			public Display(System.IntPtr plugin_, System.IntPtr engine_, string id_)
			{
				m_plugin = plugin_;
				m_engine = engine_;
				m_display = ThrusterNativePluginEngineAttachDisplay(m_plugin, m_engine, id_);
			}

			public void Destroy()
			{
				ThrusterNativePluginEngineDisplayDestroy(m_plugin, m_engine, m_display);
				m_display = System.IntPtr.Zero;
			}

			public void SetPrimaryView(View view_)
			{
				if (m_display == System.IntPtr.Zero) return;

				ThrusterNativePluginDisplaySetPrimaryView(m_plugin, m_engine, m_display, view_.GetPtr());
			}

			public void ClearPrimaryView()
			{
				if (m_display == System.IntPtr.Zero) return;

				ThrusterNativePluginDisplayClearPrimaryView(m_plugin, m_engine, m_display);
			}

			public string GetName()
			{
				if (m_display == System.IntPtr.Zero) return "";

				StringBuilder s = new StringBuilder(1024);
				ThrusterNativePluginDisplayGetName(m_plugin, m_engine, m_display, s, s.Capacity);
				return s.ToString();
			}

			public int GetWidth()
			{
				return ThrusterNativePluginDisplayGetWidth(m_plugin, m_engine, m_display);
			}

			public int GetHeight()
			{
				return ThrusterNativePluginDisplayGetHeight(m_plugin, m_engine, m_display);
			}

		}

	}

}
