using DG.Tweening;
using Quinn.PlayerSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Quinn.UI
{
	public class PlayerHealthUI : MonoBehaviour
	{
		[SerializeField, Required]
		private CanvasGroup Group;
		[SerializeField, Required]
		private Slider HPBar;

		private void Awake()
		{
			Group.alpha = 0f;
		}

		private void Start()
		{
			Player.Instance.Health.OnDamage += _ =>
			{
				if (Group.alpha <= 0f)
				{
					Group.DOKill();
					Group.DOFade(1f, 0.3f);
				}
			};

			Player.Instance.Health.OnHeal += _ =>
			{
				if (Group.alpha >= 1f)
				{
					Group.DOKill();
					Group.DOFade(0f, 1f);
				}
			};
		}

		private void Update()
		{
			float hp = Player.Instance.Health.Normalized;
			HPBar.value = hp;
		}
	}
}
