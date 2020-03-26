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
            mSequence?.Kill();
            mSequence = null;
            if (tweens.Length > 0)
            {
                mSequence = DOTween.Sequence();
                foreach (var tween in tweens)
                {
                    if (tween.Delay > 0)
                        mSequence.AppendInterval(tween.Delay);
                    mSequence.AppendCallback(tween.SetStartStatus).Append(tween.BuildTween());
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