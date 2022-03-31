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
    private TextMeshProUGUI stateLabel = default;

    #endregion


    #region MonoBehaviour

    private void Start ()
    {
      Generate();
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