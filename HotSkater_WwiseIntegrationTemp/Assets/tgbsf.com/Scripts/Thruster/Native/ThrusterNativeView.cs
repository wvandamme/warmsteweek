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

		public class View
		{

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginEngineCreateView(System.IntPtr plugin_, System.IntPtr engine_, string name_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineViewDestroy(System.IntPtr plugin_, System.IntPtr engine_, System.IntPtr view_);

			private System.IntPtr m_plugin = System.IntPtr.Zero;
			private System.IntPtr m_engine = System.IntPtr.Zero;
			private System.IntPtr m_view = System.IntPtr.Zero;

			public View(System.IntPtr plugin_, System.IntPtr engine_, string name_)
			{
				m_plugin = plugin_;
				m_engine = engine_;
				m_view = ThrusterNativePluginEngineCreateView(m_plugin, m_engine, name_);
			}

			public void Destroy()
			{
				ThrusterNativePluginEngineViewDestroy(m_plugin, m_engine, m_view);
				m_view = System.IntPtr.Zero;
			}

			public System.IntPtr GetPtr()
			{
				return m_view;
			}

		}

	}

}


