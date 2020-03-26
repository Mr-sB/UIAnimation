using DG.Tweening;
using UnityEngine;

namespace GameUtil
{
    public class FlashAnim : MonoBehaviour
    {
        [SerializeField] private RectTransform m_RTFBound;
        [SerializeField] private RectTransform m_RTFLine;
        [SerializeField] private float mMoveSpeed = 500;
        private Sequence mSequence;
        private float mPosX;
        private float mMoveTime;

        private void Awake()
        {
            mPosX = (m_RTFBound.rect.width + m_RTFLine.rect.width) / 2;
            var startPos = m_RTFLine.localPosition;
            startPos.x = -mPosX;
            m_RTFLine.localPosition = startPos;
            mMoveTime = mPosX * 2 / mMoveSpeed;
            CreateAnimation();
        }

        private void CreateAnimation()
        {
            mSequence = DOTween.Sequence()
                .Append(m_RTFLine.DOLocalMoveX(mPosX, mMoveTime).SetEase(Ease.Linear))
                .AppendInterval(1.8f)
                .SetLoops(-1, LoopType.Restart)
                .SetUpdate(true)
                .Pause();
        }

        public void Play()
        {
            gameObject.SetActive(true);
            if (!mSequence.IsPlaying())
            {
                mSequence.Play();
            }
        }

        public void Stop()
        {
            gameObject.SetActive(false);
            if (mSequence.IsPlaying())
            {
                mSequence.Pause();
            }
        }

        private void OnDestroy()
        {
            mSequence.Kill();
        }
    }
}