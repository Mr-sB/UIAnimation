using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace GameUtil
{
    public class UIDOTween : MonoBehaviour
    {
        public bool PlayOnEnable = true;
        public SingleTween[] StartSingleTweens = new SingleTween[0];
        public SingleTween[] EndSingleTweens = new SingleTween[0];
        
        public bool IsExecuteStartAction = true;
        [Tooltip("打开界面动画结束时调用")] public UnityEvent StartEvent;

        public bool IsExecuteCloseAction = true;
        [Tooltip("关闭界面动画结束时调用")] public UnityEvent CloseEvent;

        [Tooltip("打开界面时调用")] public UnityEvent OnStart;
        [Tooltip("关闭界面时调用")] public UnityEvent OnClose;
        
        private Sequence mSequence;
        
        private void Awake()
        {
            foreach (var tween in StartSingleTweens)
                tween.Bind(gameObject);

            foreach (var tween in EndSingleTweens)
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
            DoTweenInternal(StartSingleTweens, OnStart, IsExecuteStartAction, StartEvent, action);
        }

        public void DoCloseTween(Action action)
        {
            DoTweenInternal(EndSingleTweens, OnClose, IsExecuteCloseAction, CloseEvent, action);
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

        private void DoTweenInternal(SingleTween[] tweens, UnityEvent beforeEvent, bool isExecute, UnityEvent afterEvent, Action action)
        {
            float lastTweenInsertTime = 0;
            mSequence?.Kill();
            mSequence = null;
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
                    if (!isExecute) return;
                    mSequence = null;
                    action?.Invoke();
                    afterEvent?.Invoke();
                });
            }
            else if (isExecute)
            {
                action?.Invoke();
                afterEvent?.Invoke();
            }

            beforeEvent?.Invoke();
        }

        private void RecoverStatus()
        {
            foreach (var tween in EndSingleTweens)
                tween.RecoverStatus();
            
            foreach (var tween in StartSingleTweens)
                tween.RecoverStatus();
        }
    }
}