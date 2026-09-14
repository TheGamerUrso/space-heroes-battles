using TheGamerUrso.UI;
using UnityEngine;

public interface IGUIService
{
    bool IsMenuOpen { get; }
    void Register(UIView view, bool AffectCursor = false);
    void Unregister(UIView view, bool AffectCursor = false);
}
