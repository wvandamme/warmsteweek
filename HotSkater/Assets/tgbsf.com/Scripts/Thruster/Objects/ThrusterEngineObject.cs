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

using System.Collections.Generic;

namespace Thruster
{

	public class EngineObject
	{

		public class LogLine
		{
			public Native.Logger.Severity severity;
			public string origin;
			public string message;
		}

		public class MonoBehaviour : UnityEngine.MonoBehaviour
		{

			private EngineObject m_impl;

			public void SetImplementation(EngineObject impl_)
			{
				m_impl = impl_;
			}

			public UnityEngine.Events.UnityEvent JsonAvailable
			{
				get { return m_impl.JsonAvailable; }
			}

			public void WriteJson(string json_)
			{
				m_impl.WriteJson(json_);
			}

			public string ReadJson()
			{
				return m_impl.ReadJson();
			}

			public string[] PeekJson()
			{
				return m_impl.PeekJson();
			}

		}

		public UnityEngine.Events.UnityEvent JsonAvailable;

		private Native.Engine m_engine;
		private Native.Logger m_logger;
		private Native.Factory m_factory;
		private Native.JsonChannel m_json_channel;
		private UnityEngine.GameObject m_object;
		private UnityEngine.GameObject m_world;
		private UnityEngine.GameObject m_resources;
		private UnityEngine.GameObject m_cameras;

		private Queue<LogLine> m_logs = new Queue<LogLine>();
		private Queue<string> m_json = new Queue<string>();

		public EngineObject(UnityEngine.GameObject parent_, string name_)
		{
			JsonAvailable = new UnityEngine.Events.UnityEvent();

			m_engine = Manager.GetNativePlugin().CreateEngine(name_);

			m_object = new UnityEngine.GameObject(name_);
			//m_object.hideFlags = UnityEngine.HideFlags.NotEditable;
			m_object.transform.SetParent(parent_.transform, false);
			m_object.AddComponent<MonoBehaviour>().SetImplementation(this);

			m_logger = m_engine.CreateLogger();
			m_logger.SetNewLineCallback(NewLogLine);
			m_json_channel = m_engine.CreateJsonChannel();
			m_json_channel.SetReadCallback(NewIncommingJson);
			m_factory = m_engine.CreateFactory();

			m_resources = new UnityEngine.GameObject("Resources");
			m_resources.hideFlags = UnityEngine.HideFlags.NotEditable;
			m_resources.transform.SetParent(m_object.transform, false);

			m_cameras = new UnityEngine.GameObject("Cameras");
			m_cameras.hideFlags = UnityEngine.HideFlags.NotEditable;
			m_cameras.transform.SetParent(m_object.transform, false);

		}

		public void Destroy()
		{
			UnityEngine.GameObject.Destroy(m_world);
			UnityEngine.GameObject.Destroy(m_object);
			m_factory.Destroy();
			m_json_channel.Destroy();
			m_logger.Destroy();
			m_engine.Destroy();
		}

		public string[] PeekJson()
		{
			return m_json.ToArray();
		}

		public string ReadJson()
		{
			return (m_json.Count > 0) ? m_json.Dequeue() : "";
		}

		public void WriteJson(string json_)
		{
			m_json_channel.Write(json_);
		}

		private void NewIncommingJson(string json_)
		{
			m_json.Enqueue(json_);
			JsonAvailable.Invoke();
		}

		public void PopulateRealWorld(UnityEngine.Transform root_ = null)
		{
			m_world = new UnityEngine.GameObject("RealWorldRoot");
			m_world.hideFlags = UnityEngine.HideFlags.NotEditable;
			m_world.transform.SetParent((root_ == null) ? m_object.transform : root_, false);
		}

		public ResourceObject CreateResource(string name_, UnityEngine.Texture texture_, Native.ResourceDataImpl data_)
		{
			return new ResourceObject(m_engine, m_resources, name_, texture_, data_);
		}

		public CameraObject CreateCamera(UnityEngine.Transform transform_, CameraObject.Type type_, CameraObject.Mode mode_, string id_, string lid_, string rid_)
		{
			return new CameraObject(transform_, m_engine, m_cameras, type_, mode_, id_, lid_, rid_);
		}

		public Native.ResourceData<T> CreateResourceData<T>()
		{
			return m_engine.CreateResourceData<T>();
		}

		public EngineObject.LogLine[] GetLogLines()
		{
			return m_logs.ToArray();
		}

		private void NewLogLine(Native.Logger.Severity severity_, string origin_, string message_)
		{
			m_logs.Enqueue(new LogLine()
			{
				severity = severity_,
				origin = origin_,
				message = message_
			});

			while (m_logs.Count > 1024)
			{
				m_logs.Dequeue();
			}
		}

	};

}
