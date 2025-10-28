using CookieUtils.HealthSystem;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Samples.Juice
{
    public class SampleSquare : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private CookieUtils.HealthSystem.Health _health;
        private bool _isHovered;
        private SpriteRenderer _sprite;
        private float _timeSinceSpawn = 0;

        private void Awake() {
            _health = GetComponent<CookieUtils.HealthSystem.Health>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void Update() {
            _timeSinceSpawn += Time.deltaTime;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (_timeSinceSpawn > 0.2f)
                _health.Hit(
                    new CookieUtils.HealthSystem.Health.AttackInfo(
                        new Hitbox.HitboxInfo(20, 0.2f, Vector3.right), eventData.pointerPressRaycast.worldPosition
                    )
                );
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Tween.Color(_sprite, _sprite.color, new Color(0.95f, 0.76f, 0.8f), 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            Tween.Color(_sprite, _sprite.color, Color.white, 0.2f);
        }
    }
}