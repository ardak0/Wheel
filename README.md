# Wheel of Fortune — Unity Demo

A mobile "wheel of fortune" prize loop built in Unity, inspired by the Critical
Strike card-game reward flow. Each zone the player spins a wheel to grow their
banked rewards. One slice is a **bomb** that wipes everything. Every **5th** zone
is a risk-free **silver** spin and every **30th** is a **golden** super spin — on
those safe zones the player may **cash out** and keep what they have.

---

## Requirements

- **Unity 2021.3.45f2** (LTS). The exact version is pinned in
  `ProjectSettings/ProjectVersion.txt`.
- **Android Build Support** module (with OpenJDK + Android SDK/NDK) if you want
  to build the APK.
- **DOTween** drives the spin tween and is vendored in the repo under
  `Assets/Plugins/Demigiant/DOTween` — no separate install needed.

## Opening the project

> **Git LFS required.** Binary assets (DOTween's DLLs, all sprites, the
> TextMesh Pro font) are stored with [Git LFS](https://git-lfs.com). After
> cloning, run `git lfs install` then `git lfs pull` so the real binaries
> replace the LFS pointer files. Skipping this leaves DOTween.dll as a stub and
> Unity opens in **Safe Mode** with `DG.Tweening.Core` compile errors.

1. Clone the repo, then from the repo root run `git lfs install && git lfs pull`.
2. Open **Unity Hub** and click **Add ▸ Add project from disk**.
3. Select the **`Wheel`** folder itself — the one that directly contains
   `Assets/`, `Packages/`, and `ProjectSettings/`. Selecting a parent folder is
   the usual cause of Unity Hub showing *"No projects found."*
4. Open the project with **2021.3.45f2**. On first open, install the
   **Addressables** package (see below) so the optional asset-loading path
   compiles in.

## Running

1. Open `Assets/Scenes/SampleScene.unity`.
2. Press **Play**.
3. Use the on-screen **Spin** and **Leave** buttons. Buttons are wired in code
   (no inspector `OnClick` entries); references auto-bind via `OnValidate`.

## How it plays

| Zone cadence        | Wheel tier | Bomb? | Can cash out? |
|---------------------|------------|-------|---------------|
| Normal zones        | Bronze     | Yes   | No            |
| Every 5th zone      | Silver     | No    | Yes           |
| Every 30th zone     | Golden     | No    | Yes           |

Rewards scale with zone depth via an `AnimationCurve` on each wheel config. The
golden rule takes precedence over silver when a zone is divisible by both.

## Architecture

The code is split into three layers, each its own assembly definition so
dependencies flow one way and the pure logic can be unit-tested in isolation:

```
Assets/Scripts/
├── Data/      (WheelDemo.Data)     ScriptableObjects: WheelConfigSO, RewardDefinitionSO, SliceEntry, WheelTier
├── Core/      (WheelDemo.Core)     Pure C#: ZoneRules, GameStateMachine, SlicePicker, RewardInventory
├── Gameplay/  (Assembly-CSharp)    GameController + view MonoBehaviours, ComponentPool
├── Loading/   (WheelDemo.Loading)  IconLoader facade + optional Addressables backend (gated)
└── Editor/    (WheelDemo.Editor)   ImageRaycastCleaner editor tool
```

`GameController` owns the game flow; views only call `Request*` methods and
listen to its events. The `GameStateMachine` whitelists transitions so illegal
flows throw instead of corrupting state.

## Tests

EditMode unit tests live in `Assets/Tests/EditMode` (assembly
`WheelDemo.Tests.EditMode`) and cover the two pure classes:

- **ZoneRules** — tier cadence (bronze/silver/golden) and cash-out gating.
- **SlicePicker** — determinism under a fixed seed, index range, zero-weight
  handling, and weight distribution.

Run them via **Window ▸ General ▸ Test Runner ▸ EditMode ▸ Run All**.

## Addressables & Sprite Atlas

- Reward icons can load through **Addressables** by key. The integration is
  gated behind the `ADDRESSABLES_PRESENT` define (set automatically by the
  `WheelDemo.Loading` assembly's version define once the package is installed).
  Each `RewardDefinitionSO` has an optional `iconAddress`; when it is empty, or
  before an Addressables content build exists, the embedded `Icon` sprite is
  used as a fallback so the game always runs.
- A **Sprite Atlas** packs the icon sprites to cut draw calls.

The editor-side configuration (install the package, create the atlas, mark
assets addressable, build content) is documented step-by-step in
[`docs/UNITY_EDITOR_STEPS.md`](docs/UNITY_EDITOR_STEPS.md).

## Building the Android APK

Summary (full click-by-click in `docs/UNITY_EDITOR_STEPS.md`):

1. **File ▸ Build Settings ▸ Android ▸ Switch Platform**.
2. Add `SampleScene` to *Scenes In Build*.
3. Set **Player Settings** package name and minimum API level.
4. **Build** (or **Build And Run** with a device attached).

## Screenshots

Captured across the three target aspect ratios to show the responsive wheel
layout:

| 20:9 | 16:9 | 4:3 |
|------|------|-----|
| ![20:9](docs/screenshots/aspect_20-9.png) | ![16:9](docs/screenshots/aspect_16-9.png) | ![4:3](docs/screenshots/aspect_4-3.png) |

## Project layout

```
Wheel/
├── Assets/
│   ├── Scenes/SampleScene.unity
│   ├── Scripts/        (Data, Core, Gameplay, Loading, Editor)
│   ├── Tests/EditMode/ (NUnit tests)
│   ├── Prefabs/        (ui_slice, ui_reward_row)
│   └── Plugins/        (DOTween)
├── Packages/
├── ProjectSettings/
└── docs/               (editor steps + screenshots)
```
