using CharacterSystems;
using UnityEngine;


namespace _Project.Code.Features.Character.Configurations.Systems
{
    [CreateAssetMenu(fileName = "New CharacterModelSystemConfig", menuName = "Scriptable Objects/Character/Systems/Model/CharacterModelSystem")]
    public class CharacterModelSystemConfig : CharacterSystemConfig<CharacterModelSystem>
    {
        public string PrefabPath;
    }
}