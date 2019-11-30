using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Thruster
{
	[ExecuteInEditMode]
	public class Head : MonoBehaviour
	{
		float minDistance = 0.01f;

		[HideInInspector] public bool EstimateViewFrustum = false;
		[HideInInspector] public bool SetNearClipPlane = false;
		[HideInInspector] public Canvas Ui;
		[HideInInspector] public bool existingNode = false;

		Dictionary<Screen, ThrusterResource> screenToMono = new Dictionary<Screen, ThrusterResource>();
		Dictionary<Screen, ThrusterResource> screenToLeft = new Dictionary<Screen, ThrusterResource>();
		Dictionary<Screen, ThrusterResource> screenToRight = new Dictionary<Screen, ThrusterResource>();

		GameObject _mono;
		GameObject mono
		{
			get
			{
				if (_mono == null)
				{
					_mono = new GameObject("Mono");
					_mono.transform.SetParent(transform, false);
				}

				return _mono;
			}
		}

		GameObject _left;
		GameObject left
		{
			get
			{
				if (_left == null)
				{
					_left = new GameObject("Left");
					_left.transform.SetParent(transform, false);
				}

				return _left;
			}
		}

		GameObject _right;
		GameObject right
		{
			get
			{
				if (_right == null)
				{
					_right = new GameObject("Right");
					_right.transform.SetParent(transform, false);
				}

				return _right;
			}
		}

		HashSet<Screen> _screens = new HashSet<Screen>();
		public HashSet<Screen> screens
		{
			get { return _screens; }
			set
			{
				var removed = _screens.Except(value);
				var added = value.Except(_screens);

				foreach (var screen in removed)
				{
					screen.heads.Remove(this);
					RemoveCams(screenToMono, screen);
					RemoveCams(screenToLeft, screen);
					RemoveCams(screenToRight, screen);
				}

				foreach (var screen in added)
				{
					screen.heads.Add(this);
					AddCams(screenToMono, screen, mono, "Mono");
					AddCams(screenToLeft, screen, left, "Left");
					AddCams(screenToRight, screen, right, "Right");
				}

				_screens = value;
			}
		}

		public void Delete()
		{
			Destroy(mono);
			Destroy(left);
			Destroy(right);
			Destroy(this);

			if (!existingNode) Destroy(gameObject);
		}

		public void UpdateCams(Screen screen)
		{
			RemoveCams(screenToMono, screen);
			RemoveCams(screenToLeft, screen);
			RemoveCams(screenToRight, screen);
			AddCams(screenToMono, screen, mono, "Mono");
			AddCams(screenToLeft, screen, left, "Left");
			AddCams(screenToRight, screen, right, "Right");
		}

		public void UpdateResourcesData()
		{
			foreach (var screen in screenToMono.Keys)
			{
				UpdateResourceDataForScreen(screen);
			}
		}

		public void UpdateResourceDataForScreen(Screen screen)
		{
			if (stereo)
			{
				UpdateResourceData(screen, screenToLeft[screen], _left);
				UpdateResourceData(screen, screenToRight[screen], _right);
			}
			else
			{
				UpdateResourceData(screen, screenToMono[screen], _mono);
			}
		}

		void UpdateResourceData(Screen screen, ThrusterResource resource, GameObject eye)
		{
			screen.UpdateResourceData(resource, eye);
		}

		void AddCams(Dictionary<Screen, ThrusterResource> screenToCamGo, Screen screen, GameObject eyeRoot, string suffix)
		{
			var camRootGo = new GameObject(string.Format("{0} Cameras", screen.name));
			camRootGo.transform.SetParent(eyeRoot.transform, false);
			var resource = screenToCamGo[screen] = camRootGo.AddComponent<ThrusterResource>();
			resource.Engine = Engine;
			resource.Name = string.Format("{0}.{1}.{2}", name, suffix, screen.name);

			var rt = new RenderTexture(screen.resolution.x, screen.resolution.y, 16, RenderTextureFormat.ARGB32);
			resource.Resource = rt;

			resource.shaderData = screen.CreateResourceData(Engine);

			foreach (var subscreen in screen.screens)
			{
				var camGo = new GameObject(string.Format("{0} Camera", subscreen.Key));
				camGo.transform.SetParent(camRootGo.transform, false);
				var cam = camGo.AddComponent<Camera>();
				cam.targetTexture = rt;
				cam.rect = subscreen.Value.viewport;
				resource.renderOperations.Add(new ThrusterResource.RenderOperation() { camera = cam, screen = subscreen.Value });
			}
		}

		void RemoveCams(Dictionary<Screen, ThrusterResource> screenToCamGo, Screen screen)
		{
			var resource = screenToCamGo[screen];
			var camRootGo = resource.gameObject;

			foreach (var renderOperation in resource.renderOperations)
			{
				renderOperation.camera.targetTexture = null;
				Destroy(renderOperation.camera.gameObject);
			}

			if (resource.Resource != null)
			{
				resource.Resource.Release();
			}

			screenToCamGo.Remove(screen);
		}

		float[] _clippingPlanes = new float[] { 0.1f, 1000f };
		public float[] clippingPlanes
		{
			get { return _clippingPlanes; }
			set
			{
				_clippingPlanes = value;

				//TODO store all cameras in a list
				foreach (var resource in screenToLeft.Values)
				{
					foreach (var renderOperation in resource.renderOperations)
					{
						renderOperation.camera.nearClipPlane = _clippingPlanes[0];
						renderOperation.camera.farClipPlane = _clippingPlanes[1];
					}
				}

				foreach (var resource in screenToRight.Values)
				{
					foreach (var renderOperation in resource.renderOperations)
					{
						renderOperation.camera.nearClipPlane = _clippingPlanes[0];
						renderOperation.camera.farClipPlane = _clippingPlanes[1];
					}
				}

				foreach (var resource in screenToMono.Values)
				{
					foreach (var renderOperation in resource.renderOperations)
					{
						renderOperation.camera.nearClipPlane = _clippingPlanes[0];
						renderOperation.camera.farClipPlane = _clippingPlanes[1];
					}
				}
			}
		}

		bool _stereo = false;
		public bool stereo
		{
			get { return _stereo; }
			set
			{
				_stereo = value;
				left.SetActive(_stereo);
				right.SetActive(_stereo);
				mono.SetActive(!_stereo);
			}
		}

		float _ipd = 0.065f;
		public float ipd
		{
			get { return _ipd; }
			set
			{
				_ipd = value;
				left.transform.localPosition = Vector3.left * _ipd / 2;
				right.transform.localPosition = Vector3.right * _ipd / 2;
			}
		}

		public ThrusterEngine Engine { get; internal set; }

		void LateUpdate()
		{
			if (stereo)
			{
				foreach (var screen in screenToLeft.Values)
				{
					foreach (var renderOperation in screen.renderOperations)
					{
						OffAxisPerspectiveProjection(renderOperation.camera, renderOperation.screen);
					}
				}

				foreach (var screen in screenToRight.Values)
				{
					foreach (var renderOperation in screen.renderOperations)
					{
						OffAxisPerspectiveProjection(renderOperation.camera, renderOperation.screen);
					}
				}
			}
			else
			{
				foreach (var screen in screenToMono.Values)
				{
					foreach (var renderOperation in screen.renderOperations)
					{
						OffAxisPerspectiveProjection(renderOperation.camera, renderOperation.screen);
					}
				}
			}
		}

		void OffAxisPerspectiveProjection(Camera camera, Screen screen)
		{
			var screenPlane = screen.gameObject.transform;

			Vector3 size = screenPlane.lossyScale / 2;
			Vector3 center = screenPlane.position;

			// C--------D
			// | Screen |
			// A--------B
			Vector3 pa = center - screenPlane.right * size.x - screenPlane.up * size.y;   // lower left corner in world coordinates
			Vector3 pb = center + screenPlane.right * size.x - screenPlane.up * size.y;  // lower right corner
			Vector3 pc = center - screenPlane.right * size.x + screenPlane.up * size.y;   // upper left corner

			// Fix for eye too close to the screen invalid matrix problems, always produce an image
			var eyeInScreen = screenPlane.InverseTransformPoint(camera.transform.position);

			if (-eyeInScreen.z < minDistance)
			{
				camera.enabled = false;
				return;
			}
			else
			{
				camera.enabled = true;
			}

			//eyeInScreen.z = Mathf.Sign(eyeInScreen.z) * Mathf.Max(minDistance, Mathf.Abs(eyeInScreen.z));
			//Vector3 pe = screenPlane.TransformPoint(eyeInScreen);// eye position

			Vector3 pe = camera.transform.position;

			float n = camera.nearClipPlane; // distance to the near clip plane (screen)
			float f = camera.farClipPlane; // distance of far clipping plane

			Vector3 vr = pb - pa; // right axis of screen
			Vector3 vu = pc - pa; // up axis of screen
			Vector3 va = pa - pe; // from pe to pa
			Vector3 vb = pb - pe; // from pe to pb
			Vector3 vc = pc - pe; // from pe to pc

			// are we looking at the backface of the plane object?
			//var backFace = Vector3.Dot(-Vector3.Cross(va, vc), vb) < 0.0f;

			//if (backFace)
			//{
			//	// mirror points along the x axis (most users
			//	// probably expect the y axis to stay fixed)
			//	vr = -vr;
			//	pa = pb;
			//	pb = pa + vr;
			//	pc = pa + vu;
			//	va = pa - pe;
			//	vb = pb - pe;
			//	vc = pc - pe;
			//}

			vr.Normalize();
			vu.Normalize();
			Vector3 vn = -Vector3.Cross(vr, vu); // normal vector of screen
			// we need the minus sign because Unity
			// uses a left-handed coordinate system
			vn.Normalize();

			float d = -Vector3.Dot(va, vn); // distance from eye to screen

			if (SetNearClipPlane)
			{
				n = d;
				camera.nearClipPlane = n;
			}

			float l = Vector3.Dot(vr, va) * n / d; // distance to left screen edge from the 'center'
			float r = Vector3.Dot(vr, vb) * n / d; // distance to right screen edge from 'center'
			float b = Vector3.Dot(vu, va) * n / d; // distance to bottom screen edge from 'center'
			float t = Vector3.Dot(vu, vc) * n / d; // distance to top screen edge from 'center'

			Matrix4x4 p = new Matrix4x4(); // Projection matrix
			p[0, 0] = 2.0f * n / (r - l);
			p[0, 2] = (r + l) / (r - l);
			p[1, 1] = 2.0f * n / (t - b);
			p[1, 2] = (t + b) / (t - b);
			p[2, 2] = (f + n) / (n - f);
			p[2, 3] = 2.0f * f * n / (n - f);
			p[3, 2] = -1.0f;

			Matrix4x4 rm = new Matrix4x4(); // rotation matrix;
			rm[0, 0] = vr.x;
			rm[0, 1] = vr.y;
			rm[0, 2] = vr.z;
			rm[1, 0] = vu.x;
			rm[1, 1] = vu.y;
			rm[1, 2] = vu.z;
			rm[2, 0] = vn.x;
			rm[2, 1] = vn.y;
			rm[2, 2] = vn.z;
			rm[3, 3] = 1.0f;

			Matrix4x4 tm = new Matrix4x4(); // translation matrix;
			tm[0, 0] = 1.0f;
			tm[0, 3] = -pe.x;
			tm[1, 1] = 1.0f;
			tm[1, 3] = -pe.y;
			tm[2, 2] = 1.0f;
			tm[2, 3] = -pe.z;
			tm[3, 3] = 1.0f;

			camera.projectionMatrix = p;
			camera.worldToCameraMatrix = rm * tm;

			// The original paper puts everything into the projection
			// matrix (i.e. sets it to p * rm * tm and the other
			// matrix to the identity), but this doesn't appear to
			// work with Unity's shadow maps.

			if (EstimateViewFrustum)
			{
				// rotate camera to screen for culling to work
				Quaternion q = new Quaternion();
				q.SetLookRotation((0.5f * (pb + pc) - pe), vu);
				// look at center of screen
				camera.transform.rotation = q;

				// set fieldOfView to a conservative estimate
				// to make frustum tall enough
				if (camera.aspect >= 1.0)
				{
					camera.fieldOfView = Mathf.Rad2Deg * Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude) / va.magnitude);
				}
				else
				{
					// take the camera aspect into account to
					// make the frustum wide enough
					camera.fieldOfView = Mathf.Rad2Deg / camera.aspect * Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude) / va.magnitude);
				}
			}

			if (Ui)
			{
				var rectTr = Ui.transform as RectTransform;
				var dUi = d + Ui.planeDistance;
				var dScale = dUi / d;

				Vector3 paUi = pe + (pa - pe) * dScale; // lower left corner in world coordinates
				Vector3 pbUi = pe + (pb - pe) * dScale; // lower right corner
				Vector3 pcUi = pe + (pc - pe) * dScale; // upper left corner

				Ui.transform.position = pe + (center - pe) * dScale;
				Ui.transform.localScale = new Vector3(Vector3.Distance(paUi, pbUi) / rectTr.rect.width, Vector3.Distance(paUi, pcUi) / rectTr.rect.height, 1);
			}
		}
	}
}