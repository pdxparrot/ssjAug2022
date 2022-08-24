using Godot;

using System;

namespace pdxpartyparrot.ssjAug2022.Interactables
{
    public interface IInteractable
    {
        bool CanInteract { get; }

        // this can return GetType() if that makes sense
        // but subclasses often need to return a common base class here
        Type InteractableType { get; }
    }
}
