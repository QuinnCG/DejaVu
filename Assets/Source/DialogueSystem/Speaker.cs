using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

namespace Quinn.DamageSystem
{
	public class Speaker : MonoBehaviour
	{
		[SerializeField]
		private float CharacterInterval = 0.05f, LineInterval = 1f, LastLineWaitAddend = 1f;
		[SerializeField]
		private float FirstLineDelay;

		[SerializeField, Space]
		private float FadeInDuration = 0.1f;
		[SerializeField]
		private float FadeOutDuration = 0.2f;

		[SerializeField, Required, FoldoutGroup("References")]
		private CanvasGroup Group;
		[SerializeField, Required, FoldoutGroup("References")]
		private TextMeshProUGUI TextBlock;
		[SerializeField, Required, FoldoutGroup("References")]
		private EventReference WriteSound;

		[SerializeField, FoldoutGroup("Dialogue")]
		private bool WriteDialogueOnStart, CanBeRepeated = true;
		[SerializeField, FoldoutGroup("Dialogue"), Multiline]
		private string[] Lines;

		private bool _hasPlayedBuiltInDialogue;

		private void Awake()
		{
			Group.alpha = 0f;
		}

		private void Start()
		{
			if (WriteDialogueOnStart)
			{
				Speak();
			}
		}

		public void Speak(params string[] lines)
		{
			StopAllCoroutines();
			TextBlock.text = string.Empty;

			StartCoroutine(Sequence(lines));

			Group.DOKill();
			Group.DOFade(1f, FadeInDuration);
		}

		public void Speak()
		{
			if (!_hasPlayedBuiltInDialogue || CanBeRepeated)
			{
				_hasPlayedBuiltInDialogue = true;
				Speak(Lines);
			}
		}

		public void End()
		{
			StopAllCoroutines();
			Group.DOFade(0f, FadeOutDuration);
		}

		private IEnumerator Sequence(string[] lines)
		{
			yield return new WaitForSeconds(FirstLineDelay);

			var builder = new StringBuilder();

			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i];

				for (int j = 0; j < line.Length; j++)
				{
					builder.Append(line[j]);
					TextBlock.text = builder.ToString();

					Audio.Play(WriteSound, transform.position);
					yield return new WaitForSeconds(CharacterInterval);
				}

				yield return new WaitForSeconds(LineInterval);
			}

			yield return new WaitForSeconds(LastLineWaitAddend);
			End();
		}
	}
}
