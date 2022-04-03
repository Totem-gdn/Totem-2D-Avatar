# Totem 2D Avatar for Unity

**Procedural avatar for Unity leveraging the Totem platform.**

## Installation

Add the following packages into a Unity 2021.2+ project:

> In the _Package Manager_ window select `+` button at the top lect corner, select **Add package from git URL...**, and copy the package’s link.

1. `https://github.com/Totem-gdn/TotemGeneratorUnity.git#1.0.1`
2. `https://github.com/Totem-gdn/Totem-2D-Avatar.git?path=/Packages/com.totem.avatar2d`

## Usage

> The package provides the **Totem Avatar** prefab, all it takes is to drag-and-drop it to the scene.

### Avatar

To change the character’s displayed permutation, set its `Avatar` property:

```cs
public Totem2DAvatar character;

public void SetAvatar(TotemAvatar avatar)
{
  character.Avatar = avatar;
}
```

### Animation

To simplify the animation control, add the **Totem > 2D Avatar Animation** component to the prefab instance in the scene:

```cs
controller.Direction = DirState.Right; // Left, Right
controller.Motion = MotionState.Walk; // Idle, Walk, Right
controller.Jump();
```

Alternatively, the Animator component can be manipulated directly using the following paramters:

- `Jump` _trigger_
- `Motion` _int_
  - `0` Idle
  - `1` Walk
  - `2` Run

## Samples

The following samples are provided through the Package Manager:

1. Random – A simple demo featuring a random procedural avatar.
2. Authentication – A demo using the MockDB, allows previewing all the avatars that belong to a user.

# Production

The following is a the production pipline steps. Its only relevent for those intedning to alter or augment the character.

## PSB Rig

The base PSB is ___Base Female Thin Wimp_, other PSBs reference its skeleton. Change the skeleton is only possible through this PSB.

While adding or manipulatin the art, the following constraints should be taken into account:

1. It’s recommanded to maintina the original element’s dimensions, otherwise the mesh and weights must be udapted accorsingly.
2. When adding a new variant, a new PSB must be created (PNG is not supported) referencing the base skeleton and manually create mesh and weights.
3. To add a new element (and new bone), the base PSB need to be updated with a new bone in the skeleton (also mesh and weights).

There is a single Sprite Library asset named _Charactar Sprite Lib_. It composes the sprites from multiple PSB rigs into a unified libraray, spaning the whole range of permutations.

> The skeleton has a bone named `weapon` for future updates.

## Prefab

The prefab _Totem Avatar_ combines the multiple PSBs into a single procedural charactar. Its composed manually from the multiple resulting rigs and controlled by the Sprite Library from the previous step.

All sprites use the _Totem 2D Avatar_ material that supports masked coloring.

Each swapable Sprite Renderer also has a Sprite Resolver component assigned with the appropriate category and an initial label.

## Masks

The characters’ cloth will retain the color of the original texture, other parts will be colored according the the _Totem filter_.

We use a secondary mask texture to define the purpose of each area in the texture.

> For reference, you can download the [original masks PSDs](https://github.com/Totem-gdn/Totem-2D-Avatar/releases/tag/v1.0.0). They are pre configured with channeling as described below.

### Base atlas

The PSB will generate an atlas texture of all its sprites – it can be used to assist in the process of creating the masks texture.

The main Unity project of this asset (not the package itslef but the whole project) provides a script for saving the generated atlas as a standalone PNG:

1. For the PSB importer, enable _Read/Write Enabled_ and disable compression.
2. Right click the generated atlas and select **2D Avatar Pipeline > Save as PNG**.
3. Deselect the read/write option and restore the compression.

### Channels

We use the texture’s color channels to diffrentiate between the character’s components.

- `r` Eyes
- `g` Hair
- `b` Skin
- `a` Shadow

The mask texture must be saved as a format that supports 4 channels, it recommanded to use **Targa 32bit**.

### Unity Setup

When importing the texture into Unity, make sure to set it as a default (non sprite) texture. **Deselect** the _Alpha is transparency_ option.

After importing and configuring the mask texture, we need to attach it to the rig’s texture.
Open the Sprite Editor of the original PSB, under the _Secondary Textures_ screen add a new texture named `_MaskTex` and assign the mask texture to it.

## Material

The _Totem 2D Avatar_ material is used by all the character’s sprites and operates the maseked coloring. It uses the shader `Totem/2D Avatar (Unlit Sprite)`.

The shadow can be modified using the following parameters:
- Shadow Saturation _0..1_
- Shadow Brightness _0..1_

## Animation

Animating the character follows the familiar Unity workflow.