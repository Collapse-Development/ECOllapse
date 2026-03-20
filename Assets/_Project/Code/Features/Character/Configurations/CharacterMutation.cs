using UnityEngine;
using _Project.Code.Features.Character.MB;


public abstract class CharacterMutation : ScriptableObject, ICharacterMutation
{
    [Header("Основные параметры мутации")]
    public string mutationName; // название для отображения
    [TextArea(3, 5)]
    public string description; // описание мутации
    
    [Header("Игровые параметры")]
    public int cost = 1; // стоимость в очках мутаций
    public int rarity = 0; // редкость: 0 - обычная, 1 - редкая, 2 - эпическая
    
    [Header("Развитие")]
    public CharacterMutation nextUpgrade; // следующая ступень улучшения
    public abstract void Apply(CharacterBuildConfig cfg);
}
