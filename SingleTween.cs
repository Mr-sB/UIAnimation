using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameUtil
{
    [Serializable]
    public class SingleTween
    {
        public enum ItemType
        {
            Tweener,
            Delay,
            Callback
        }
        
        public enum TweenType
        {
            Move,
            Rotate,
            Scale,
            Image,
            Text,
            TextMeshProUGUI,
            Canvas,
            AnchorPos3D,
        }
        
        public enum LinkType
        {
            Append,
            Join,
            Insert
        }

#if UNITY_EDITOR
        //Only for editor display
        [SerializeField] private string Name;
#endif
        public ItemType AddItemType;
        //Tweener/Delay
        public float Duration;
        //Tweener
        public bool UseCurve;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
        public Ease EaseType = Ease.OutQuad;
        //Tweener/Callback
        [FormerlySerializedAs("TweenerLinkType")]
        public LinkType ItemLinkType;
        public float AtPosition;
        //Tweener
        public TweenType Mode;
        //Callback
        public UnityEvent Callback;
        
        //Component
        public Transform Transform;
        public Image Image;
        public Text Text;
        public TextMeshProUGUI TextMeshProUGUI;
        public CanvasGroup CanvasGroup;
        public RectTransform RectTransform;
        //Status
        public bool OverrideStartStatus;
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Vector3 StartRotation;
        public Vector3 EndRotation;
        public Vector3 StartScale;
        public Vector3 EndScale;
        [Range(0,1)]
        public float StartAlpha;
        [Range(0,1)]
        public float EndAlpha;
        //Recover Status
        private Color mRecoverColor;
        private Vector3 mRecoverPos;
        private Vector3 mRecoverRotation;
        private Vector3 mRecoverScale;

        public bool IsValid { private set; get; }
        
        public void Bind(GameObject go)
        {
            IsValid = true;
            if (AddItemType != ItemType.Tweener) return;
            switch (Mode)
            {
                case TweenType.Move:
                    if (!Transform)
                        Transform = go.transform;
                    mRecoverPos = Transform.localPosition;
                    break;
                case TweenType.Rotate:
                    if (!Transform)
                        Transform = go.transform;
                    mRecoverRotation = Transform.localEulerAngles;
                    break;
                case TweenType.Scale:
                    if (!Transform)
                        Transform = go.transform;
                    mRecoverScale = Transform.localScale;
                    break;
                case TweenType.Image:
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
                case TweenType.Text:
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
                case TweenType.TextMeshProUGUI:
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
                case TweenType.Canvas:
                    if (!CanvasGroup)
                    {
                        CanvasGroup = go.GetComponent<CanvasGroup>();
                        if (!CanvasGroup)
                            CanvasGroup = go.AddComponent<CanvasGroup>();
                    }
                    mRecoverColor.a = CanvasGroup.alpha;
                    break;
                case TweenType.AnchorPos3D:
                    if (!RectTransform)
                    {
                        RectTransform = go.GetComponent<RectTransform>();
                        if (!RectTransform)
                        {
                            IsValid = false;
                            Debug.LogError("AnchorPosTween类型的UI动画，游戏物体上必须挂载RectTransform组件！");
                            return;
                        }
                    }
                    mRecoverPos = RectTransform.anchoredPosition3D;
                    break;
                default:
                    IsValid = false;
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }

        public void SetStartStatus()
        {
            if (AddItemType != ItemType.Tweener || !IsValid || !OverrideStartStatus) return;
            switch (Mode)
            {
                case TweenType.Move:
                    Transform.localPosition = StartPos;
                    break;
                case TweenType.Rotate:
                    Transform.localEulerAngles = StartRotation;
                    break;
                case TweenType.Scale:
                    Transform.localScale = StartScale;
                    break;
                case TweenType.Image:
                    var color = Image.color;
                    color.a = StartAlpha;
                    Image.color = color;
                    break;
                case TweenType.Text:
                    var color1 = Text.color;
                    color1.a = StartAlpha;
                    Text.color = color1;
                    break;
                case TweenType.TextMeshProUGUI:
                    var color2 = TextMeshProUGUI.color;
                    color2.a = StartAlpha;
                    TextMeshProUGUI.color = color2;
                    break;
                case TweenType.Canvas:
                    CanvasGroup.alpha = StartAlpha;
                    break;
                case TweenType.AnchorPos3D:
                    RectTransform.anchoredPosition3D = StartPos;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }
        
        public Tweener BuildTween()
        {
            if (AddItemType != ItemType.Tweener || !IsValid) return null;
            Tweener result;
            switch (Mode)
            {
                case TweenType.Move:
                    result = Transform.DOLocalMove(EndPos, Duration);
                    break;
                case TweenType.Rotate:
                    result = Transform.DOLocalRotate(EndRotation, Duration);
                    break;
                case TweenType.Scale:
                    result = Transform.DOScale(EndScale, Duration);
                    break;
                case TweenType.Image:
                    result = Image.DOFade(EndAlpha, Duration);
                    break;
                case TweenType.Text:
                    result = Text.DOFade(EndAlpha, Duration);
                    break;
                case TweenType.TextMeshProUGUI:
                    Color color = TextMeshProUGUI.color;
                    color.a = EndAlpha;
                    result = DOTween.To(() => TextMeshProUGUI.color, x => TextMeshProUGUI.color = x, color, Duration);
                    break;
                case TweenType.Canvas:
                    result = CanvasGroup.DOFade(EndAlpha, Duration);
                    break;
                case TweenType.AnchorPos3D:
                    result = RectTransform.DOAnchorPos3D(EndPos, Duration);
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
            }

            return result;
        }

        public void RecoverStatus()
        {
            if (AddItemType != ItemType.Tweener || !IsValid) return;
            switch (Mode)
            {
                case TweenType.Move:
                    Transform.localPosition = mRecoverPos;
                    break;
                case TweenType.Rotate:
                    Transform.localEulerAngles = mRecoverRotation;
                    break;
                case TweenType.Scale:
                    Transform.localScale = mRecoverScale;
                    break;
                case TweenType.Image:
                    Image.color = mRecoverColor;
                    break;
                case TweenType.Text:
                    Text.color = mRecoverColor;
                    break;
                case TweenType.TextMeshProUGUI:
                    TextMeshProUGUI.color = mRecoverColor;
                    break;
                case TweenType.Canvas:
                    CanvasGroup.alpha = mRecoverColor.a;
                    break;
                case TweenType.AnchorPos3D:
                    RectTransform.anchoredPosition3D = mRecoverPos;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }

        public void InvokeCallback()
        {
            if (AddItemType != ItemType.Callback) return;
            Callback?.Invoke();
        }

        public void CopyStartStatus(GameObject go)
        {
            if (AddItemType != ItemType.Tweener || !OverrideStartStatus)
            {
                Debug.LogWarning("No need Copy Start Status From Component.");
                return;
            }
            switch (Mode)
            {
                case TweenType.Move:
                    StartPos = (Transform != null ? Transform : go.transform).localPosition;
                    break;
                case TweenType.Rotate:
                    StartRotation = (Transform != null ? Transform : go.transform).localEulerAngles;
                    break;
                case TweenType.Scale:
                    StartScale = (Transform != null ? Transform : go.transform).localScale;
                    break;
                case TweenType.Image:
                    var image = Image;
                    if (!image)
                    {
                        image = go.GetComponent<Image>();
                        if (!image)
                        {
                            Debug.LogError("ImageTween类型的UI动画，游戏物体上必须挂载Image组件！");
                            return;
                        }
                    }
                    StartAlpha = image.color.a;
                    break;
                case TweenType.Text:
                    var text = Text;
                    if (!text)
                    {
                        text = go.GetComponent<Text>();
                        if (!text)
                        {
                            Debug.LogError("TextTween类型的UI动画，游戏物体上必须挂载Text组件！");
                            return;
                        }
                    }
                    StartAlpha = text.color.a;
                    break;
                case TweenType.TextMeshProUGUI:
                    var textMeshProUGUI = TextMeshProUGUI;
                    if (!textMeshProUGUI)
                    {
                        textMeshProUGUI = go.GetComponent<TextMeshProUGUI>();
                        if (!textMeshProUGUI)
                        {
                            Debug.LogError("TextMeshPropTween类型的UI动画，游戏物体上必须挂载TextMeshPropTween组件！");
                            return;
                        }
                    }
                    StartAlpha = textMeshProUGUI.color.a;
                    break;
                case TweenType.Canvas:
                    var canvasGroup = CanvasGroup;
                    if (!canvasGroup)
                    {
                        canvasGroup = go.GetComponent<CanvasGroup>();
                        if (!canvasGroup)
                            canvasGroup = go.AddComponent<CanvasGroup>();
                    }
                    StartAlpha = canvasGroup.alpha;
                    break;
                case TweenType.AnchorPos3D:
                    var rectTransform = RectTransform;
                    if (!rectTransform)
                    {
                        rectTransform = go.GetComponent<RectTransform>();
                        if (!rectTransform)
                        {
                            Debug.LogError("AnchorPosTween类型的UI动画，游戏物体上必须挂载RectTransform组件！");
                            return;
                        }
                    }
                    StartPos = rectTransform.anchoredPosition3D;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }

        public void PasteStartStatus(GameObject go)
        {
            if (AddItemType != ItemType.Tweener || !OverrideStartStatus)
            {
                Debug.LogWarning("Can not Paste Start Status To Component.");
                return;
            }

            Color color;
            Transform transform;
            switch (Mode)
            {
                case TweenType.Move:
                    transform = Transform != null ? Transform : go.transform;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(transform, transform.name);
#endif
                    transform.localPosition = StartPos;
                    break;
                case TweenType.Rotate:
                    transform = Transform != null ? Transform : go.transform;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(transform, transform.name);
#endif
                    transform.localEulerAngles = StartRotation;
                    break;
                case TweenType.Scale:
                    transform = Transform != null ? Transform : go.transform;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(transform, transform.name);
#endif
                    transform.localScale = StartScale;
                    break;
                case TweenType.Image:
                    var image = Image;
                    if (!image)
                    {
                        image = go.GetComponent<Image>();
                        if (!image)
                        {
                            Debug.LogError("ImageTween类型的UI动画，游戏物体上必须挂载Image组件！");
                            return;
                        }
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(image, image.name);
#endif
                    color = image.color;
                    color.a = StartAlpha;
                    image.color = color;
                    break;
                case TweenType.Text:
                    var text = Text;
                    if (!text)
                    {
                        text = go.GetComponent<Text>();
                        if (!text)
                        {
                            Debug.LogError("TextTween类型的UI动画，游戏物体上必须挂载Text组件！");
                            return;
                        }
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(text, text.name);
#endif
                    color = text.color;
                    color.a = StartAlpha;
                    text.color = color;
                    break;
                case TweenType.TextMeshProUGUI:
                    var textMeshProUGUI = TextMeshProUGUI;
                    if (!textMeshProUGUI)
                    {
                        textMeshProUGUI = go.GetComponent<TextMeshProUGUI>();
                        if (!textMeshProUGUI)
                        {
                            Debug.LogError("TextMeshPropTween类型的UI动画，游戏物体上必须挂载TextMeshPropTween组件！");
                            return;
                        }
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(textMeshProUGUI, textMeshProUGUI.name);
#endif
                    color = textMeshProUGUI.color;
                    color.a = StartAlpha;
                    textMeshProUGUI.color = color;
                    break;
                case TweenType.Canvas:
                    var canvasGroup = CanvasGroup;
                    if (!canvasGroup)
                    {
                        canvasGroup = go.GetComponent<CanvasGroup>();
                        if (!canvasGroup)
                            canvasGroup = go.AddComponent<CanvasGroup>();
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(canvasGroup, canvasGroup.name);
#endif
                    canvasGroup.alpha = StartAlpha;
                    break;
                case TweenType.AnchorPos3D:
                    var rectTransform = RectTransform;
                    if (!rectTransform)
                    {
                        rectTransform = go.GetComponent<RectTransform>();
                        if (!rectTransform)
                        {
                            Debug.LogError("AnchorPosTween类型的UI动画，游戏物体上必须挂载RectTransform组件！");
                            return;
                        }
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(rectTransform, rectTransform.name);
#endif
                    rectTransform.anchoredPosition3D = StartPos;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }
        
        public void CopyEndStatus(GameObject go)
        {
            if (AddItemType != ItemType.Tweener)
            {
                Debug.LogWarning("No need Copy End Status From Component.");
                return;
            }
            switch (Mode)
            {
                case TweenType.Move:
                    EndPos = (Transform != null ? Transform : go.transform).localPosition;
                    break;
                case TweenType.Rotate:
                    EndRotation = (Transform != null ? Transform : go.transform).localEulerAngles;
                    break;
                case TweenType.Scale:
                    EndScale = (Transform != null ? Transform : go.transform).localScale;
                    break;
                case TweenType.Image:
                    var image = Image;
                    if (!image)
                    {
                        image = go.GetComponent<Image>();
                        if (!image)
                        {
                            Debug.LogError("ImageTween类型的UI动画，游戏物体上必须挂载Image组件！");
                            return;
                        }
                    }
                    EndAlpha = image.color.a;
                    break;
                case TweenType.Text:
                    var text = Text;
                    if (!text)
                    {
                        text = go.GetComponent<Text>();
                        if (!text)
                        {
                            Debug.LogError("TextTween类型的UI动画，游戏物体上必须挂载Text组件！");
                            return;
                        }
                    }
                    EndAlpha = text.color.a;
                    break;
                case TweenType.TextMeshProUGUI:
                    var textMeshProUGUI = TextMeshProUGUI;
                    if (!textMeshProUGUI)
                    {
                        textMeshProUGUI = go.GetComponent<TextMeshProUGUI>();
                        if (!textMeshProUGUI)
                        {
                            Debug.LogError("TextMeshPropTween类型的UI动画，游戏物体上必须挂载TextMeshPropTween组件！");
                            return;
                        }
                    }
                    EndAlpha = textMeshProUGUI.color.a;
                    break;
                case TweenType.Canvas:
                    var canvasGroup = CanvasGroup;
                    if (!canvasGroup)
                    {
                        canvasGroup = go.GetComponent<CanvasGroup>();
                        if (!canvasGroup)
                            canvasGroup = go.AddComponent<CanvasGroup>();
                    }
                    EndAlpha = canvasGroup.alpha;
                    break;
                case TweenType.AnchorPos3D:
                    var rectTransform = RectTransform;
                    if (!rectTransform)
                    {
                        rectTransform = go.GetComponent<RectTransform>();
                        if (!rectTransform)
                        {
                            Debug.LogError("AnchorPosTween类型的UI动画，游戏物体上必须挂载RectTransform组件！");
                            return;
                        }
                    }
                    EndPos = rectTransform.anchoredPosition3D;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }

        public void PasteEndStatus(GameObject go)
        {
            if (AddItemType != ItemType.Tweener)
            {
                Debug.LogWarning("Can not Paste End Status To Component.");
                return;
            }

            Color color;
            Transform transform;
            switch (Mode)
            {
                case TweenType.Move:
                    transform = Transform != null ? Transform : go.transform;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(transform, transform.name);
#endif
                    transform.localPosition = EndPos;
                    break;
                case TweenType.Rotate:
                    transform = Transform != null ? Transform : go.transform;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(transform, transform.name);
#endif
                    transform.localEulerAngles = EndRotation;
                    break;
                case TweenType.Scale:
                    transform = Transform != null ? Transform : go.transform;
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(transform, transform.name);
#endif
                    transform.localScale = EndScale;
                    break;
                case TweenType.Image:
                    var image = Image;
                    if (!image)
                    {
                        image = go.GetComponent<Image>();
                        if (!image)
                        {
                            Debug.LogError("ImageTween类型的UI动画，游戏物体上必须挂载Image组件！");
                            return;
                        }
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(image, image.name);
#endif
                    color = image.color;
                    color.a = EndAlpha;
                    image.color = color;
                    break;
                case TweenType.Text:
                    var text = Text;
                    if (!text)
                    {
                        text = go.GetComponent<Text>();
                        if (!text)
                        {
                            Debug.LogError("TextTween类型的UI动画，游戏物体上必须挂载Text组件！");
                            return;
                        }
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(text, text.name);
#endif
                    color = text.color;
                    color.a = EndAlpha;
                    text.color = color;
                    break;
                case TweenType.TextMeshProUGUI:
                    var textMeshProUGUI = TextMeshProUGUI;
                    if (!textMeshProUGUI)
                    {
                        textMeshProUGUI = go.GetComponent<TextMeshProUGUI>();
                        if (!textMeshProUGUI)
                        {
                            Debug.LogError("TextMeshPropTween类型的UI动画，游戏物体上必须挂载TextMeshPropTween组件！");
                            return;
                        }
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(textMeshProUGUI, textMeshProUGUI.name);
#endif
                    color = textMeshProUGUI.color;
                    color.a = EndAlpha;
                    textMeshProUGUI.color = color;
                    break;
                case TweenType.Canvas:
                    var canvasGroup = CanvasGroup;
                    if (!canvasGroup)
                    {
                        canvasGroup = go.GetComponent<CanvasGroup>();
                        if (!canvasGroup)
                            canvasGroup = go.AddComponent<CanvasGroup>();
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(canvasGroup, canvasGroup.name);
#endif
                    canvasGroup.alpha = EndAlpha;
                    break;
                case TweenType.AnchorPos3D:
                    var rectTransform = RectTransform;
                    if (!rectTransform)
                    {
                        rectTransform = go.GetComponent<RectTransform>();
                        if (!rectTransform)
                        {
                            Debug.LogError("AnchorPosTween类型的UI动画，游戏物体上必须挂载RectTransform组件！");
                            return;
                        }
                    }
#if UNITY_EDITOR
                    UnityEditor.Undo.RecordObject(rectTransform, rectTransform.name);
#endif
                    rectTransform.anchoredPosition3D = EndPos;
                    break;
                default:
                    Debug.LogError("不存在的UI动画类型！");
                    break;
            }
        }
    }
}