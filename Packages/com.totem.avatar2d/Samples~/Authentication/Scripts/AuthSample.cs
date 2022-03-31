using System.Collections.Generic;
using TMPro;
using TotemEntities;
using UnityEngine;

namespace Totem.Avatar2D.Samples
{
  /// <summary>
  ///   A demo using the MockDB, allows previewing all the avatars that belong to a user.
  /// </summary>
  internal sealed class AuthSample : MonoBehaviour
  {
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

    #endregion


    #region Fields

    private TotemMockDB _database;

    private string _authedUser;

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

    #endregion
  }
}