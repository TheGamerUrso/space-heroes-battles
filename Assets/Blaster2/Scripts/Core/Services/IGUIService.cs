using UnityEngine;

public interface IGUIService
{
    bool IsMenuOpen { get; }
    void QuitButton();
    void PauseButton();
}
