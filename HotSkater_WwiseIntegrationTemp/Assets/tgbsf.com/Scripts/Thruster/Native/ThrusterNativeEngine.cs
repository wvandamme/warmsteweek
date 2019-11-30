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

		public class Engine
		{

			[DllImport("ThrusterNativePlugin")]
			private static extern System.IntPtr ThrusterNativePluginCreateEngine(System.IntPtr plugin, string name_);

			[DllImport("ThrusterNativePlugin")]
			private static extern void ThrusterNativePluginEngineDestroy(System.IntPtr plugin_, System.IntPtr engine_);

			private System.IntPtr m_plugin = System.IntPtr.Zero;
			private System.IntPtr m_engine = System.IntPtr.Zero;

			public Engine(System.IntPtr plugin_, string name_)
			{
				m_plugin = plugin_;
				m_engine = ThrusterNativePluginCreateEngine(m_plugin, name_);
			}

			public void Destroy()
			{
				ThrusterNativePluginEngineDestroy(m_plugin, m_engine);
				m_engine = System.IntPtr.Zero;
			}

			public Logger CreateLogger()
			{
				if (m_engine == System.IntPtr.Zero) return null;

				return new Logger(m_plugin, m_engine);
			}

			public JsonChannel CreateJsonChannel()
			{
				if (m_engine == System.IntPtr.Zero) return null;

				return new JsonChannel(m_plugin, m_engine);
			}

			public Factory CreateFactory()
			{
				if (m_engine == System.IntPtr.Zero) return null;

				return new Factory(m_plugin, m_engine);
			}

			public Resource CreateResource(string name_, UnityEngine.Texture texture_, ResourceDataImpl data_)
			{
				if (m_engine == System.IntPtr.Zero) return null;

				return new Resource(m_plugin, m_engine, name_, texture_, data_);
			}

			public ResourceData<T> CreateResourceData<T>()
			{
				if (m_engine == System.IntPtr.Zero) return null;

				return new ResourceData<T>(m_plugin, m_engine);
			}

			public Display CreateDisplay(string id_)
			{
				if (m_engine == System.IntPtr.Zero) return null;

				return new Display(m_plugin, m_engine, id_);
			}

			public View CreateView(string name_)
			{
				if (m_engine == System.IntPtr.Zero) return null;

				return new View(m_plugin, m_engine, name_);
			}

		};

	}

}
