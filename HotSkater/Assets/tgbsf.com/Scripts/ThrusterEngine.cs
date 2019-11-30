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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThrusterConfigGUI;
using UnityEngine;

namespace Thruster
{

	[AddComponentMenu("Thruster/Engine")]
	public class ThrusterEngine : MonoBehaviour
	{
		public string Name = "MyExperience";
		public Transform RealWorldRoot = null;

		public class Camera
		{

			private CameraObject obj = null;
			public Transform transform;
			public CameraObject.Type type;
			public CameraObject.Mode mode;
			public string id;
			public string lid;
			public string rid;

			public Camera(Transform transform_, CameraObject.Type type_, CameraObject.Mode mode_, string id_, string lid_, string rid_)
			{
				transform = transform_;
				type = type_;
				mode = mode_;
				id = id_;
				lid = lid_;
				rid = rid_;
			}

			public void Attach(EngineObject engine_)
			{
				if (engine_ == null) return;

				obj = engine_.CreateCamera(transform, type, mode, id, lid, rid);
			}

			public void Detach()
			{
				if (obj == null) return;

				obj.Destroy();
				obj = null;
			}

		}

		public class Resource
		{

			private string name;
			private Texture texture;
			private ResourceObject obj = null;
			private Native.ResourceDataImpl data = null;

			public Resource(string name_, Texture texture_, Native.ResourceDataImpl data_)
			{
				name = name_;
				texture = texture_;
				data = data_;
			}

			public void Attach(EngineObject engine_)
			{
				if (engine_ == null) return;

				obj = engine_.CreateResource(name, texture, data);
			}

			public void Detach()
			{
				if (obj == null) return;

				obj.Destroy();
				obj = null;
			}

		}

		private HashSet<Resource> m_resources = new HashSet<Resource>();
		private HashSet<Camera> m_cameras = new HashSet<Camera>();
		private EngineObject m_engine = null;
		public UnityEngine.Events.UnityEvent JsonAvailable = new UnityEngine.Events.UnityEvent();

		void OnEnable()
		{
			m_engine = Manager.CreateEngine(Name);
			//m_engine.PopulateRealWorld(RealWorldRoot);

			foreach (Resource i in m_resources) i.Attach(m_engine);

			foreach (Camera i in m_cameras) i.Attach(m_engine);

			m_engine.JsonAvailable.AddListener(ReadJsonQueue);
		}

		void OnDisable()
		{
			m_engine.JsonAvailable.RemoveListener(ReadJsonQueue);

			foreach (Camera i in m_cameras) i.Detach();

			foreach (Resource i in m_resources) i.Detach();

			m_engine.Destroy();
			m_engine = null;
		}

		public void RegisterResource(Resource resource_)
		{
			m_resources.Add(resource_);
			resource_.Attach(m_engine);
		}

		public void UnregisterResource(Resource resource_)
		{
			m_resources.Remove(resource_);
			resource_.Detach();
		}

		public Native.ResourceData<T> CreateResourceData<T>()
		{
			return m_engine.CreateResourceData<T>();
		}

		public void RegisterCamera(Camera camera_)
		{
			m_cameras.Add(camera_);
			camera_.Attach(m_engine);
		}

		public void UnregisterCamera(Camera camera_)
		{
			m_cameras.Remove(camera_);
			camera_.Detach();
		}

		public EngineObject.LogLine[] GetLogLines()
		{
			if (m_engine != null)
			{
				return m_engine.GetLogLines();
			}

			return null;
		}

		public string[] PeekJson()
		{
			return m_engine.PeekJson();
		}

		public string ReadJson()
		{
			return m_engine.ReadJson();
		}

		public void WriteJson(string json_)
		{
			m_engine.WriteJson(json_);
		}

		// Paul migration
		public Dictionary<int, Head> heads = new Dictionary<int, Head>();
		public Dictionary<int, Screen> screens = new Dictionary<int, Screen>();

		Dictionary<int, Screen> flatScreens { get { return GetScreens<FlatScreen>(); } }
		Dictionary<int, Screen> domeScreens { get { return GetScreens<DomeScreen>(); } }
		Dictionary<int, Screen> toreScreens { get { return GetScreens<ToreScreen>(); } }

		Dictionary<int, Screen> GetScreens<T>()
		{
			return screens
			       .Where(obj => typeof(T).IsAssignableFrom(obj.Value.GetType()))
			       .ToDictionary(obj => obj.Key, obj => obj.Value);
		}

		void Awake()
		{
			if (RealWorldRoot == null)
				RealWorldRoot = new GameObject("VR Root").transform;
		}

		void ReadJsonQueue()
		{
			var msg = "";

			while ((msg = m_engine.ReadJson()) != "")
			{
				NewIncommingJson(msg);
			}
		}

		void NewIncommingJson(string json_)
		{
			var msg = JsonUtility.FromJson<JsonMessage>(json_);
			var type = Type.GetType("ThrusterConfigGUI." + msg.msgType + msg.classType + "Message");
			var detailed = JsonUtility.FromJson(json_, type);

			MethodInfo method = this.GetType().GetMethod(
			                        msg.msgType + msg.classType,
			                        BindingFlags.Instance | BindingFlags.NonPublic,
			                        null,
			                        new Type[] { type },
			                        null);

			if (method != null)
			{
				method.Invoke(this, new object[] { detailed });
			}
		}

