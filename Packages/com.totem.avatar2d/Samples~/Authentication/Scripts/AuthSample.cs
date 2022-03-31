using System.Collections;
using System.Collections.Generic;
using TMPro;
using TotemEntities;
using UnityEngine;

namespace Totem.Avatar2D.Samples
{
  /// <summary>
  ///   A demo using the MockDB, allows previewing all the avatars that belong to a user.
  /// </summary>
  [DefaultExecutionOrder(500)]
  internal sealed class AuthSample : MonoBehaviour
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
    private TMP_InputField inputUser = default;

    [SerializeField]
    private TMP_InputField inputPass = default;

    [SerializeField]
    private TMP_Dropdown inputAvatars = default;

    [SerializeField]
    private TextMeshProUGUI labelError = default;

    [SerializeField]
    private TextMeshProUGUI permutationLabel = default;

    [SerializeField]
    private MeshRenderer backgroundRenderer = default;

    #endregion


    #region Fields

    private TotemMockDB _database;

    private string _authedUser;

    private bool _isJumping;

    private float _bgOffset;

    #endregion


    #region Proeprties

    private IReadOnlyList<TotemAvatar> UserAvatars =>
      _database.UsersDB.GetUser(_authedUser)?.GetOwnedAvatars();

    #endregion


    #region MonoBehaviour

    private void Awake ()
    {
      _database = new TotemMockDB();
    }

    private void Start ()
    {
      TotemAvatar permutation = TotemGenerator.GenerateAvatar();
      character.Avatar = permutation;
      UpdatePermutationLabel(permutation);

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
      // Authentication form
      //

      // Tab to switch input
      if ( Input.GetKeyDown(KeyCode.Tab) )
      {
        TMP_InputField input = inputUser.isFocused ? inputPass : inputUser;
        input.Select();
        input.ActivateInputField();
      }

      // Ignore input while textfield is focused
      if ( inputUser.isFocused || inputPass.isFocused )
        return;

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
    ///   Login a user and list its avatars.
    /// </summary>
    public void Authenticate ()
    {
      _authedUser = null;
      labelError.gameObject.SetActive(false);
      inputAvatars.gameObject.SetActive(false);

      //
      // Authenticate
      //

      bool didAuth =
        _database.UsersDB.AuthenticateUser(inputUser.text, inputPass.text);
      if ( !didAuth )
      {
        labelError.gameObject.SetActive(true);
        labelError.text = "Unknown username and password combination.";
        return;
      }

      // 
      // List avatars
      //

      _authedUser = inputUser.text;
      inputAvatars.gameObject.SetActive(true);

      IReadOnlyList<TotemAvatar> avatars = UserAvatars;
      if ( avatars.Count == 0 )
      {
        labelError.gameObject.SetActive(true);
        labelError.text = "User has no avatars.";
        return;
      }

      List<TMP_Dropdown.OptionData> options = new(avatars.Count);
      for ( int index = 0; index < avatars.Count; index++ )
        options.Add(new TMP_Dropdown.OptionData($"Avatar {index + 1}"));

      inputAvatars.options = options;
      inputAvatars.value = 0;

      TotemAvatar permutation = avatars[0];
      character.Avatar = permutation;
      UpdatePermutationLabel(permutation);
    }

    /// <summary>
    ///   Display a user avatar.
    /// </summary>
    public void SelectAvatar (int _)
    {
      TotemAvatar permutation = UserAvatars[inputAvatars.value];
      character.Avatar = permutation;
      UpdatePermutationLabel(permutation);
    }

    private void UpdatePermutationLabel (TotemAvatar permutation)
    {
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