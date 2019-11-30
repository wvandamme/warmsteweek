using UnityEngine;

namespace Thruster
{
	public static class Utilities
	{
		public static float[] ToFloatArray(this Vector3 vec) => new float[] { vec.x, vec.y, vec.z };
	}
}