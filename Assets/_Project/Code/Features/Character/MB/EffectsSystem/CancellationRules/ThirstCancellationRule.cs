using _Project.Code.Features.Character.MB.EffectsSystem;
using _Project.Code.Features.Character.MB.Thirst;
using System;
using UnityEngine;

public class ThirstCancellationRule : ICharacterEffectCancellationRule
{
    public event Action CancelRequired;

    private readonly ICharacterThirstSystem _thirstSystem;

    public ThirstCancellationRule(ICharacterThirstSystem thirstSystem)
    {
        _thirstSystem = thirstSystem;
    }

    public void Tick(float dt)
    {
        if (_thirstSystem.CurrentValue > 0f)
        {
            CancelRequired?.Invoke();
        }
    }
}
