# Pause menu setup

Pause is toggled with **Escape**. The pause panel has **Resume**, **Restart**, and **Quit**. Same pattern as Game Over and Upgrade UI.

## Editor setup

1. **Canvas**
   - On the same Canvas that has Game Over / Upgrade UI (or your main game Canvas), create a **child GameObject** (e.g. `PausePanel`).
   - Leave it **inactive** (unchecked in the Inspector) so the game doesn’t start paused.

2. **Panel layout**
   - Add a **Panel** (UI → Panel) or a **Image** as background so the game is dimmed when paused.
   - Add a **Text** (e.g. "Paused") at the top if you want a title.
   - Add three **Buttons**:
     - **Resume** – closes the pause menu and resumes the game.
     - **Restart** – reloads the current scene (same as Game Over “New Game”).
     - **Quit** – quits the application (in the Editor, stops Play mode).

3. **PauseUI script**
   - Add the **PauseUI** script to the **Canvas** (or the same GameObject that has UpgradeUI / GameOverUI).
   - In the Inspector, assign:
     - **Panel** → the pause panel GameObject (the one you leave inactive).
     - **Resume Button** → the Resume button.
     - **Restart Button** → the Restart button.
     - **Quit Button** → the Quit button.

4. **Behaviour**
   - **Escape** toggles pause. Pause does not open when the Game Over or Upgrade panel is already visible.
   - **Resume** (or Escape again) sets `Time.timeScale = 1` and hides the panel.
   - **Restart** reloads the active scene.
   - **Quit** calls `Application.Quit()` (and in the Editor, stops Play mode).

No extra input setup is required; the script uses `Input.GetKeyDown(KeyCode.Escape)`.
