using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace GameUtil
{
    public class UIDOTween : MonoBehaviour
    {
        private enum TweenStatus
        {
            None,
            Start,
            Close
        }
        
        public bool PlayOnEnable = true;
        public SingleTween[] StartSingleTweens = new SingleTween[0];
        public SingleTween[] CloseSingleTweens = new SingleTween[0];

        [Tooltip("打开界面时调用")] public UnityEvent OnStartBeforeAnim;
        [Tooltip("关闭界面时调用")] public UnityEvent OnCloseBeforeAnim;
        [Tooltip("打开界面动画结束时调用")] public UnityEvent OnStartAfterAnim;
        [Tooltip("关闭界面动画结束时调用")] public UnityEvent OnCloseAfterAnim;
        
        private Sequence mSequence;
        private TweenStatus mTweenStatus = TweenStatus.None;
        
        private void Awake()
        {
            foreach (var tween in StartSingleTweens)
                tween.Bind(gameObject);

            foreach (var tween in CloseSingleTweens)
                tween.Bind(gameObject);
        }
        
        private void OnEnable()
        {
            if (PlayOnEnable)
                DoStartTween();
        }

        public void DoStartTween(Action action)
        {
            RecoverStatus();
            mTweenStatus = TweenStatus.Start;
            DoTweenInternal(StartSingleTweens, OnStartBeforeAnim, OnStartAfterAnim, action);
        }

        public void DoCloseTween(Action action)
        {
            mTweenStatus = TweenStatus.Close;
            DoTweenInternal(CloseSingleTweens, OnCloseBeforeAnim, OnCloseAfterAnim, action);
        }
        
        [ContextMenu("DoStartTween")]
        public void DoStartTween()
        {
            DoStartTween(null);
        }

        [ContextMenu("DoCloseTween")]
        public void DoCloseTween()
        {
            DoCloseTween(null);
        }
        
        private void DoTweenInternal(SingleTween[] tweens, UnityEvent beforeEvent, UnityEvent afterEvent, Action action)
        {
            float lastTweenInsertTime = 0;
            mSequence?.Kill();
            mSequence = null;
            beforeEvent?.Invoke();
            if (tweens.Length > 0)
            {
                mSequence = DOTween.Sequence();
                foreach (var tween in tweens)
                {
                    if (tween.IsDelay && tween.Delay > 0)
                        mSequence.AppendInterval(tween.Delay);
                    else
                    {
                        if(!tween.IsValid) continue;
                        switch (tween.TweenerLinkType)
                        {
                            case SingleTween.LinkType.Append:
                                lastTweenInsertTime = mSequence.Duration(false);
                                if (tween.OverrideStartStatus)
                                    mSequence.AppendCallback(tween.SetStartStatus);
                                mSequence.Append(tween.BuildTween());
                                break;
                            case SingleTween.LinkType.Join:
                                if (tween.OverrideStartStatus)
                                    mSequence.InsertCallback(lastTweenInsertTime, tween.SetStartStatus);
                                mSequence.Join(tween.BuildTween());
                                break;
                            case SingleTween.LinkType.Insert:
                                lastTweenInsertTime = tween.AtPosition;
                                if (tween.OverrideStartStatus)
                                    mSequence.InsertCallback(lastTweenInsertTime, tween.SetStartStatus);
                                mSequence.Insert(lastTweenInsertTime, tween.BuildTween());
                                break;
                        }
                    }
                }

                mSequence.AppendCallback(() =>
                {
                    mSequence = null;
                    mTweenStatus = TweenStatus.None;
                    afterEvent?.Invoke();
                    action?.Invoke();
                });
            }
            else
            {
                mTweenStatus = TweenStatus.None;
                afterEvent?.Invoke();
                action?.Invoke();
            }
        }
        
        /// <param name="recalculation">whether recalculate duration</param>
        public float GetStartDuration(bool recalculation = false)
        {
            if(!recalculation && mSequence != null && mTweenStatus == TweenStatus.Start)
                return mSequence.Duration(false);
            return GetDuration(StartSingleTweens);
        }
        
        /// <param name="recalculation">whether recalculate duration</param>
        public float GetCloseDuration(bool recalculation = false)
        {
            if(!recalculation && mSequence != null && mTweenStatus == TweenStatus.Close)
                return mSequence.Duration(false);
            return GetDuration(CloseSingleTweens);
        }
        
        private static float GetDuration(SingleTween[] tweens)
        {
            if (tweens == null || tweens.Length <= 0) return 0;
            float duration = 0;
            float lastTweenInsertTime = 0;
            foreach (var tween in tweens)
            {
                if (tween.IsDelay && tween.Delay > 0)
                    duration += tween.Delay;
                else
                {
                    if (!tween.IsValid) continue;
                    switch (tween.TweenerLinkType)
                    {
                        case SingleTween.LinkType.Append:
                            lastTweenInsertTime = duration;
                            duration += tween.Duration;
                            break;
                        case SingleTween.LinkType.Insert:
                            lastTweenInsertTime = tween.AtPosition;
                            goto case SingleTween.LinkType.Join;
                        case SingleTween.LinkType.Join:
                            float newDuration = lastTweenInsertTime + tween.Duration;
                            if (newDuration > duration)
                                duration = newDuration;
                            break;
                    }
                }
            }
            return duration;
        }

        private void RecoverStatus()
        {
            foreach (var tween in CloseSingleTweens)
                tween.RecoverStatus();
            
            foreach (var tween in StartSingleTweens)
                tween.RecoverStatus();
        }
    }
}