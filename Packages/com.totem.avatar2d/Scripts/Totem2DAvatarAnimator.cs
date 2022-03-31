using UnityEngine;

namespace Totem.Avatar2D
{
  /// <summary>
  ///   Utility component for controlling an Avatar’s animation state.
  /// </summary>
  /// <remarks>
  ///   For direct control, it’s possible to manipulate the Animator’s properties instead.
  /// </remarks>
  [AddComponentMenu("Totem/2D Avatar Animator")]
  [RequireComponent(typeof(Totem2DAvatar), typeof(Animator))]
  public sealed class Totem2DAvatarAnimator : MonoBehaviour
  {
    #region Constatnts

    private static readonly int ANIM_MOTION = Animator.StringToHash("Motion");

    private static readonly int ANIM_JUMP = Animator.StringToHash("Jump");

    #endregion


    #region Fields

    private Animator _animator;

    #endregion


    #region Properties

    /// <summary>
    ///   Active motion animation.
    /// </summary>
    public MotionState Motion
    {
      get => (MotionState) _animator.GetInteger(ANIM_MOTION);
      set => _animator.SetInteger(ANIM_MOTION, (int) value);
    }

    /// <summary>
    ///   The direction in which the avatar is facing.
    /// </summary>
    public DirState Direction
    {
      get => transform.localScale.x >= 0 ? DirState.Right : DirState.Left;
      set {
        Vector3 scale = transform.localScale;

        // Non-negative is right.
        scale.x = Mathf.Abs(scale.x);
        if ( value == DirState.Left )
          scale.x *= -1;

        transform.localScale = scale;
      }
    }

    #endregion


    #region MonoBehaviour

    private void Awake ()
    {
      _animator = GetComponent<Animator>();
    }

    #endregion


    #region Methods

    /// <summary>
    ///   Perform jump animation.
    /// </summary>
    public void Jump ()
    {
      _animator.SetTrigger(ANIM_JUMP);
    }

    #endregion
  }
}