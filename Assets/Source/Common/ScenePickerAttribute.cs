using System;
using UnityEngine;

namespace Quinn
{
	/// <summary>
	/// Draws the string as an asset picker that only allows scene assets to be selected.<br/>
	/// Used in conjunction with the ScenePickerDrawer editor script to provide a scene picker in the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ScenePickerAttribute : PropertyAttribute
	{ }
}