		void RemoveScreen(int index)
		{
			var removedScreen = screens[index];

			foreach (var head in heads.Values)
			{
				if (head.screens.Contains(removedScreen))
				{
					HashSet<Screen> headScreens = new HashSet<Screen>(head.screens);
					headScreens.Remove(removedScreen);
					head.screens = headScreens;
				}
			}

			Destroy(screens[index].gameObject);
			screens.Remove(index);
		}

		void AddFlatScreen(AddFlatScreenMessage msg)
		{
			var name = msg.classType + msg.screen.localIndex;
			GameObject findFlatScreenGo = RealWorldRoot.Find(name)?.gameObject;
			var flatScreenGo = findFlatScreenGo ?? new GameObject(name);

			flatScreenGo.transform.SetParent(RealWorldRoot, false);
			var flatScreen = flatScreenGo.AddComponent<FlatScreen>();
			screens.Add(msg.screen.index, flatScreen);
			UpdateFlatScreen(new UpdateFlatScreenMessage() { screen = msg.screen });
		}

		void UpdateFlatScreen(UpdateFlatScreenMessage msg)
		{
			var screen = screens[msg.screen.index];
			screen.transform.localPosition = msg.screen.position;
			screen.transform.localRotation = Quaternion.Euler(msg.screen.rotation);
			screen.transform.localScale = new Vector3(msg.screen.size.x, msg.screen.size.y, 1);
			screen.resolution = msg.screen.resolution;
		}

		void RemoveFlatScreen(RemoveFlatScreenMessage msg)
		{
			RemoveScreen(msg.screen.index);
		}

		void AddDomeScreen(AddDomeScreenMessage msg)
		{
			var name = msg.classType + msg.screen.localIndex;
			GameObject findDomeScreenGo = RealWorldRoot.Find(name)?.gameObject;
			var domeScreenGo = findDomeScreenGo ?? new GameObject(name);

			domeScreenGo.transform.SetParent(RealWorldRoot, false);
			var domeScreen = domeScreenGo.AddComponent<DomeScreen>();
			screens.Add(msg.screen.index, domeScreen);
			UpdateDomeScreen(new UpdateDomeScreenMessage() { screen = msg.screen });
		}

		void UpdateDomeScreen(UpdateDomeScreenMessage msg)
		{
			var screen = screens[msg.screen.index] as DomeScreen;
			screen.transform.localPosition = msg.screen.position;
			screen.transform.localRotation = Quaternion.Euler(msg.screen.rotation);
			screen.radius = msg.screen.radius;
			screen.angleUp = msg.screen.angleUp;
			screen.angleDown = msg.screen.angleDown;
			screen.fieldOfView = msg.screen.fieldOfView;
			screen.resolution = msg.screen.resolution;
			screen.cropping = msg.screen.cropping;
		}

		void RemoveDomeScreen(RemoveDomeScreenMessage msg)
		{
			RemoveScreen(msg.screen.index);
		}

		void AddToreScreen(AddToreScreenMessage msg)
		{
			var name = msg.classType + msg.screen.localIndex;
			GameObject findToreScreenGo = RealWorldRoot.Find(name)?.gameObject;
			var toreScreenGo = findToreScreenGo ?? new GameObject(name);

			toreScreenGo.transform.SetParent(RealWorldRoot, false);
			var toreScreen = toreScreenGo.AddComponent<ToreScreen>();
			screens.Add(msg.screen.index, toreScreen);
			UpdateToreScreen(new UpdateToreScreenMessage() { screen = msg.screen });
		}

		void UpdateToreScreen(UpdateToreScreenMessage msg)
		{
			var screen = screens[msg.screen.index] as ToreScreen;
			screen.transform.localPosition = msg.screen.position;
			screen.transform.localRotation = Quaternion.Euler(msg.screen.rotation);
			screen.radius1 = msg.screen.radius1;
			screen.radius2 = msg.screen.radius2;
			screen.resolution = msg.screen.resolution;
		}

		void RemoveToreScreen(RemoveToreScreenMessage msg)
		{
			RemoveScreen(msg.screen.index);
		}

		void AddHead(AddHeadMessage msg)
		{
			var name = msg.classType + msg.head.index;
			GameObject findHeadGo = RealWorldRoot.Find(name)?.gameObject;
			var headGo = findHeadGo ?? new GameObject(name);
			headGo.transform.SetParent(RealWorldRoot, false);
			var head = headGo.AddComponent<Head>();
			head.Engine = this;
			head.existingNode = findHeadGo != null;
			heads.Add(msg.head.index, head);

			UpdateHead(new UpdateHeadMessage() { head = msg.head });
		}

