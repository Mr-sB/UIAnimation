using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameUtil
{
	[RequireComponent(typeof(Slider))]
	public class SliderAnim : MonoBehaviour
	{
		private Slider m_Slider;
		public Slider Slider => m_Slider;

		public float value
		{
			get => Slider.value;
			set => ForceSetValue(value);
		}

		private float mEndValue;
		private float mCurValue;
		private Tween mTween;

		public static implicit operator Slider(SliderAnim anim)
		{
			return anim.Slider;
		}

		public static SliderAnim Get(Slider slider)
		{
			if (!slider) return null;
			var go = slider.gameObject;
			var anim = go.GetComponent<SliderAnim>();
			if (!anim)
				anim = go.AddComponent<SliderAnim>();
			return anim;
		}

		private void Awake()
		{
			m_Slider = gameObject.GetComponent<Slider>();
		}

		/// <param name="stayMax">true:会把大于1的整数最终值保留为1  反之为0</param>
		public void SetProgress(float endValue, float? startValue = null, float duration = 0.3f,
			TweenCallback onComplete = null, bool stayMax = false)
		{
			mTween?.Kill();
			var remain = Mathf.FloorToInt(mEndValue) - Mathf.FloorToInt(mCurValue);
			remain = remain > 0 ? remain : 0;
			mEndValue = remain + endValue;
			mCurValue = startValue ?? Slider.value;
			mTween = DOTween.To(() => mCurValue, x =>
				{
					mCurValue = x;
					SetValue(x, stayMax);
				}, mEndValue, duration)
				.OnComplete(onComplete)
				.SetEase(Ease.OutQuad)
				.SetUpdate(true);
		}

		/// <param name="stayMax">true:会把大于1的整数最终值保留为1  反之为0</param>
		private void SetValue(float value, bool stayMax)
		{
			if (value >= 1)
			{
				value = value % 1;
				if (stayMax)
					value = value < 1e-6 ? 1 : value;
			}

			Slider.value = value;
		}

		private void ForceSetValue(float value)
		{
			mTween?.Kill();
			SetValue(value, true);
		}
	}
}
