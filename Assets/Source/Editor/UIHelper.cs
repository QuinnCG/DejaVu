using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace Quinn
{
	public static class UIHelper
	{
		public const string RootIconPath = "Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular";

		public static readonly Color EditorPrimary = new(0.2196f, 0.2196f, 0.2196f);
		public static readonly Color EditorField = new(0.1568f, 0.1568f, 0.1568f);
		public static readonly Color EditorInteractable = new(0.345f, 0.345f, 0.345f);
		public static readonly Color EditorText = new(0.8235f, 0.8235f, 0.8235f);
		public static readonly Color EditorAccent = new(0.4980f, 0.8392f, 0.9882f);
		public static readonly Color EditorOutline = new(0.07843137254f, 0.07843137254f, 0.07843137254f);

		/// <summary>
		/// If you wanted to load the ExclamationMark icon you would simply input "ExclamationMark" for the name paramter.<br/>
		/// Do note that this method will search in the following path: 'Packages/com.unity.dt.app-ui/PackageResources/Icons/Regular/'.
		/// </summary>
		public static Texture LoadIcon(string name)
		{
			return AssetDatabase.LoadAssetAtPath<Texture>($"{RootIconPath}/{name}.png");
		}

		public static VisualElement CreateErrorBox(string message)
		{
			var error = new VisualElement();
			error.style.flexDirection = FlexDirection.Row;
			error.style.SetPadding(2f);
			error.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
			error.style.SetBorderRadius(5f);

			var errorIcon = LoadIcon("ExclamationMark");
			var icon = new Image
			{
				image = errorIcon,
				tintColor = Color.red
			};
			icon.style.height = icon.style.width = 20f;
			error.Add(icon);

			var msg = new Label(message);
			msg.style.color = Color.red;
			msg.style.unityTextAlign = TextAnchor.MiddleLeft;
			error.Add(msg);

			return error;
		}

		public static VisualElement VerticalSpacer(float height)
		{
			var element = new VisualElement();
			element.style.height = height;

			return element;
		}
	}
}