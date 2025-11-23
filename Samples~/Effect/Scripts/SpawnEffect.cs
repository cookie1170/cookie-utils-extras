using System.Collections.Generic;
using System.Threading.Tasks;
using CookieUtils.Extras.Juice;
using CookieUtils.ObjectPooling;
using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnEffect", menuName = "Scriptable Objects/SpawnEffect")]
public class SpawnEffect : Effect
{
    private static readonly int ProgressID = Shader.PropertyToID("_Progress");

    [SerializeField] private float particleDelay;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private TweenSettings<Vector3> scaleSettings = new();
    [SerializeField] private ShakeSettings shakeSettings = new();
    [SerializeField] private TweenSettings<float> flashInSettings = new() { startFromCurrent = true };
    [SerializeField] private TweenSettings<float> flashOutSettings = new() { startFromCurrent = true };

    public override bool Is2D => true;

    public override async Task Play(Renderer[] renderers, GameObject host, Vector3 direction, Vector3 contactPoint) {
        SpawnParticles(contactPoint);
        await Task.WhenAll(Scale(host), Flash(renderers), Shake(host));
    }

    private async void SpawnParticles(Vector3 contactPoint) {
        await Awaitable.WaitForSecondsAsync(particleDelay);
        particlePrefab.Get(contactPoint);
    }

    private async Task Shake(GameObject host) {
        await Tween.ShakeLocalRotation(host.transform, shakeSettings);
    }

    private async Task Flash(Renderer[] renderers) {
        List<Task> tasks = new(renderers.Length);
        foreach (Renderer renderer in renderers) tasks.Add(FlashRenderer(renderer));

        await Task.WhenAll(tasks);
    }

    private async Task FlashRenderer(Renderer renderer) {
        await Tween.MaterialProperty(renderer.material, ProgressID, flashInSettings);
        await Tween.MaterialProperty(renderer.material, ProgressID, flashOutSettings);
    }

    private async Task Scale(GameObject host) {
        host.transform.localScale = Vector3.zero;
        await Tween.Scale(host.transform, scaleSettings);
    }
}