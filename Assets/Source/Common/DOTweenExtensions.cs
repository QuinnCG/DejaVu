using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Quinn
{
	public static class DOTweenExtensions
	{
		public static TweenerCore<float, float, FloatOptions> DOAnimateFloat(this Material material, string propertyName, float endValue, float duration)
		{
			var tween = DOTween.To(() => material.GetFloat(propertyName), x => material.SetFloat(propertyName, x), endValue, duration);
			tween.target = material;
			tween.SetTarget(material);
			return tween;
		}

		public static TweenerCore<float, float, FloatOptions> DOFade(this Light2D light, float endValue, float duration)
		{
			var tween = DOTween.To(() => light.intensity, x => light.intensity = x, endValue, duration);
			tween.SetTarget(light);
			tween.target = light;
			return tween;
		}

		public static TweenerCore<float, float, FloatOptions> DORotateZ(this Transform transform, float endValue, float duration)
		{
			return DOTween.To(() =>
			{
				return transform.rotation.z;
			}, x =>
			{
				transform.rotation = Quaternion.AngleAxis(x, Vector3.forward);
			}, endValue, duration)
				.SetTarget(transform);
		}
		public static TweenerCore<float, float, FloatOptions> DOLocalRotateZ(this Transform transform, float endValue, float duration)
		{
			return DOTween.To(() =>
			{
				return transform.localRotation.z;
			}, x =>
			{
				transform.localRotation = Quaternion.AngleAxis(x, Vector3.forward);
			}, endValue, duration)
				.SetTarget(transform);
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalScale(this Transform transform, Vector3 endValue, float duration)
		{
			return DOTween.To(() => transform.localScale, x => transform.localScale = x, endValue, duration)
				.SetTarget(transform);
		}
	}
}
