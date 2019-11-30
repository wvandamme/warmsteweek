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

using UnityEngine;

namespace Thruster
{
	[AddComponentMenu("Thruster/Camera")]
	public class ThrusterCamera : MonoBehaviour
	{

		public ThrusterEngine Engine;
		public CameraObject.Type Type;
		public CameraObject.Mode Mode;

		public string mono_display;
		public string left_display;
		public string right_display;

		private ThrusterEngine.Camera m_camera = null;

		private void Awake()
		{
			m_camera = new ThrusterEngine.Camera(transform, Type, Mode, mono_display, left_display, right_display);
		}

		private void OnEnable()
		{
			Engine.RegisterCamera(m_camera);
		}

		private void OnDisable()
		{
			Engine.UnregisterCamera(m_camera);
		}

	}


}