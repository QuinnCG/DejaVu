using Quinn.PlayerSystem;
using UnityEngine;

namespace Quinn
{
	public class FacePlayer : MonoBehaviour
	{
		private void LateUpdate()
		{
			Vector2 dir = transform.position.DirectionTo(Player.Instance.transform.position);

			var scale = transform.localScale;
			scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
			transform.localScale = scale;
		}
	}
}
