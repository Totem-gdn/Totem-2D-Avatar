using System.Collections;
using TMPro;
using TotemEntities;
using UnityEngine;

namespace Totem.Avatar2D.Samples
{
  /// <summary>
  ///   A simple demo featuring a random procedural avatar.
  /// </summary>
  [DefaultExecutionOrder(500)]
  internal sealed class RandomSample : MonoBehaviour
  {
    #region Constants

    private const float JUMP_DURATION = 0.35f;

    private const float JUMP_AMOUNT = 1.8f;

    private static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");

    private const float BACKGROUND_SPEED_WALK = 0.4f;

    private const float BACKGROUND_SPEED_RUN = 0.8f;

    #endregion


    #region Inspector

    [SerializeField]
    private Totem2DAvatar character = default;

    [SerializeField]
    private Totem2DAvatarAnimator controller = default;

    [SerializeField]
    private TextMeshProUGUI permutationLabel = default;

    [SerializeField]
    private MeshRenderer backgroundRenderer = default;

    #endregion


    #region Fields

    private bool _isJumping;

    private float _bgOffset;

    #endregion


    #region MonoBehaviour

    private void Start ()
    {
      Generate();

      // Stretch background to fill screen

      Camera cam = Camera.main;
      if ( cam == null )
        return;

      float height = 2 * cam.orthographicSize;
      float width = height * cam.aspect;
      backgroundRenderer.transform.localScale = new Vector3(width, height, 1);
    }

    private void Update ()
    {
      //
      // Generate shortcut
      //

      if ( Input.GetKeyDown(KeyCode.Return) ||
           Input.GetKeyDown(KeyCode.KeypadEnter) )
        Generate();

      //
      // Character controller
      //

      // Direction

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

      // Motion

      float offset = 0;
      if ( direction == 0 )
      {
        controller.Motion = MotionState.Idle;
      }
      else if ( Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift) )
      {
        controller.Motion = MotionState.Run;
        offset = Time.deltaTime * BACKGROUND_SPEED_RUN;
      }
      else
      {
        controller.Motion = MotionState.Walk;
        offset = Time.deltaTime * BACKGROUND_SPEED_WALK;
      }

      // Jump

      if ( Input.GetKeyDown(KeyCode.Space) && !_isJumping )
      {
        _isJumping = true;
        controller.Jump();
        StartCoroutine(Jump());
      }

      // Background

      if ( direction < 0 )
        offset *= -1;

      if ( _isJumping )
        offset *= 1.5f;

      _bgOffset += offset;

      backgroundRenderer.sharedMaterial.SetTextureOffset(
        MAIN_TEX, new Vector2(_bgOffset, 0));
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
      permutationLabel.text =
        $"{permutation.sex}\n#{ColorUtility.ToHtmlStringRGB(permutation.skinColor)}\n#{ColorUtility.ToHtmlStringRGB(permutation.eyeColor)}\n#{ColorUtility.ToHtmlStringRGB(permutation.hairColor)}\n{permutation.hairStyle}\n{permutation.bodyFat}\n{permutation.bodyMuscles}";
    }

    private IEnumerator Jump ()
    {
      Vector3 initial = character.transform.position;

      float passed = 0;
      while ( passed <= JUMP_DURATION )
      {
        float progress = Mathf.Sin((passed / JUMP_DURATION) * Mathf.PI);
        passed += Time.deltaTime;

        Vector3 pos = initial;
        pos.y += progress * JUMP_AMOUNT;
        character.transform.position = pos;

        yield return null;
      }

      character.transform.position = initial;
      _isJumping = false;
    }

    #endregion
  }
}