using TMPro;
using TotemEntities;
using UnityEngine;

namespace Totem.Avatar2D.Samples
{
  /// <summary>
  ///   A simple demo featuring a random procedural avatar.
  /// </summary>
  internal sealed class RandomSample : MonoBehaviour
  {
    #region Inspector

    [SerializeField]
    private Totem2DAvatar character = default;

    [SerializeField]
    private Totem2DAvatarAnimator controller = default;

    [SerializeField]
    private TextMeshProUGUI stateLabel = default;

    #endregion


    #region MonoBehaviour

    private void Start ()
    {
      Generate();
    }

    private void Update ()
    {
      //
      // Direction
      //

      int direction = 0;
      if ( Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) )
        direction++;
      if ( Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.W) )
        direction--;

      controller.Direction = direction switch
      {
        1 => DirState.Right,
        -1 => DirState.Left,
        _ => controller.Direction
      };

      //
      // Motion
      //

      if ( direction == 0 )
        controller.Motion = MotionState.Idle;
      else if ( Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift) )
        controller.Motion = MotionState.Run;
      else
        controller.Motion = MotionState.Walk;

      //
      // Jump
      //

      if ( Input.GetKey(KeyCode.Space) )
        controller.Jump();
    }

    #endregion


    #region Methods

    /// <summary>
    ///   Replace the avatar with a new random permutation.
    /// </summary>
    public void Generate ()
    {
      TotemAvatar permutation = TotemGenerator.GenerateAvatar();
      character.Avatar = permutation;
      stateLabel.text =
        $"{permutation.sex}\n#{ColorUtility.ToHtmlStringRGB(permutation.skinColor)}\n#{ColorUtility.ToHtmlStringRGB(permutation.eyeColor)}\n#{ColorUtility.ToHtmlStringRGB(permutation.hairColor)}\n{permutation.hairStyle}\n{permutation.bodyFat}\n{permutation.bodyMuscles}";
    }

    #endregion
  }
}