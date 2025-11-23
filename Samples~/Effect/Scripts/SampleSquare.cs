using CookieUtils.Extras.Juice;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Samples.Juice
{
    public class SampleSquare : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private EffectPlayer hurtEffect;
        [SerializeField] private EffectPlayer deathEffect;
        private int _health = 100;

        private bool _isHovered;
        private SpriteRenderer _sprite;
        private float _timeSinceSpawn = 0;

        private void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void Update() {
            _timeSinceSpawn += Time.deltaTime;
        }

        public async void OnPointerClick(PointerEventData eventData) {
            if (_timeSinceSpawn <= 0.2f) return;

            _health -= 20;
            if (_health <= 0) {
                await deathEffect.Play(Vector3.right, eventData.pointerCurrentRaycast.worldPosition);
                Destroy(gameObject);

                return;
            }

            await hurtEffect.Play(Vector3.right, eventData.pointerCurrentRaycast.worldPosition);
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Tween.Color(_sprite, _sprite.color, new Color(0.95f, 0.76f, 0.8f), 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            Tween.Color(_sprite, _sprite.color, Color.white, 0.2f);
        }
    }
}