using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class CrosshairManager : MonoBehaviour
	{
		public static CrosshairManager Instance { get; private set; }

		[SerializeField, AssetsOnly]
		private GameObject Prefab;

		private GameObject _crosshair;

		private void Awake()
		{
			Instance = this;
			_crosshair = Prefab.Clone(transform);
		}

		private void LateUpdate()
		{
			Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_crosshair.transform.position = cursorPos;
		}

		public void Show()
		{
			if (!_crosshair.activeInHierarchy)
			{
				_crosshair.SetActive(true);
			}
		}

		public void Hide()
		{
			if (_crosshair.activeInHierarchy)
			{
				_crosshair.SetActive(false);
			}
		}
	}
}
