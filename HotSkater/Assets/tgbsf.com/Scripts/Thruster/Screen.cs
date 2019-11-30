using System;
using System.Collections;
using System.Collections.Generic;
using Thruster.Native;
using UnityEngine;

namespace Thruster
{
	public class Screen : MonoBehaviour
	{
		public virtual string[] screensNames => new string[] { };
		Dictionary<string, FlatScreen> _screens;
		public virtual Dictionary<string, FlatScreen> screens
		{
			get
			{
				if (_screens == null)
				{
					_screens = new Dictionary<string, FlatScreen>();

					foreach (var screenName in screensNames)
					{
						var flatScreenGo = new GameObject(screenName);
						flatScreenGo.transform.SetParent(transform);
						_screens[screenName] = flatScreenGo.AddComponent<FlatScreen>();
					}
				}

				return _screens;
			}
		}

		public HashSet<Head> heads = new HashSet<Head>();

		protected Vector2Int _resolution;
		public virtual Vector2Int resolution
		{
			get { return _resolution; }
			set
			{
				if (value != _resolution)
				{
					_resolution = value;
					UpdateHeads();
				}
			}
		}

		protected void UpdateScreen(string side, Vector3 rotation, Vector2 screenSize, Vector3 screenOffset, Rect viewport)
		{
			screens[side].transform.localRotation = Quaternion.Euler(rotation);
			screens[side].transform.localPosition = Vector3.zero;
			screens[side].transform.Translate(screenOffset, Space.Self);
			screens[side].transform.transform.localScale = new Vector3(screenSize.x, screenSize.y, 1);
			screens[side].viewport = viewport;
		}

		protected void UpdateHeads()
		{
			foreach (var head in heads)
			{
				head.UpdateCams(this);
			}
		}

		protected void UpdateShaders()
		{
			foreach (var head in heads)
			{
				head.UpdateResourceDataForScreen(this);
			}
		}

		public virtual ResourceDataImpl CreateResourceData(ThrusterEngine engine)
		{
			return null;
		}

		public virtual void UpdateResourceData(ThrusterResource resource, GameObject eye)
		{

		}
	}
}