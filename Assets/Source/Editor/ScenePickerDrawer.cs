using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Quinn.Editor
{
	[CustomPropertyDrawer(typeof(ScenePickerAttribute))]
	public class ScenePickerDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				return UIHelper.CreateErrorBox($"The '<b>ScenePicker</b>' attribute is used on a non-string field ('<b>{property.name}</b>')!");
			}

			var existingAsset = AssetDatabase.LoadAssetAtPath<Object>(property.stringValue);

			if (!string.IsNullOrWhiteSpace(property.stringValue))
			{
				if (existingAsset == null)
				{
					var comp = property.serializedObject.targetObject as Component;
					Debug.LogWarning($"The field '{comp.GetType().Name}' on game object '{comp.gameObject.name}' failed to deserialize the scene path '{property.stringValue}'! Did you change the scene's path?");
				}
			}

			var scenePicker = new ObjectField(ObjectNames.NicifyVariableName(property.name))
			{
				allowSceneObjects = false,
				objectType = typeof(SceneAsset),
				value = existingAsset
			};
			scenePicker.labelElement.style.display = property.isArray ? DisplayStyle.None : DisplayStyle.Flex;

			property.serializedObject.ApplyModifiedProperties();

			scenePicker.RegisterValueChangedCallback(evt =>
			{
				var asset = evt.newValue as SceneAsset;
				string path = AssetDatabase.GetAssetPath(asset);
				property.stringValue = path;
				property.serializedObject.ApplyModifiedProperties();
			});
			return scenePicker;
		}
	}
}