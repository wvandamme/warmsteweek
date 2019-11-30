using System;
using UnityEngine;

namespace ThrusterConfigGUI
{
	public enum Eye
	{
		Mono,
		Left,
		Right
	}

	public enum StereoMode
	{
		QuadBuffer,
		SideBySide,
		DualLink
	}

	[Serializable]
	public class ConfigRepr
	{
		public string mapPath;
		public string collisionMeshPath;
		public string occlusionMeshPath;
		public ARTTrackerLibRepr[] artTrackerLibs;
		public FlatScreenRepr[] flatScreens;
		public DomeScreenRepr[] domeScreens;
		public ToreScreenRepr[] toreScreens;
		public HeadRepr[] heads;
		public CameraRepr[] cameras;
		public ARMarkerRepr[] arMarkers;
		public PhysicalDisplayRepr[] physicalDisplays;
		public ViewportRepr[] viewports;
		public WarpMeshRepr[] warpMeshes;
		public DisplayRepr[] displays;
		public TrackerRepr[] trackers;
	}

	[Serializable]
	public class PhysicalDisplayRepr
	{
		public string id;
		public string name;
		public bool primary;
		public int[] pos;
		public int[] size;
	}

	[Serializable]
	public class ScreenRepr
	{
		public int index;
		public int localIndex;
		public string labelName;
		public Vector2Int resolution;
		public Vector3 position;
		public Vector3 rotation;
	}

	[Serializable]
	public class FlatScreenRepr : ScreenRepr
	{
		public Vector2 size;
		public int pivotIndex;
	}

	[Serializable]
	public class DomeScreenRepr : ScreenRepr
	{
		public float radius;
		public float angleUp;
		public float angleDown;
		public float fieldOfView;
		public float[] cropping;
	}

	[Serializable]
	public class ToreScreenRepr : ScreenRepr
	{
		public float radius1;
		public float radius2;
	}

	[Serializable]
	public class HeadRepr
	{
		public int index;
		public string labelName;
		public Vector3 position;
		public Vector3 rotation;
		public float[] clippingPlanes;
		public bool stereo;
		public float ipd;
		public int[] screenIndices;
		public int trackerIndex;

		public Vector3 handPosition;
		public Vector3 handRotation;
		public bool[] buttons;
		public float[] axes;
	}

	[Serializable]
	public class CameraRepr
	{
		public string labelName;
		public Vector3 position;
		public Vector3 rotation;
		public float fieldOfView;
	}

	[Serializable]
	public class ARMarkerRepr
	{
		public int index;
		public string labelName;
		public Vector3 position;
		public Vector3 rotation;
		public Vector2 size;
	}

	[Serializable]
	public class ChannelRepr
	{
		public int headIndex;
		public Eye eye;
	}

	[Serializable]
	public class WarpMeshRepr
	{
		public string labelName;
		public int screenIndex;
		public Vector2Int pointsDimensions;
		public Vector2[] points;
		public string blendMaskPath;
		public int displayIndex;
		public ChannelRepr[] channels;
	}

	[Serializable]
	public class ViewportRepr : WarpMeshRepr
	{
	}

	[Serializable]
	public class DisplayRepr
	{
		public int index;
		public string labelName;
		public bool stereo;
		public StereoMode stereoMode;
		public Vector2Int resolution;
		public string[] physicalDisplayIds;
		public Vector2[] homographyPoints;
	}

	[Serializable]
	public class TrackerLibRepr
	{
		public int trackerIndex;
	}

	[Serializable]
	public class ARTTrackerLibRepr : TrackerLibRepr
	{
		public string serverAddress;
		public string dataAddress;
		public int dataPort;
	}

	[Serializable]
	public class TrackerRepr
	{
		public int index;
		public string labelName;
		public Vector3 position;
		public Vector3 rotation;
	}
}
