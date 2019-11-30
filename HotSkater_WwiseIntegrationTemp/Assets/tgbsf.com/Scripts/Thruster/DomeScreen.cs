using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Thruster.Native;
using UnityEngine;

namespace Thruster
{
	[StructLayout(LayoutKind.Sequential)]
	public struct DomeData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
		public float[] cropping;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public float[] eyePos;
		[MarshalAs(UnmanagedType.R4)]
		public float radius;
		[MarshalAs(UnmanagedType.R4)]
		public float texel_size_w; //texel size x,y + resolution x, y
		[MarshalAs(UnmanagedType.R4)]
		public float texel_size_h; //texel size x,y + resolution x, y

	}

	public class DomeScreen : Screen
	{
		public override string[] screensNames => new string[]
		{
			"Top",
			"Bottom",
			"Front",
			"Left",
			"Right",
			//"Back" //TODO : Enable this again or smartly cull (disabled only for the demo)
		};

		float _radius;
		public float radius
		{
			get => _radius;

			set
			{
				if (value != _radius)
				{
					_radius = value;
					UpdateScreens();
					UpdateShaders();
				}
			}
		}

		float _angleUp;
		public float angleUp
		{
			get => _angleUp;

			set
			{
				if (value != _angleUp)
				{
					_angleUp = value;
					UpdateScreens();
				}
			}
		}

		float _angleDown;
		public float angleDown
		{
			get => _angleDown;

			set
			{
				if (value != _angleDown)
				{
					_angleDown = value;
					UpdateScreens();
				}
			}
		}

		float _fieldOfView;
		public float fieldOfView
		{
			get => _fieldOfView;

			set
			{
				if (value != _fieldOfView)
				{
					_fieldOfView = value;
					UpdateScreens();
				}
			}
		}

		[HideInInspector] public float[] cropping;

		public override Vector2Int resolution
		{
			set
			{
				base.resolution = new Vector2Int(value.x * 6, value.x);
			}
		}

		void UpdateScreens()
		{
			UpdateScreen("Top",
			             new Vector3(-90, 0, 0),
			             new Rect(0f / 6, 0, 1f / 6, 1));

			UpdateScreen("Bottom",
			             new Vector3(90, 0, 0),
			             new Rect(1f / 6, 0, 1f / 6, 1));

			UpdateScreen("Front",
			             Vector3.zero,
			             new Rect(2f / 6, 0, 1f / 6, 1));

			UpdateScreen("Left",
			             new Vector3(0, -90, 0),
			             new Rect(3f / 6, 0, 1f / 6, 1));

			UpdateScreen("Right",
			             new Vector3(0, 90, 0),
			             new Rect(4f / 6, 0, 1f / 6, 1));

			//TODO : Enable this again or smartly cull (disabled only for the demo)
			//UpdateScreen("Back",
			//             new Vector3(0, -180, 0),
			//             new Rect(5f / 6, 0, 1f / 6, 1));
		}

		void UpdateScreen(string side, Vector3 rotation, Rect viewport)
		{
			UpdateScreen(side, rotation, Vector2.one * 2 * _radius, Vector3.forward * _radius, viewport);
		}

		public override ResourceDataImpl CreateResourceData(ThrusterEngine engine)
		{
			return engine.CreateResourceData<DomeData>();
		}

		public override void UpdateResourceData(ThrusterResource resource, GameObject eye)
		{
			var localEyePos = transform.InverseTransformPoint(eye.transform.position);
			var domeshaderData = resource.shaderData as ResourceData<DomeData>;
			var width = resource.Resource.width;
			var height = resource.Resource.height;
			domeshaderData?.Set(
			    new DomeData
			{
				eyePos = localEyePos.ToFloatArray(),
				radius = _radius,
				texel_size_w = width,
				texel_size_h = height,
				cropping = cropping
			}
			);
		}
	}
}