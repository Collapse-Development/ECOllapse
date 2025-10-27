using UnityEngine;
using _Project.Code.Features.Character.MB.Model;

[RequireComponent(typeof(CharacterModel))]
public class SimpleMover : MonoBehaviour
{
    [Min(0f)] public float moveSpeed = 2.5f;

    private CharacterModel _model;
    private ICharacterModelAnimationSystem _animSys;

    private void Awake()
    {
        _model = GetComponent<CharacterModel>();
        _animSys = _model.GetSystem<ICharacterModelAnimationSystem>();
        if (_animSys == null)
            Debug.LogError("ICharacterModelAnimationSystem не найден. Убедись, что CharacterModelAnimationSystem есть на объекте.");
    }

    private void Update()
    {
        // постоянное движение вперёд в мировых координатах
        Vector3 worldVel = transform.forward * moveSpeed;
        transform.position += worldVel * Time.deltaTime;

        // передаём локальную скорость в систему анимации (для шагов/поворота корпуса)
        Vector3 localVel = transform.InverseTransformVector(worldVel); // будет (0,0,moveSpeed)
        _animSys?.SetMoveVelocity(localVel);
    }
}
