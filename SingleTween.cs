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
        public bool OverrideStartStatus;
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Vector3 StartScale;
        public Vector3 EndScale;
        public float StartAlpha;
        public float EndAlpha;
        //Component
        private Image m_Image;
        private Text m_Text;
        private TextMeshProUGUI m_TextMeshProUGUI;
        private Transform m_Transform;
        private CanvasGroup m_CanvasGroup;
        //Status
        private Color mRecoverColor;
        private Vector3 mRecoverPos;
        private Vector3 mRecoverScale;
        
        public void Bind(GameObject go)
        {
            switch (Mode)
            {
                case TweenType.ImageTween:
                    m_Image = go.GetComponent<Image>();
                    if (m_Image == null)
                    {
                        Debug.LogError("ImageTween类型的UI动画，游戏物体上必须挂载Image组件！");
                        return;
                    }

                    mRecoverColor = m_Image.color;
                    break;
                case TweenType.TextTween:
                    m_Text = go.GetComponent<Text>();
                    if (m_Text == null)
                    {
                        Debug.LogError("TextTween类型的UI动画，游戏物体上必须挂载Text组件！");
                        return;
                    }

                    mRecoverColor = m_Text.color;
                    break;
                case TweenType.TextMeshPropTween:
                    m_TextMeshProUGUI = go.GetComponent<TextMeshProUGUI>();
                    if (m_TextMeshProUGUI == null)
                    {
                        Debug.LogError("TTextMeshPropTween类型的UI动画，游戏物体上必须挂载TextMeshPropTween组件！");
                        return;
                    }

                    mRecoverColor = m_TextMeshProUGUI.color;
                    break;
                case TweenType.MoveTween:
                    m_Transform = go.transform;
                    mRecoverPos = m_Transform.localPosition;
                    break;
                case TweenType.ScaleTween:
                    m_Transform = go.transform;
                    mRecoverScale = m_Transform.localScale;
                    break;
                case TweenType.CanvasTween:
                    m_CanvasGroup = go.GetComponent<CanvasGroup>();
                    if (m_CanvasGroup == null)
                    {
                        m_CanvasGroup = go.AddComponent<CanvasGroup>();
                    }

                    mRecoverColor.a = m_CanvasGroup.alpha;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }

        public void SetStartStatus()
        {
            if(!OverrideStartStatus) return;
            switch (Mode)
            {
                case TweenType.ImageTween:
                    m_Image.color = new Color(m_Image.color.r, m_Image.color.g, m_Image.color.b, StartAlpha);
                    break;
                case TweenType.TextTween:
                    m_Text.color = new Color(m_Text.color.r, m_Text.color.g, m_Text.color.b, StartAlpha);
                    break;
                case TweenType.TextMeshPropTween:
                    m_TextMeshProUGUI.color = new Color(m_TextMeshProUGUI.color.r, m_TextMeshProUGUI.color.g, m_TextMeshProUGUI.color.b, StartAlpha);;
                    break;
                case TweenType.MoveTween:
                    m_Transform.localPosition = StartPos;
                    break;
                case TweenType.ScaleTween:
                    m_Transform.localScale = StartScale;
                    break;
                case TweenType.CanvasTween:
                    m_CanvasGroup.alpha = StartAlpha;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }
        
        public Tweener BuildTween()
        {
            Tweener result;
            switch (Mode)
            {
                case TweenType.ImageTween:
                    result = m_Image.DOFade(EndAlpha < 0 ? 1 : EndAlpha, Duration);
                    break;
                case TweenType.TextTween:
                    result = m_Text.DOFade(EndAlpha < 0 ? 1 : EndAlpha, Duration);
                    break;
                case TweenType.TextMeshPropTween:
                    Color color = new Color(m_TextMeshProUGUI.color.r, m_TextMeshProUGUI.color.g, m_TextMeshProUGUI.color.b, EndAlpha);
                    result = DOTween.To(() => m_TextMeshProUGUI.color, x => m_TextMeshProUGUI.color = x, color, Duration);
                    break;
                case TweenType.MoveTween:
                    result = m_Transform.DOLocalMove(EndPos, Duration, false);
                    break;
                case TweenType.ScaleTween:
                    result = m_Transform.DOScale(EndScale, Duration);
                    break;
                case TweenType.CanvasTween:
                    result = m_CanvasGroup.DOFade(EndAlpha < 0 ? 1 : EndAlpha, Duration);
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
            switch (Mode)
            {
                case TweenType.ImageTween:
                    m_Image.color = mRecoverColor;
                    break;
                case TweenType.TextTween:
                    m_Text.color = mRecoverColor;
                    break;
                case TweenType.TextMeshPropTween:
                    m_TextMeshProUGUI.color = mRecoverColor;
                    break;
                case TweenType.MoveTween:
                    m_Transform.localPosition = mRecoverPos;
                    break;
                case TweenType.ScaleTween:
                    m_Transform.localScale = mRecoverScale;
                    break;
                case TweenType.CanvasTween:
                    m_CanvasGroup.alpha = mRecoverColor.a;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }
    }
}