using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionInputData", menuName = "InputData/InteractionInputData", order = 0)]
public class InteractionInputData : ScriptableObject {
    [SerializeField] private InputProvider inputProvider = null;

    [Space] [SerializeField] private bool interactionClicked;
    [SerializeField] private bool interactionReleased;

    [Space] public bool isInteracting;
    public float holdTimer;

    private bool InteractClicked {
        get => interactionClicked;
        set => interactionClicked = value;
    }

    private bool InteractReleased {
        get => interactionReleased;
        set => interactionReleased = value;
    }

    public void OnEnable() {
        inputProvider.InteractionStartedEvent += OnInteractionClicked;
        inputProvider.InteractionCancelledEvent += OnInteractionReleased;
    }

    public void OnDisable() {
        inputProvider.InteractionStartedEvent -= OnInteractionClicked;
        inputProvider.InteractionCancelledEvent -= OnInteractionReleased;
    }

    private void OnInteractionClicked() {
        InteractClicked = true;
        InteractReleased = false;
        isInteracting = true;
        holdTimer = 0;
    }

    private void OnInteractionReleased() {
        InteractClicked = false;
        InteractReleased = true;
        isInteracting = false;
        holdTimer = 0;
    }

    public void EnableGameplayInput() => inputProvider.EnableGameplayInput();

    public void Reset() {
        interactionClicked = false;
        interactionReleased = false;
    }
}