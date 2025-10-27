using UnityEngine;

namespace _Project.Code.Features.Character.MB.Model
{
    /// <summary>
    /// ��������� ������� ����������� �������� ������ ���������.
    /// </summary>
    public interface ICharacterModelAnimationSystem : ISystem
    {
        /// <summary>������ ������� �������� � ��������� ����������� ������ (�/�).</summary>
        void SetMoveVelocity(Vector3 velocity);

        /// <summary>������������� ��������� ����� �������� "�����".</summary>
        void SetGroundMask(LayerMask mask);

        /// <summary>��������/��������� IK (�� ������ �������).</summary>
        void SetEnabled(bool enabled);
    }
}
