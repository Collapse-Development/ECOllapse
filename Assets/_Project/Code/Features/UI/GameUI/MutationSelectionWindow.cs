using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MutationSelectionWindow : MonoBehaviour
{
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private GameObject _window;
    [SerializeField] private TMP_Text _firstDescription;
    [SerializeField] private TMP_Text _secondDescription;

    private CharacterMutation _firstMutation;
    private CharacterMutation _secondMutation;
    private Action<CharacterMutation> _onSelected;

    private void Awake()
    {
        SetWindowActive(false);

        if (_gameUI != null)
            _gameUI.OnInitialized += Initialize;
    }

    private void OnDestroy()
    {
        if (_gameUI != null)
            _gameUI.OnInitialized -= Initialize;
    }

    public void Show(
        IReadOnlyList<CharacterMutation> mutations,
        Action<CharacterMutation> onSelected)
    {
        _onSelected = onSelected;

        SelectMutations(mutations);
        SetDescription(_firstDescription, _firstMutation);
        SetDescription(_secondDescription, _secondMutation);
        SetWindowActive(true);
    }

    public void SelectFirst()
    {
        Select(_firstMutation);
    }

    public void SelectSecond()
    {
        Select(_secondMutation);
    }

    private void Initialize()
    {
        _gameUI.GameSceneContext.MutationSelectionWindow = this;
    }

    private void SelectMutations(IReadOnlyList<CharacterMutation> mutations)
    {
        _firstMutation = null;
        _secondMutation = null;

        if (mutations == null || mutations.Count == 0)
            return;

        var firstIndex = UnityEngine.Random.Range(0, mutations.Count);
        _firstMutation = mutations[firstIndex];

        if (mutations.Count == 1)
        {
            _secondMutation = mutations[firstIndex];
            return;
        }

        var secondIndex = firstIndex;
        while (secondIndex == firstIndex)
            secondIndex = UnityEngine.Random.Range(0, mutations.Count);

        _secondMutation = mutations[secondIndex];
    }

    private void Select(CharacterMutation mutation)
    {
        SetWindowActive(false);

        var onSelected = _onSelected;
        _onSelected = null;
        onSelected?.Invoke(mutation);
    }

    private static void SetDescription(TMP_Text text, CharacterMutation mutation)
    {
        if (text != null)
            text.text = mutation != null ? mutation.Description : string.Empty;
    }

    private void SetWindowActive(bool active)
    {
        if (_window != null && _window.activeSelf != active)
            _window.SetActive(active);
    }
}
