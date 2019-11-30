using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThrusterConfigGUI
{
	public class JsonMessage
	{
		public string msgType = "";
		public string classType = "";
	}

	[System.Serializable]
	public class AddFlatScreenMessage : JsonMessage
	{
		public AddFlatScreenMessage() { base.msgType = "Add"; base.classType = "FlatScreen"; }
		public FlatScreenRepr screen;
	}

	[System.Serializable]
	public class UpdateFlatScreenMessage : JsonMessage
	{
		public UpdateFlatScreenMessage() { base.msgType = "Update"; base.classType = "FlatScreen"; }
		public FlatScreenRepr screen;
	}

	[System.Serializable]
	public class RemoveFlatScreenMessage : JsonMessage
	{
		public RemoveFlatScreenMessage() { base.msgType = "Remove"; base.classType = "FlatScreen"; }
		public FlatScreenRepr screen;
	}

	[System.Serializable]
	public class AddDomeScreenMessage : JsonMessage
	{
		public AddDomeScreenMessage() { base.msgType = "Add"; base.classType = "DomeScreen"; }
		public DomeScreenRepr screen;
	}

	[System.Serializable]
	public class UpdateDomeScreenMessage : JsonMessage
	{
		public UpdateDomeScreenMessage() { base.msgType = "Update"; base.classType = "DomeScreen"; }
		public DomeScreenRepr screen;
	}

	[System.Serializable]
	public class RemoveDomeScreenMessage : JsonMessage
	{
		public RemoveDomeScreenMessage() { base.msgType = "Remove"; base.classType = "DomeScreen"; }
		public DomeScreenRepr screen;
	}

	[System.Serializable]
	public class AddToreScreenMessage : JsonMessage
	{
		public AddToreScreenMessage() { base.msgType = "Add"; base.classType = "ToreScreen"; }
		public ToreScreenRepr screen;
	}

	[System.Serializable]
	public class UpdateToreScreenMessage : JsonMessage
	{
		public UpdateToreScreenMessage() { base.msgType = "Update"; base.classType = "ToreScreen"; }
		public ToreScreenRepr screen;
	}

	[System.Serializable]
	public class RemoveToreScreenMessage : JsonMessage
	{
		public RemoveToreScreenMessage() { base.msgType = "Remove"; base.classType = "ToreScreen"; }
		public ToreScreenRepr screen;
	}

	[System.Serializable]
	public class AddHeadMessage : JsonMessage
	{
		public AddHeadMessage() { base.msgType = "Add"; base.classType = "Head"; }
		public HeadRepr head;
	}

	[System.Serializable]
	public class RemoveHeadMessage : JsonMessage
	{
		public RemoveHeadMessage() { base.msgType = "Remove"; base.classType = "Head"; }
		public HeadRepr head;
	}

	[System.Serializable]
	public class UpdateHeadMessage : JsonMessage
	{
		public UpdateHeadMessage() { base.msgType = "Update"; base.classType = "Head"; }
		public HeadRepr head;
	}

	[System.Serializable]
	public class UpdateConfigMessage : JsonMessage
	{
		public UpdateConfigMessage() { base.msgType = "Update"; base.classType = "Config"; }
		public ConfigRepr config;
	}
}