

using System;

namespace pdxpartyparrot.ssjAug2022.Interactables
{
    public class InteractableEventArgs : EventArgs
    {
        public IInteractable Interactable { get; set; }
    }
}
