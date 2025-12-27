using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CookieUtils.Extras.SceneManager;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace CookieUtils.Extras.Juice
{
    public abstract class FullscreenTransition : SceneTransition
    {
        [SerializeField]
        private Graphic screen;

        protected Graphic Screen
        {
            get
            {
                if (screen)
                    return screen;

                screen = GetComponentInChildren<Graphic>();
                return screen;
            }
        }

        [SerializeField]
        protected ProgressBar progressBar;

        [SerializeField]
        protected List<Graphic> progressBarImages;

        [SerializeField]
        protected TweenSettings<float> progressBarShowSettings = new()
        {
            endValue = 1,
            startFromCurrent = true,
            settings = new TweenSettings
            {
                duration = 0.3f,
                ease = Ease.InQuad,
                useUnscaledTime = true,
            },
        };

        [SerializeField]
        protected TweenSettings<float> progressBarHideSettings = new()
        {
            endValue = 0,
            startFromCurrent = true,
            settings = new TweenSettings
            {
                duration = 0.3f,
                endDelay = 0.1f,
                ease = Ease.OutQuad,
                useUnscaledTime = true,
            },
        };

        public override IProgress<float> Progress => progressBar ? progressBar : null;

        protected virtual void Awake()
        {
            if (!screen)
                screen = GetComponentInChildren<Graphic>();

            if (!progressBar)
                progressBar = GetComponentInChildren<ProgressBar>();
        }

        protected virtual void Reset()
        {
            if (!screen)
                screen = GetComponentInChildren<Graphic>();

            if (!progressBar)
                progressBar = GetComponentInChildren<ProgressBar>();
        }

        protected abstract Task OnShow();
        protected abstract Task OnHide();

        public override Task Show()
        {
            Tween.StopAll(this);
            return OnShow();
        }

        public override Task Hide()
        {
            Tween.StopAll(this);
            return OnHide();
        }

        protected virtual Sequence ShowProgressBar()
        {
            Sequence seq = Sequence.Create(useUnscaledTime: true);

            foreach (Graphic img in progressBarImages)
            {
                seq.InsertCallback(0, img, (i) => i.enabled = true);
            }

            foreach (Graphic img in progressBarImages)
            {
                seq.Insert(0, Tween.Alpha(img, progressBarShowSettings));
            }

            return seq;
        }

        protected virtual Sequence HideProgressBar()
        {
            Sequence seq = Sequence.Create(useUnscaledTime: true);

            foreach (Graphic img in progressBarImages)
            {
                seq.Insert(0, Tween.Alpha(img, progressBarHideSettings));
            }

            foreach (Graphic img in progressBarImages)
            {
                seq.ChainCallback(img, (i) => i.enabled = true);
            }

            return seq;
        }

        protected virtual void ShowProgressBarImmediately()
        {
            foreach (Graphic img in progressBarImages)
            {
                img.enabled = true;
                img.color = img.color.SetAlpha(progressBarShowSettings.endValue);
            }
        }

        protected virtual void HideProgressBarImmediately()
        {
            foreach (Graphic img in progressBarImages)
            {
                img.color = img.color.SetAlpha(progressBarHideSettings.endValue);
                img.enabled = false;
            }
        }
    }
}
