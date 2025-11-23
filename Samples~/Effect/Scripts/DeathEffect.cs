using System.Collections.Generic;
using System.Threading.Tasks;
using CookieUtils.Extras.Juice;
using CookieUtils.ObjectPooling;
using PrimeTween;
using UnityEngine;

[CreateAssetMenu(fileName = "DeathEffect", menuName = "Scriptable Objects/DeathEffect")]
public class DeathEffect : Effect
{
    private static readonly int ProgressID = Shader.PropertyToID("_Progress");

    [SerializeField] private float particleDelay;
    [SerializeField] private GameObject particlePrefab;
    [SerializeField] private TweenSettings<Vector3> scaleSettings = new();
    [SerializeField] private ShakeSettings rotationShake = new();
    [SerializeField] private TweenSettings<float> flashSettings = new() { startFromCurrent = true };

    public override bool Is2D => true;

    public override async Task Play(Renderer[] renderers, GameObject host, Vector3 direction, Vector3 contactPoint) {
        SpawnParticles(contactPoint);
        await Task.WhenAll(Scale(host), Flash(renderers), Rotation(host));
    }

    private async void SpawnParticles(Vector3 contactPoint) {
        await Awaitable.WaitForSecondsAsync(particleDelay);
        particlePrefab.Get(contactPoint);
    }

    private async Task Rotation(GameObject host) {
        await Tween.ShakeLocalRotation(host.transform, rotationShake);
    }

    private async Task Flash(Renderer[] renderers) {
        List<Task> tasks = new(renderers.Length);
        foreach (Renderer renderer in renderers) tasks.Add(FlashRenderer(renderer));

        await Task.WhenAll(tasks);
    }

    private async Task FlashRenderer(Renderer renderer) {
        await Tween.MaterialProperty(renderer.material, ProgressID, flashSettings);
    }

    private async Task Scale(GameObject host) {
        await Tween.Scale(host.transform, scaleSettings);
    }
}