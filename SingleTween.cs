using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameUtil
{
    [Serializable]
    public class SingleTween
    {
        public enum TweenType
        {
            MoveTween,
            ScaleTween,
            ImageTween,
            TextTween,
            TextMeshPropTween,
            CanvasTween,
        }
        
        public enum LinkType
        {
            Append,
            Join,
            Insert
        }

        [Tooltip("Special. Do nothing except delay")]
        public bool IsDelay;
        public float Delay;
        public float Duration;
        public bool UseCurve;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
        public Ease EaseType = Ease.OutQuad;
        public LinkType TweenerLinkType;
        public float AtPosition;
        public TweenType Mode;
        //Component
        public Transform Transform;
        public Image Image;
        public Text Text;
        public TextMeshProUGUI TextMeshProUGUI;
        public CanvasGroup CanvasGroup;
        //Status
        public bool OverrideStartStatus;
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Vector3 StartScale;
        public Vector3 EndScale;
        [Range(0,1)]
        public float StartAlpha;
        [Range(0,1)]
        public float EndAlpha;
        //Recover Status
        private Color mRecoverColor;
        private Vector3 mRecoverPos;
        private Vector3 mRecoverScale;

        public bool IsValid { private set; get; }
        
        public void Bind(GameObject go)
        {
            IsValid = true;
            if(IsDelay) return;
            switch (Mode)
            {
                case TweenType.MoveTween:
                    if(!Transform)
                        Transform = go.transform;
                    mRecoverPos = Transform.localPosition;
                    break;
                case TweenType.ScaleTween:
                    if(!Transform)
                        Transform = go.transform;
                    mRecoverScale = Transform.localScale;
                    break;
                case TweenType.ImageTween:
                    if (!Image)
                    {
                        Image = go.GetComponent<Image>();
                        if (!Image)
                        {
                            IsValid = false;
                            Debug.LogError("ImageTween类型的UI动画，游戏物体上必须挂载Image组件！");
                            return;
                        }
                    }
                    mRecoverColor = Image.color;
                    break;
                case TweenType.TextTween:
                    if (!Text)
                    {
                        Text = go.GetComponent<Text>();
                        if (!Text)
                        {
                            IsValid = false;
                            Debug.LogError("TextTween类型的UI动画，游戏物体上必须挂载Text组件！");
                            return;
                        }
                    }
                    mRecoverColor = Text.color;
                    break;
                case TweenType.TextMeshPropTween:
                    if (!TextMeshProUGUI)
                    {
                        TextMeshProUGUI = go.GetComponent<TextMeshProUGUI>();
                        if (!TextMeshProUGUI)
                        {
                            IsValid = false;
                            Debug.LogError("TextMeshPropTween类型的UI动画，游戏物体上必须挂载TextMeshPropTween组件！");
                            return;
                        }
                    }
                    mRecoverColor = TextMeshProUGUI.color;
                    break;
                case TweenType.CanvasTween:
                    if (!CanvasGroup)
                    {
                        CanvasGroup = go.GetComponent<CanvasGroup>();
                        if (!CanvasGroup)
                            CanvasGroup = go.AddComponent<CanvasGroup>();
                    }
                    mRecoverColor.a = CanvasGroup.alpha;
                    break;
                default:
                    IsValid = false;
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }

        public void SetStartStatus()
        {
            if(IsDelay || !IsValid || !OverrideStartStatus) return;
            switch (Mode)
            {
                case TweenType.ImageTween:
                    var color = Image.color;
                    color.a = StartAlpha;
                    Image.color = color;
                    break;
                case TweenType.TextTween:
                    var color1 = Text.color;
                    color1.a = StartAlpha;
                    Text.color = color1;
                    break;
                case TweenType.TextMeshPropTween:
                    var color2 = TextMeshProUGUI.color;
                    color2.a = StartAlpha;
                    TextMeshProUGUI.color = color2;
                    break;
                case TweenType.MoveTween:
                    Transform.localPosition = StartPos;
                    break;
                case TweenType.ScaleTween:
                    Transform.localScale = StartScale;
                    break;
                case TweenType.CanvasTween:
                    CanvasGroup.alpha = StartAlpha;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }
        
        public Tweener BuildTween()
        {
            if (IsDelay || !IsValid) return null;
            Tweener result;
            switch (Mode)
            {
                case TweenType.MoveTween:
                    result = Transform.DOLocalMove(EndPos, Duration, false);
                    break;
                case TweenType.ScaleTween:
                    result = Transform.DOScale(EndScale, Duration);
                    break;
                case TweenType.ImageTween:
                    result = Image.DOFade(EndAlpha < 0 ? 1 : EndAlpha, Duration);
                    break;
                case TweenType.TextTween:
                    result = Text.DOFade(EndAlpha < 0 ? 1 : EndAlpha, Duration);
                    break;
                case TweenType.TextMeshPropTween:
                    Color color = TextMeshProUGUI.color;
                    color.a = EndAlpha;
                    result = DOTween.To(() => TextMeshProUGUI.color, x => TextMeshProUGUI.color = x, color, Duration);
                    break;
                case TweenType.CanvasTween:
                    result = CanvasGroup.DOFade(EndAlpha < 0 ? 1 : EndAlpha, Duration);
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    result = null;
                    break;
            }

            if (result != null)
            {
                if (UseCurve)
                    result.SetEase(Curve);
                else
                    result.SetEase(EaseType);
                result.SetUpdate(true);
            }

            return result;
        }

        public void RecoverStatus()
        {
            if (IsDelay || !IsValid) return;
            switch (Mode)
            {
                case TweenType.MoveTween:
                    Transform.localPosition = mRecoverPos;
                    break;
                case TweenType.ScaleTween:
                    Transform.localScale = mRecoverScale;
                    break;
                case TweenType.ImageTween:
                    Image.color = mRecoverColor;
                    break;
                case TweenType.TextTween:
                    Text.color = mRecoverColor;
                    break;
                case TweenType.TextMeshPropTween:
                    TextMeshProUGUI.color = mRecoverColor;
                    break;
                case TweenType.CanvasTween:
                    CanvasGroup.alpha = mRecoverColor.a;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }
    }
}