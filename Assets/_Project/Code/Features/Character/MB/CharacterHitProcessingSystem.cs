using UnityEngine;

public interface ICharacterSystem
{
    void Initialize(Character character);
}

public interface IHitProcessingSystem : ICharacterSystem
{
    void ProcessHit(float damage);
}

[DisallowMultipleComponent]
public class CharacterHitProcessingSystem : MonoBehaviour, IHitProcessingSystem
{
    [SerializeField] private Character _character;

    private bool _isRegistered;

    private void Awake()
    {
        if (_character == null)
        {
            Debug.LogError("CharacterHitProcessingSystem: поле _character не назначено!", this);
            return;
        }

        _isRegistered = _character.TryRegisterSystem<IHitProcessingSystem>(this);
        if (!_isRegistered)
        {
            Debug.LogWarning("IHitProcessingSystem уже зарегистрирована для этого персонажа", this);
        }
    }

    public void Initialize(Character character)
    {
        // Подготовка системы при инициализации, если необходимо
    }

    public void ProcessHit(float damage)
    {
        if (_character == null)
        {
            Debug.LogWarning("Character не назначен в HitProcessingSystem");
            return;
        }

        var healthSystem = _character.GetSystem<IHealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.ApplyDamage(damage);
        }
        else
        {
            Debug.LogWarning("HealthSystem не найдена, урон не применён");
        }

        // Взаимодействие с другими системами персонажа можно добавить здесь
    }
}