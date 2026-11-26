# DECISIONS

## Week 11 — Assignment 10

- 2026-11-11 I asked Codex to help publish my existing Assignment 10 work to GitHub; the gameplay code was already in the project and was not written by Codex.
- 2026-11-11 The first HTTPS push failed because GitHub rejected the saved token. I updated the GitHub credential, then confirmed the push succeeded.
- 2026-11-11 I tested the level with two players; the first completed it in three attempts and the second in two.
- 2026-11-11 After the test, I changed the Charger wind-up from 1.0 to 0.5 seconds and the Shooter cooldown from 2.0 to 1.6 seconds, shortening the warning and the time between shots.
- 2026-11-11 I kept the enemy implementation and Level 4 changes in two commits, then merged the branch into `main` so this week's work is visible together.
## Week 12 — Assignment 11

- 2026-11-16 I moved player, projectile, hazard, boss, and Level 4 enemy balance values into ScriptableObject assets so I can tune them without editing gameplay code.
- 2026-11-16 I kept the survival timer on Level 2; the other levels use coin collection as their goal.
- 2026-11-16 I tested a malformed save file. The game loaded default data without crashing and wrote a new save.json.
- 2026-11-16 After returning to the main menu, ResetButton could target a destroyed duplicate SaveManager. I changed it to use SaveManager.Instance through MenuButtons and confirmed the reset works.

## Week 13 — Assignment 12

- 2026-11-24 I asked Codex to check which clips were already in the game and find the missing effects and a separate music track for the levels.
- 2026-11-24 The original enemy warning clips were longer than the wind-up. I kept short versions so players hear the cue before the attack.
- 2026-11-24 Codex first set the death cue to 0.8 seconds. I asked to shorten it to 0.2 seconds so the game restarts sooner.
- 2026-11-24 I made the Music and SFX sliders in Unity and asked Codex to connect them to separate mixer groups and save both values.
- 2026-11-24 I asked Codex to soften the music on pause and vary the pitch of repeated effects. The pause snapshot changes a separate group so it preserves the user's volume setting.
