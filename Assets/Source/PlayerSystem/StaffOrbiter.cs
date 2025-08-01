using Quinn.DamageSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class StaffOrbiter : MonoBehaviour
	{
		[SerializeField, Required, ChildGameObjectsOnly]
		private Transform Staff;
		[SerializeField, Required]
		private Transform Pivot;
		[SerializeField]
		private float Distance = 0.5f;

		private Grabber _grabber;

		private void Awake()
		{
			_grabber = GetComponent<Grabber>();

			GetComponent<Health>().OnDeath += () =>
			{
				Staff.gameObject.SetActive(false);
			};

			Staff.transform.SetParent(null, true);
		}

		private void OnDestroy()
		{
			if (Staff != null)
			{
				Staff.gameObject.Destroy();
			}
		}

		private void LateUpdate()
		{
			Vector2 dir = Pivot.position.DirectionTo(CrosshairManager.Instance.Position);

			if (_grabber.IsGrabbing)
			{
				dir = Pivot.position.DirectionTo(_grabber.GrabPosition);
			}

			Vector2 pos = (Vector2)Pivot.position + (dir * Distance);
			Quaternion rot = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, Vector3.forward);

			Staff.SetPositionAndRotation(pos, rot);

			var scale = Staff.localScale;
			scale.x = scale.y = Mathf.Sign(dir.x);
			Staff.localScale = scale;
		}
	}
}
