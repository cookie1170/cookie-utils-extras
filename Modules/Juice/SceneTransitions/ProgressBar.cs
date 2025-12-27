using System;
using UnityEngine;
using UnityEngine.UI;

namespace CookieUtils.Extras.Juice
{
    public class ProgressBar : MonoBehaviour, IProgress<float>
    {
        [SerializeField]
        private Image _image;

        public Image Image
        {
            get
            {
                if (_image)
                    return _image;

                _image = GetComponentInChildren<Image>();
                return _image;
            }
        }

        private void Reset()
        {
            if (!_image)
                _image = GetComponentInChildren<Image>();

            if (_image)
                _image.fillMethod = Image.FillMethod.Horizontal;
        }

        public void Report(float value)
        {
            Image.fillAmount = value;
        }
    }
}
