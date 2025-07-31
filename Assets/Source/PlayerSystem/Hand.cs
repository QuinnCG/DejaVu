using Sirenix.OdinInspector;
using UnityEngine;

namespace Quinn.PlayerSystem
{
	public class Hand : MonoBehaviour
	{
		[SerializeField]
		private AnimationCurve LineThicknessChangeOverDistance;
		[SerializeField]
		private float LineFrequencyFactor = 1f;
		[SerializeField]
		private AnimationCurve LineAmpFactor;
		[SerializeField]
		private float LineAmpMultiplier = 1f;
		[SerializeField]
		private int MinVertices = 12, MaxVertices = 32;

		[Space]

		[SerializeField, AssetsOnly]
		private GameObject HandPrefab;
		[SerializeField, Required]
		private Sprite OpenHand, ClosedHand;

		private LineRenderer _line;
		private GameObject _handInstance;
		private SpriteRenderer _handRenderer;

		private void Awake()
		{
			_line = GetComponent<LineRenderer>();
		}

		private void OnDestroy()
		{
			DestroyHand();
		}

		public void UpdateHand(Vector2 start, Vector2 end, Quaternion rotation, float normExtensionProgress, float normDstToEnd, bool isFist = true)
		{
			if (_handInstance == null)
			{
				_handInstance = HandPrefab.Clone(start);
				_handRenderer = _handInstance.GetComponent<SpriteRenderer>();
			}

			_handRenderer.sprite = isFist ? ClosedHand : OpenHand;

			UpdateHandTransform(start, end, normExtensionProgress, rotation);
			UpdateLineData(normExtensionProgress, normDstToEnd, start, end);
		}

		public void DestroyHand()
		{
			if (_handInstance != null)
			{
				_handInstance.Destroy();
				_handInstance = null;

				_line.positionCount = 0;
			}
		}

		private void UpdateHandTransform(Vector2 start, Vector2 end, float normExtensionProgress, Quaternion rot)
		{
			var pos = Vector2.Lerp(start, end, normExtensionProgress);
			_handInstance.transform.SetPositionAndRotation(pos, rot);
		}

		private void UpdateLineData(float normExtensionProgress, float normDstToEnd, Vector2 start, Vector2 end)
		{
			int vertexCount = Mathf.RoundToInt(Mathf.Lerp(MinVertices, MaxVertices, normDstToEnd));
			vertexCount = MaxVertices;
			var vertices = new Vector3[vertexCount];

			Vector2 dir = start.DirectionTo(end);
			var perpDir = new Vector3(-dir.y, dir.x);

			for (int i = 0; i < vertices.Length; i++)
			{
				float t = i / (float)(vertices.Length - 1);
				Vector3 basePos = Vector3.Lerp(start, _handInstance.transform.position, t);

				float offsetMultiplier = LineAmpFactor.Evaluate(t) * LineAmpMultiplier;
				offsetMultiplier *= 1f - normExtensionProgress;

				Vector3 offset = Mathf.Sin(Mathf.PI * LineFrequencyFactor) * offsetMultiplier * perpDir;

				vertices[i] = basePos + offset;
			}

			_line.positionCount = vertices.Length;
			_line.SetPositions(vertices);
			_line.widthCurve = GetLineWidthCurve(normDstToEnd);
		}

		private AnimationCurve GetLineWidthCurve(float normProgress)
		{
			float t = LineThicknessChangeOverDistance.Evaluate(normProgress);

			var start = new Keyframe(0f, 1f)
			{
				outTangent = Mathf.Lerp(3f, -5f, t)
			};

			var mid = new Keyframe(0.5f, Mathf.Lerp(2f, 0.4f, t))
			{
				inTangent = 0f,
				outTangent = 0f
			};

			var end = new Keyframe(1f, 1f)
			{
				inTangent = Mathf.Lerp(-3f, 5f, t)
			};

			return new AnimationCurve(start, mid, end);
		}
	}
}