		void UpdateHead(UpdateHeadMessage msg)
		{
			var headGo = heads[msg.head.index];
			headGo.transform.localPosition = msg.head.position;
			headGo.transform.localRotation = Quaternion.Euler(msg.head.rotation);
			var head = headGo.GetComponent<Head>();

			var headScreens = new List<Screen>();

			foreach (var screenIndex in msg.head.screenIndices)
			{
				headScreens.Add(screens[screenIndex]);
			}

			head.screens = new HashSet<Screen>(headScreens);

			head.clippingPlanes = msg.head.clippingPlanes;
			head.stereo = msg.head.stereo;
			head.ipd = msg.head.ipd;

			head.UpdateResourcesData();
		}

		void RemoveHead(RemoveHeadMessage msg)
		{
			var headGo = heads[msg.head.index];
			var head = headGo.GetComponent<Head>();
			head.screens = new HashSet<Screen>(); // Trigger the screens dereferencing
			head.Delete();
			heads.Remove(msg.head.index);
		}

		void UpdateConfig(UpdateConfigMessage msg)
		{
			// Flat screens
			var oldFlatScreenIndices = new HashSet<int>(flatScreens.Keys);
			var newFlatScreenIndices = new HashSet<int>(msg.config.flatScreens.Select(flat => flat.index));
			var configFlatScreens = msg.config.flatScreens.ToDictionary(flat => flat.index);
			var removedFlatScreens = oldFlatScreenIndices.Except(newFlatScreenIndices);
			var updatedFlatScreens = oldFlatScreenIndices.Intersect(newFlatScreenIndices);
			var addedFlatScreens = newFlatScreenIndices.Except(oldFlatScreenIndices);

			// Dome screens
			var oldDomeScreenIndices = new HashSet<int>(domeScreens.Keys);
			var newDomeScreenIndices = new HashSet<int>(msg.config.domeScreens.Select(dome => dome.index));
			var configDomeScreens = msg.config.domeScreens.ToDictionary(dome => dome.index);
			var removedDomeScreens = oldDomeScreenIndices.Except(newDomeScreenIndices);
			var updatedDomeScreens = oldDomeScreenIndices.Intersect(newDomeScreenIndices);
			var addedDomeScreens = newDomeScreenIndices.Except(oldDomeScreenIndices);

			// Tore screens
			var oldToreScreenIndices = new HashSet<int>(toreScreens.Keys);
			var newToreScreenIndices = new HashSet<int>(msg.config.toreScreens.Select(tore => tore.index));
			var configToreScreens = msg.config.toreScreens.ToDictionary(tore => tore.index);
			var removedToreScreens = oldToreScreenIndices.Except(newToreScreenIndices);
			var updatedToreScreens = oldToreScreenIndices.Intersect(newToreScreenIndices);
			var addedToreScreens = newToreScreenIndices.Except(oldToreScreenIndices);

			// Heads
			var oldHeadIndices = new HashSet<int>(heads.Keys);
			var newHeadIndices = new HashSet<int>(msg.config.heads.Select(headRepr => headRepr.index));
			var configHeads = msg.config.heads.ToDictionary(headRepr => headRepr.index);
			var removedHeads = oldHeadIndices.Except(newHeadIndices);
			var updatedHeads = oldHeadIndices.Intersect(newHeadIndices);
			var addedHeads = newHeadIndices.Except(oldHeadIndices);

			foreach (var index in removedHeads)
			{
				RemoveHead(new RemoveHeadMessage() { head = new HeadRepr() { index = index } });
			}

			// Removals
			foreach (var index in removedFlatScreens)
			{
				RemoveFlatScreen(new RemoveFlatScreenMessage() { screen = new FlatScreenRepr() { index = index } });
			}

			foreach (var index in removedDomeScreens)
			{
				RemoveDomeScreen(new RemoveDomeScreenMessage() { screen = new DomeScreenRepr() { index = index } });
			}

			foreach (var index in removedToreScreens)
			{
				RemoveToreScreen(new RemoveToreScreenMessage() { screen = new ToreScreenRepr() { index = index } });
			}

			// Addings
			foreach (var index in addedFlatScreens)
			{
				AddFlatScreen(new AddFlatScreenMessage() { screen = configFlatScreens[index] });
			}

			foreach (var index in addedDomeScreens)
			{
				AddDomeScreen(new AddDomeScreenMessage() { screen = configDomeScreens[index] });
			}

			foreach (var index in addedToreScreens)
			{
				AddToreScreen(new AddToreScreenMessage() { screen = configToreScreens[index] });
			}

			// Updates
			foreach (var index in updatedFlatScreens)
			{
				UpdateFlatScreen(new UpdateFlatScreenMessage() { screen = configFlatScreens[index] });
			}

			foreach (var index in updatedDomeScreens)
			{
				UpdateDomeScreen(new UpdateDomeScreenMessage() { screen = configDomeScreens[index] });
			}

			foreach (var index in updatedToreScreens)
			{
				UpdateToreScreen(new UpdateToreScreenMessage() { screen = configToreScreens[index] });
			}

			foreach (var index in addedHeads)
			{
				AddHead(new AddHeadMessage() { head = configHeads[index] });
			}

			foreach (var index in updatedHeads)
			{
				UpdateHead(new UpdateHeadMessage() { head = configHeads[index] });
			}
		}
	}

}