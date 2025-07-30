using TMPro;
using UnityEngine;

namespace Quinn
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class VersionUI : MonoBehaviour
	{
		private TextMeshProUGUI _textBlock;

		private void OnValidate()
		{
			if (_textBlock == null)
			{
				Awake();
			}

			Update();
		}

		private void Awake()
		{
			_textBlock = GetComponent<TextMeshProUGUI>();

			Debug.developerConsoleEnabled = false;
			Debug.developerConsoleVisible = false;
		}

		private void Update()
		{
			string gameVersion = $"v{Application.version}";
			string unityVersion = $"u{Application.unityVersion}";

			_textBlock.text = $"{gameVersion}\n{unityVersion}";
		}
	}
}
