CPR Training Simulator
LVNV Financial Insurance School — Interactive Character Experience

A Unity-based interactive CPR training simulator that guides users through a full Cardiopulmonary Resuscitation tutorial using animated 3D characters, synchronized audio narration, and a step-by-step instruction system.

Project Description

This project is an interactive training simulator built in Unity 6 for LVNV Financial Insurance School. It teaches users how to perform CPR through a guided, animated experience featuring three characters:

Dr. Marcus — The instructor who explains each step of the CPR process through synchronized animation and audio narration
Jamie — The patient who collapses and receives CPR during the demonstration
Alex — The trainee who performs CPR on the patient during the live demonstration

The experience walks users through five instructional steps delivered by Dr. Marcus, followed by a live demonstration where Jamie and Alex perform the full CPR sequence together.

Controls and Interactions
UI Navigation
Button	Action
Start Training (Screen 1)	Moves to the character introduction screen
Begin Scenario (Screen 2)	Starts the CPR training and enters Screen 3
Back	Returns to the previous screen
Playback Controls (Screen 3 — Training View)
Control	Action
Play	Resumes the current step if paused
Pause	Freezes the current animation and pauses audio
Next Step	Advances to the next instructional step
Previous Step	Goes back to the previous step (works during demonstration too)
Volume Slider	Adjusts the overall audio volume in real time
Mute Button	Toggles all audio on or off
Timeline Slider	Visual indicator of progress through the training
Keyboard / Direct Input
Key	Action
No direct keyboard bindings currently — all interaction is UI-driven via mouse or touch	
Screens
Screen	Description
Screen 1 — Welcome	Title screen with animated intro sequence. Shows the CPR Training Simulator title and Start Training button
Screen 2 — Character Introduction	Introduces the three characters with name and role cards before the training begins
Screen 3 — Training View	Main training screen. Shows the 3D scene, step title, playback controls, volume, and timeline
Screen 4 — Pause / Step Detail	Reserved for detailed step information when paused
Completion Screen	Shown after the demonstration ends. Confirms the training is complete
CPR Training Steps
Step	Title	Description
1	Scene Safety	Dr. Marcus explains how to assess the scene before approaching a patient
2	Check Responsiveness	Marcus points at Jamie and demonstrates how to check for a response
3	Call for Help	Marcus explains how to contact emergency services
4	CPR Technique	Marcus points at the CPR diagram and explains hand placement and compression depth
5	Rescue Breaths	Marcus explains the 30:2 compression-to-breath ratio
Demo	Live Demonstration	Jamie and Alex perform the full CPR sequence: walking in, collapse, CPR, revival
System Architecture

The project uses a Manager-based architecture where each manager handles one responsibility and communicates with others via static Instance references.

Scripts Overview
Assets/Scripts/
├── Managers/
│   ├── TrainingManager.cs   — Core training logic
│   ├── UIManager.cs         — Screen switching and UI state
│   └── AudioManager.cs      — All audio playback and volume control
└── Data/
    └── CPRStep.cs           — Serializable data container for step info
TrainingManager.cs

The central controller for the training experience.

Responsibilities:

Defines and sequences all CPR instructional steps as MarcusSection data objects
Controls Marcus's animator — fires OnTalk, OnPoint, OnWalk triggers in sync with audio timestamps
Manages the audio source — plays, pauses, and resumes narration from exact timestamps per section
Runs the demonstration coroutine — sequences Jamie's collapse, Alex's CPR, and Jamie's revival
Handles all playback button events: Play, Pause, Next Step, Previous Step
Tracks isInDemonstration flag so pause and previous behave correctly during demo phase
Updates the step title text and timeline slider on every section change

Key methods:

Method	Purpose
StartTraining()	Called by UIManager when Screen 3 opens
PlaySection(int index)	Plays a specific Marcus section with correct audio timestamp
SectionTimer(float duration)	Coroutine that counts down section time and freezes on completion
BeginDemonstration()	Switches to Jamie and Alex, resets animators, starts demo coroutine
DemonstrationSequence()	Full timed coroutine for the CPR demonstration
OnPlayPressed()	Resumes animation and audio — works for both Marcus and demo phases
OnPausePressed()	Freezes animation and audio — works for both phases
OnNextPressed()	Advances Marcus steps or triggers demonstration if on last step
OnPreviousPressed()	Goes back one Marcus step or exits demonstration back to step 5
UIManager.cs

