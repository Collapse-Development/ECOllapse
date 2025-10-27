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
            Debug.LogError("ICharacterModelAnimationSystem �� ������. �������, ��� CharacterModelAnimationSystem ���� �� �������.");
    }

    private void Update()
    {
        // ���������� �������� ����� � ������� �����������
        Vector3 worldVel = transform.forward * moveSpeed;
        transform.position += worldVel * Time.deltaTime;

        // ������� ��������� �������� � ������� �������� (��� �����/�������� �������)
        Vector3 localVel = transform.InverseTransformVector(worldVel); // ����� (0,0,moveSpeed)
        _animSys?.SetMoveVelocity(localVel);
    }
}
