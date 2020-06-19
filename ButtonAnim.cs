using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameUtil
{
    public class ButtonAnim : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Serializable]
        public class TweenSetting
        {
            public float Duration;
            public bool UseCurve;
            public AnimationCurve Curve;
            public Ease EaseType;
            public Vector3 Scale;
            
            public TweenSetting() { }

            public TweenSetting(float duration, bool useCurve, AnimationCurve curve, Ease easeType, Vector3 scale)
            {
                Duration = duration;
                UseCurve = useCurve;
                Curve = curve;
                EaseType = easeType;
                Scale = scale;
            }
        }
        
        [SerializeField] private Transform m_ScaleTransform;
        [SerializeField] private Button m_Button;
        public bool RelativeScale = true;
        public TweenSetting DownSetting = new TweenSetting(0.1f, false, AnimationCurve.Linear(0,0,1,1), Ease.OutQuad, new Vector3(0.95f, 0.95f, 1));
        public TweenSetting UpSetting = new TweenSetting(0.1f, false, AnimationCurve.Linear(0,0,1,1), Ease.OutQuad, new Vector3(1.02f, 1.02f, 1));
        public bool NeedSecondUpSetting = true;
        public TweenSetting SecondUpSetting = new TweenSetting(0.08f, false, AnimationCurve.Linear(0,0,1,1), Ease.OutQuad, new Vector3(1, 1, 1));
        
        private Vector3 mStartScale;
        private Tween mTween;
        private bool mIsPress;
        private bool mIsStay;
        private bool mIsEnable = true;

        private bool mInteractable => !m_Button || m_Button.gameObject.activeSelf && m_Button.enabled && m_Button.interactable;

        private void Awake()
        {
            if (!m_Button)
                m_Button = gameObject.GetComponent<Button>();
            if (!m_ScaleTransform)
                m_ScaleTransform = transform;

            mStartScale = m_ScaleTransform.localScale;
        }

        public void SetAnimEnable(bool enable)
        {
            if (mIsEnable == enable) return;
            mIsEnable = enable;
            if (mIsEnable) return;
            Recover();
        }
        
        public void Recover()
        {
            KillTween();
            m_ScaleTransform.localScale = mStartScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (mIsPress || !mIsEnable || !mInteractable) return;
            mIsPress = true;
            DownAnim();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!mIsPress || !mIsEnable) return;
            mIsPress = false;
            if (mIsStay) UpAnim();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!mIsEnable || !mInteractable) return;
            mIsStay = true;
            if (mIsPress) DownAnim();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!mIsPress || !mIsEnable || !mIsStay) return;
            mIsStay = false;
            UpAnim();
        }

        public void DownAnim()
        {
            AnimInternal(DownSetting);
        }

        public void UpAnim()
        {
            AnimInternal(UpSetting);
            if (NeedSecondUpSetting)
                mTween.OnComplete(() => { AnimInternal(SecondUpSetting); });
        }

        private void AnimInternal(TweenSetting setting)
        {
            KillTween();
            mTween = m_ScaleTransform.DOScale(RelativeScale ? Vector3.Scale(setting.Scale, mStartScale) : setting.Scale, setting.Duration).SetUpdate(true);
            if (setting.UseCurve)
                mTween.SetEase(setting.Curve);
            else
                mTween.SetEase(setting.EaseType);
        }

        private void KillTween()
        {
            mTween?.Kill();
            mTween = null;
        }
    }
}