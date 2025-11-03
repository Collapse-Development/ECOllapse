using Unity.Cinemachine;
using UnityEngine;
using _Project.Code.Features.Player.MB;
using _Project.Code.Features.Character.MB;
public class CameraController : MonoBehaviour 
{ 
    [SerializeField] private Player _player; 

    private CinemachineCamera _virtualCamera; 
    private void Start() 
    { 
        if (_player.Character != null) 
        { 
            _virtualCamera.Follow = _player.Character.transform; 
            _virtualCamera.LookAt = _player.Character.transform; 
        } 
    } 
    private void Awake() 
    { 
        _virtualCamera = GetComponent<CinemachineCamera>(); 
    } 
    private void OnEnable() 
    { 
        if (_player != null) 
        { 
            _player.OnCharacterUpdated += OnCharacterUpdated; 
        } 
    } 
    private void OnDisable() 
    { 
        if (_player != null) 
        { 
            _player.OnCharacterUpdated -= OnCharacterUpdated; 
        } 
    } 
    private void OnCharacterUpdated(Character oldCharacter, Character newCharacter) 
    { 
        Debug.Log("Camera following: " + newCharacter); 
        if (newCharacter == null) 
            return; 
        
        _virtualCamera.Follow = newCharacter.transform; 
        _virtualCamera.LookAt = newCharacter.transform; 
    } 
}