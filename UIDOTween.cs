using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace GameUtil
{
    public class UIDOTween : MonoBehaviour
    {
        public enum TweenStatus
        {
            None,
            Start,
            Close
        }
        
        public bool PlayOnEnable = true;
        public UpdateType UpdateType = UpdateType.Normal;
        public bool IgnoreTimeScale = true;
        public SingleTween[] StartSingleTweens = new SingleTween[0];
        public SingleTween[] CloseSingleTweens = new SingleTween[0];

        public UnityEvent OnStartBeforeAnim;
        public UnityEvent OnCloseBeforeAnim;
        public UnityEvent OnStartAfterAnim;
        public UnityEvent OnCloseAfterAnim;
        
        private Sequence mSequence;
        public TweenStatus Status { private set; get; } = TweenStatus.None;
        
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
            Status = TweenStatus.Start;
            DoTweenInternal(StartSingleTweens, OnStartBeforeAnim, OnStartAfterAnim, action);
        }

        public void DoCloseTween(Action action)
        {
            Status = TweenStatus.Close;
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
            bool needCallImmediately = false;
            mSequence?.Kill();
            mSequence = null;
            beforeEvent?.Invoke();
            if (tweens.Length > 0)
            {
                mSequence = DOTween.Sequence().SetUpdate(UpdateType, IgnoreTimeScale);
                foreach (var tween in tweens)
                {
                    switch (tween.AddItemType)
                    {
                        case SingleTween.ItemType.Tweener:
                            if(!tween.IsValid) continue;
                            switch (tween.ItemLinkType)
                            {
                                case SingleTween.LinkType.Append:
                                    lastTweenInsertTime = mSequence.Duration(false);
                                    needCallImmediately = lastTweenInsertTime < 1e-4;
                                    if (tween.OverrideStartStatus)
                                    {
                                        if (needCallImmediately)
                                            tween.SetStartStatus();
                                        else
                                            mSequence.AppendCallback(tween.SetStartStatus);
                                    }

                                    mSequence.Append(tween.BuildTween());
                                    break;
                                case SingleTween.LinkType.Join:
                                    if (tween.OverrideStartStatus)
                                    {
                                        if (needCallImmediately)
                                            tween.SetStartStatus();
                                        else
                                            mSequence.InsertCallback(lastTweenInsertTime, tween.SetStartStatus);
                                    }

                                    mSequence.Join(tween.BuildTween());
                                    break;
                                case SingleTween.LinkType.Insert:
                                    lastTweenInsertTime = tween.AtPosition;
                                    needCallImmediately = lastTweenInsertTime < 1e-4;
                                    if (tween.OverrideStartStatus)
                                    {
                                        if (needCallImmediately)
                                            tween.SetStartStatus();
                                        else
                                            mSequence.InsertCallback(lastTweenInsertTime, tween.SetStartStatus);
                                    }

                                    mSequence.Insert(lastTweenInsertTime, tween.BuildTween());
                                    break;
                                default:
                                    Debug.LogError("LinkType does not contain an enumeration of this type: " + (int)tween.ItemLinkType);
                                    break;
                            }
                            break;
                        case SingleTween.ItemType.Delay:
                            if (tween.Duration > 0)
                                mSequence.AppendInterval(tween.Duration);
                            break;
                        case SingleTween.ItemType.Callback:
                            //Callback also will change lastTweenInsertTime
                            switch (tween.ItemLinkType)
                            {
                                case SingleTween.LinkType.Append:
                                    lastTweenInsertTime = mSequence.Duration(false);
                                    mSequence.AppendCallback(tween.InvokeCallback);
                                    break;
                                case SingleTween.LinkType.Join:
                                    mSequence.InsertCallback(lastTweenInsertTime, tween.InvokeCallback);
                                    break;
                                case SingleTween.LinkType.Insert:
                                    lastTweenInsertTime = tween.AtPosition;
                                    mSequence.InsertCallback(tween.AtPosition, tween.InvokeCallback);
                                    break;
                                default:
                                    Debug.LogError("LinkType does not contain an enumeration of this type: " + (int)tween.ItemLinkType);
                                    break;
                            }
                            break;
                        default:
                            Debug.LogError("ItemType does not contain an enumeration of this type: " + (int)tween.AddItemType);
                            break;
                    }
                }
                mSequence.OnComplete(() =>
                {
                    mSequence = null;
                    Status = TweenStatus.None;
                    afterEvent?.Invoke();
                    action?.Invoke();
                });
            }
            else
            {
                Status = TweenStatus.None;
                afterEvent?.Invoke();
                action?.Invoke();
            }
        }
        
        /// <param name="recalculation">whether recalculate duration</param>
        public float GetStartDuration(bool recalculation = false)
        {
            if(!recalculation && mSequence != null && Status == TweenStatus.Start)
                return mSequence.Duration(false);
            return GetDuration(StartSingleTweens);
        }
        
        /// <param name="recalculation">whether recalculate duration</param>
        public float GetCloseDuration(bool recalculation = false)
        {
            if(!recalculation && mSequence != null && Status == TweenStatus.Close)
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
                switch (tween.AddItemType)
                {
                    case SingleTween.ItemType.Tweener:
                        if (!tween.IsValid) continue;
                        switch (tween.ItemLinkType)
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
                            default:
                                Debug.LogError("LinkType does not contain an enumeration of this type: " + (int)tween.ItemLinkType);
                                break;
                        }
                        break;
                    case SingleTween.ItemType.Delay:
                        if (tween.Duration > 0)
                            duration += tween.Duration;
                        break;
                    case SingleTween.ItemType.Callback:
                        //For callback, only Insert maybe change duration.
                        switch (tween.ItemLinkType)
                        {
                            case SingleTween.LinkType.Append:
                                lastTweenInsertTime = duration;
                                break;
                            case SingleTween.LinkType.Insert:
                                lastTweenInsertTime = tween.AtPosition;
                                if (lastTweenInsertTime > duration)
                                    duration = lastTweenInsertTime;
                                break;
                            case SingleTween.LinkType.Join:
                                break;
                            default:
                                Debug.LogError("LinkType does not contain an enumeration of this type: " + (int)tween.ItemLinkType);
                                break;
                        }
                        break;
                    default:
                        Debug.LogError("ItemType does not contain an enumeration of this type: " + (int)tween.AddItemType);
                        break;
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