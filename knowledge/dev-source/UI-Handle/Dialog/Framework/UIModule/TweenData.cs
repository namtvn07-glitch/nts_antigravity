using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace HenryLe.Scripts.UIModule
{
    [Serializable]
    public class TweenData
    {
        public Transform target;

        public TweenConfigSO configSO;

        public bool custom;

        public TweenConfig config;

        public Action OnCompleted;

        public bool UseCanvas = false;

        public bool IsLoop = false;

        private string _obj;

        public void SetupData(Action callback = null, string name = "")
        {
            HelperManager.Log("commmmmmmm star-" + name);
            _obj = name;
            if (configSO != null && !custom)
            {
                SetDataFromConfigSo();
            }
            OnCompleted = callback;
            OnConfigSOValueChanged();
        }

        private DG.Tweening.Sequence _seq;

        private void OnConfigSOValueChanged()
        {
            if (!target)
            {
                HelperManager.Log("commmmmmmm nulll_" + _obj);

                OnCompleted?.Invoke();
                return;
            }

            HelperManager.Log("commmmmmmm star-1" + _obj);

            _seq?.Kill();
            _seq = DOTween.Sequence();

            _seq.AppendInterval(config.delay);

            switch (config.tweenType)
            {
                case UITweenType.Active:
                    target.gameObject.SetActive(false);
                    _seq.AppendCallback(() => target.gameObject.SetActive(true));
                    break;

                case UITweenType.Scale:
                    if (UseCanvas)
                    {
                        var rect = target.GetComponent<RectTransform>();
                        if (!rect) break;
                        rect.localScale = Vector3.one * config.from;
                        _seq.Append(rect.DOScale(config.to, config.duration).SetEase(config.curve));
                    }
                    else
                    {
                        target.localScale = Vector3.one * config.from;
                        _seq.Append(target.DOScale(config.to, config.duration).SetEase(config.curve));
                    }
                    break;

                case UITweenType.Move:
                    if (UseCanvas)
                    {
                        var rect = target.GetComponent<RectTransform>();
                        if (!rect) break;
                        rect.anchoredPosition = config.mFrom;
                        _seq.Append(rect.DOAnchorPos(config.mTo, config.duration).SetEase(config.curve));
                    }
                    else
                    {
                        target.position = config.mFrom;
                        _seq.Append(target.DOMove(config.mTo, config.duration).SetEase(config.curve));
                    }
                    break;

                case UITweenType.FadeGroup:
                    var cg = target.GetComponent<CanvasGroup>() ?? target.AddComponent<CanvasGroup>();
                    cg.alpha = config.from;
                    _seq.Append(cg.DOFade(config.to, config.duration).SetEase(config.curve));
                    break;
            }

            if (IsLoop) _seq.SetLoops(-1, LoopType.Yoyo);

            _seq.OnComplete(() => OnCompleted?.Invoke());
            HelperManager.Log("commmmmmmm star-2" + _obj);
            _seq.SetUpdate(UpdateType.Normal, false);

            // optional: auto kill if target destroyed
            _seq.SetLink(target.gameObject).SetAutoKill();
        }

        private void SetDataFromConfigSo()
        {
            config = configSO.config;
        }
    }
}
