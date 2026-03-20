using System.Collections;
using UnityEngine;

namespace _Project.Code.Features.Character.MB.Model
{
    /// <summary>
    /// ����������� �������� ��� � IK:
    ///  - ��������� ������� ����� ������� (������� ����),
    ///  - ���������� ��� ��� ���������� ������,
    ///  - ������� ������ �� ��������,
    ///  - ���������� IK � OnAnimatorIK.
    /// ��������� ������ ������ �� ��� �� GO, ��� � CharacterModel + Animator.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterModel))]
    [RequireComponent(typeof(Animator))]
    public class CharacterModelAnimationSystem : MonoBehaviour, ICharacterModelAnimationSystem
    {
        [Header("������")]
        [SerializeField] private Transform hips;               // ����� �����/���
        [SerializeField] private Transform leftFootHome;       // "���" ����� ���� (����� ��� hip'�� �����)
        [SerializeField] private Transform rightFootHome;      // "���" ������ ����
        [SerializeField] private float homeOffsetForward = 0.2f; // �������� ����� ����� ������������ ��������

        [Header("���")]
        [SerializeField, Min(0f)] private float stepDistance = 0.35f; // ���������, ��� ������� ���������� ����� ���
        [SerializeField, Min(0f)] private float stepHeight = 0.12f;   // ������ ����
        [SerializeField, Min(0f)] private float stepDuration = 0.18f; // ������������ �������� ����
        [SerializeField] private float feetSpacing = 0.18f;           // ���������� ���������� ����� ������
        [SerializeField] private float maxSlopeAngle = 45f;           // ����. ���� �����������

        [Header("�����")]
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private float raycastUp = 0.6f;
        [SerializeField] private float raycastDown = 1.2f;

        [Header("�������� � ����")]
        [SerializeField] private float hipsFollow = 8f;        // ��������� ������ ��� ������� �� ������� ������ ����
        [SerializeField] private float turnLerp = 10f;         // �������� �������� ������� �� ������� ��������

        private CharacterModel _model;
        private Animator _anim;

        // ������� ������� ��������� ������� (� ����)
        private Vector3 _leftFootPos, _rightFootPos;
        private Vector3 _leftFootNormal = Vector3.up, _rightFootNormal = Vector3.up;

        // ����� ����
        private bool _isLeftStepping, _isRightStepping;
        private bool _ikEnabled = true;

        // ���� �������� (� ������� ����������� ������, �/�)
        private Vector3 _localMoveVelocity;

        // ��������� API
        public void SetMoveVelocity(Vector3 velocity) => _localMoveVelocity = velocity;
        public void SetGroundMask(LayerMask mask) => groundMask = mask;
        public void SetEnabled(bool enabled) => _ikEnabled = enabled;

        private void Awake()
        {
            _model = GetComponent<CharacterModel>();
            _anim = GetComponent<Animator>();

            // ����������� � CharacterModel
            _model.TryRegisterSystem<ICharacterModelAnimationSystem>(this);

            // ��������� ��������� ������� ��� � �� "�����"
            _leftFootPos  = leftFootHome.position;
            _rightFootPos = rightFootHome.position;
        }

        private void Update()
        {
            // 1) ������� ������� � ����������� �������� (�����)
            var worldVel = transform.TransformVector(_localMoveVelocity);
            var flat = new Vector3(worldVel.x, 0f, worldVel.z);
            if (flat.sqrMagnitude > 0.0001f)
            {
                var targetRot = Quaternion.LookRotation(flat.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-turnLerp * UnityEngine.Time.deltaTime));
            }

            // 2) ��������� "��������" ����� ����� ��� ������� forward
            var forward = transform.forward;
            var right = transform.right;

            var leftHomePos  = hips.position + forward * homeOffsetForward - right * feetSpacing * 0.5f;
            var rightHomePos = hips.position + forward * homeOffsetForward + right * feetSpacing * 0.5f;

            // 3) ���������� ��� ����� �� ����� ���������
            var leftGround  = ProbeGround(leftHomePos);
            var rightGround = ProbeGround(rightHomePos);