Handles all screen transitions with animated in and out states.

Responsibilities:

Maintains reference to all five screen GameObjects
Calls SetActive and animator IsVisible bool on each screen transition
Uses a coroutine delay so the outro animation finishes before the next screen activates
Calls TrainingManager.StartTraining() when entering Screen 3
Calls AudioManager.StopMenuMusic() and PlayMenuMusic() on screen changes

Screen transition flow:

Screen1 (Welcome)
  → Screen2 (Character Intro)
    → Screen3 (Training) ← calls StartTraining()
      → Completion Screen

Each screen has its own Animator Controller with:

IsVisible Bool parameter
ScreenX_Intro clip — plays on enter
ScreenX_outro clip — plays on exit
AudioManager.cs

Centralized audio management with volume and mute controls.

Responsibilities:

Owns two Audio Sources: backgroundMusic and trainingAudio
Plays menu background music on Screen 1 and 2 with fade in
Fades out background music when training starts
Responds to volume slider changes via onValueChanged listener
Toggles mute state and swaps the mute button sprite accordingly
Applies volume changes globally via AudioListener.volume
Animator Controllers

Each character has its own Animator Controller with humanoid animations from Mixamo:

Dr. Marcus (ch16.controller)

State	Trigger	Purpose
Talking	OnTalk	Default narration pose
Pointing	OnPoint	Directing attention to diagram or patient
Walking Left Turn	OnWalk	Exits scene after instruction

Parameters: OnTalk (Trigger), OnPoint (Trigger), OnWalk (Trigger)

Jamie — Patient (ch28.controller)

State	Trigger	Purpose
Standard Walk	default	Walks into scene
Falling Back Death	OnCollapse	Heart attack collapse
Receiving CPR	OnReceiveCPR	Reacts to compressions
Stand Up	OnRevived	Wakes up after CPR

Parameters: OnCollapse (Trigger), OnReceiveCPR (Trigger), OnRevived (Trigger)

Alex — Trainee (ch12.controller)

State	Trigger	Purpose
Standard Walk	default	Walks into scene
Kneeling Down	OnKneel	Gets into position
Administering CPR	OnCPR	Performs chest compressions

Parameters: OnWalk (Trigger), OnKneel (Trigger), OnCPR (Trigger), OnStandUp (Trigger)

UI Animation System

Each screen uses Unity's Animator component with a Canvas Group or Rect Transform Scale approach:

Intro clip — elements slide in or scale up from zero on screen enter
Outro clip — elements slide out or scale down to zero on screen exit
IsVisible bool — single parameter controlling which clip plays
Transitions have Has Exit Time OFF and Transition Duration 0 for instant response
Folder Structure
Assets/
├── Animation clips/
│   ├── UI/              — Screen intro and outro animation clips
│   ├── ch12/            — Alex animation clips
│   ├── ch16/            — Marcus animation clips
│   └── ch28/            — Jamie animation clips
├── Models/
│   ├── Ch12_nonPBR      — Alex character model
│   ├── Ch16_nonPBR      — Marcus character model
│   └── Ch28_nonPBR      — Jamie character model
├── Scripts/
│   ├── Managers/
│   │   ├── TrainingManager.cs
│   │   ├── UIManager.cs
│   │   └── AudioManager.cs
│   └── CPRStep.cs
├── Scenes/
│   └── SampleScene
└── TutorialInfo/
Dependencies
Unity 6 (6000.0.75f1)
TextMesh Pro — UI text rendering
Mixamo — Character models and animation clips (Humanoid rig)
Unity UI — Canvas, Slider, Button components
How to Run
Open the project in Unity 6
Open Assets/Scenes/SampleScene
Press Play in the Unity Editor
Click Start Training on the welcome screen
Click Begin Scenario to enter the training
Use Next Step to progress through each instructional section
Watch the demonstration after Step 5 completes automatically
