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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Thruster
{

	public class Manager
	{

		static UnityEngine.GameObject _instance;
		static Manager.MonoBehaviour instance
		{
			get
			{
				if (Manager._instance == null)
				{
					Manager._instance = new UnityEngine.GameObject("Thruster");
					Manager._instance.hideFlags = UnityEngine.HideFlags.NotEditable;
					Manager._instance.AddComponent<Manager.MonoBehaviour>();
				}

				return Manager._instance.GetComponent<Manager.MonoBehaviour>();
			}
		}

		static public Native.Plugin GetNativePlugin()
		{
			return instance.GetNativePlugin();
		}

		static public EngineObject CreateEngine(string name_)
		{
			return instance.CreateEngine(name_);
		}

		[UnityEngine.AddComponentMenu("")]
		public class MonoBehaviour : UnityEngine.MonoBehaviour
		{

			private Native.Plugin m_plugin = new Native.Plugin();

			void Awake()
			{
				DontDestroyOnLoad(gameObject);
				StartCoroutine("EndOfFrameEvent");
			}

			void OnEnable()
			{
				m_plugin.Open(true);
			}

			void OnDisable()
			{
				m_plugin.Close();
			}

			private void Update()
			{
				m_plugin.Tick();
			}

			private IEnumerator EndOfFrameEvent()
			{
				for (; ; )
				{
					yield return new UnityEngine.WaitForEndOfFrame();

					try { m_plugin.Render(); }
					catch { }
				}
			}

			public Native.Plugin GetNativePlugin()
			{
				return m_plugin;
			}

			public EngineObject CreateEngine(string name_)
			{
				return new EngineObject(gameObject, name_);
			}

		}

	}

}