            // 4) ��������� ��������� �� ������� ����� �� �������� ����� � ���������� ���
            if (!_isLeftStepping && DistanceOnPlane(_leftFootPos, leftGround.point) > stepDistance && !_isRightStepping)
            {
                StartCoroutine(StepFoot(
                    isLeft: true,
                    fromPos: _leftFootPos,
                    toPos: leftGround.point,
                    toNormal: leftGround.normal
                ));
            }

            if (!_isRightStepping && DistanceOnPlane(_rightFootPos, rightGround.point) > stepDistance && !_isLeftStepping)
            {
                StartCoroutine(StepFoot(
                    isLeft: false,
                    fromPos: _rightFootPos,
                    toPos: rightGround.point,
                    toNormal: rightGround.normal
                ));
            }

            // 5) ���������� ��� �� ������� ������ ����� (���� �� ��������)
            var feetMid = 0.5f * (_leftFootPos + _rightFootPos);
            hips.position = Vector3.Lerp(hips.position, new Vector3(feetMid.x, hips.position.y, feetMid.z),
                1f - Mathf.Exp(-hipsFollow * UnityEngine.Time.deltaTime));
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!_ikEnabled || _anim == null) return;

            // �������� IK � ����� �������/������� (���������� ������ �� ������� �����������)
            _anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            _anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            _anim.SetIKPosition(AvatarIKGoal.LeftFoot, _leftFootPos);
            _anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(transform.up, _leftFootNormal) * transform.rotation);

            _anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            _anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
            _anim.SetIKPosition(AvatarIKGoal.RightFoot, _rightFootPos);
            _anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(transform.up, _rightFootNormal) * transform.rotation);
        }

        private (Vector3 point, Vector3 normal) ProbeGround(Vector3 around)
        {
            var origin = new Vector3(around.x, around.y + raycastUp, around.z);
            var dir = Vector3.down;
            if (Physics.Raycast(origin, dir, out var hit, raycastUp + raycastDown, groundMask, QueryTriggerInteraction.Ignore))
            {
                var angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle <= maxSlopeAngle)
                    return (hit.point, hit.normal);
            }
            // ���� �� ����� � ������ ������� ��������, ������ ���������� ������� �� ������ hips.y - raycastDown/2
            var fallback = new Vector3(around.x, hips.position.y - 0.5f * raycastDown, around.z);
            return (fallback, Vector3.up);
        }

        private IEnumerator StepFoot(bool isLeft, Vector3 fromPos, Vector3 toPos, Vector3 toNormal)
        {
            if (isLeft) _isLeftStepping = true; else _isRightStepping = true;

            float t = 0f;
            while (t < 1f)
            {
                // Строка 160 - исправлено
                t += UnityEngine.Time.deltaTime / Mathf.Max(0.0001f, stepDuration);
                var p = Parabola(fromPos, toPos, stepHeight, t);

                if (isLeft)
                {
                    _leftFootPos = p;
                    _leftFootNormal = Vector3.Slerp(_leftFootNormal, toNormal, t);
                }
                else
                {
                    _rightFootPos = p;
                    _rightFootNormal = Vector3.Slerp(_rightFootNormal, toNormal, t);
                }
                yield return null;
            }

            if (isLeft)
            {
                _leftFootPos = toPos;
                _leftFootNormal = toNormal;
                _isLeftStepping = false;
            }
            else
            {
                _rightFootPos = toPos;
                _rightFootNormal = toNormal;
                _isRightStepping = false;
            }
        }

        // �������� ��� �������� ����
        private static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            t = Mathf.Clamp01(t);
            var mid = Vector3.Lerp(start, end, t);
            float yOffset = 4f * height * t * (1f - t); // ������������ ����
            return new Vector3(mid.x, Mathf.Max(start.y, end.y) + yOffset, mid.z);
        }

        // �������������� ��������� (���������� Y)
        private static float DistanceOnPlane(Vector3 a, Vector3 b)
        {
            a.y = 0f; b.y = 0f;
            return Vector3.Distance(a, b);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(_leftFootPos, 0.025f);
            Gizmos.DrawSphere(_rightFootPos, 0.025f);
        }
#endif
    }
}
