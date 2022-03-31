using System;
using enums;
using TotemEntities;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Totem.Avatar2D
{
  [ExecuteAlways]
  [AddComponentMenu("Totem/2D Avatar")]
  public sealed class Totem2DAvatar : MonoBehaviour
  {
    #region Constants

    private static readonly int EYES_COLOR = Shader.PropertyToID("_EyesColor");

    private static readonly int HAIR_COLOR = Shader.PropertyToID("_HairColor");

    private static readonly int SKIN_COLOR = Shader.PropertyToID("_BodyColor");

    #endregion


    #region Inspector

    [SerializeField]
    private TotemAvatar avatar;

    [Header("Resolvers")]
    [SerializeField]
    private SpriteResolver hairFront = default;

    [SerializeField]
    private SpriteResolver hairBack = default;

    [SerializeField]
    private SpriteResolver bra = default;

    [SerializeField]
    private SpriteResolver neck = default;

    [SerializeField]
    private SpriteResolver head = default;

    [SerializeField]
    private SpriteResolver armFront = default;

    [SerializeField]
    private SpriteResolver armBack = default;

    [SerializeField]
    private SpriteResolver underwear = default;

    [SerializeField]
    private SpriteResolver pantLegFront = default;

    [SerializeField]
    private SpriteResolver legFront = default;

    [SerializeField]
    private SpriteResolver legBack = default;

    [SerializeField]
    private SpriteResolver body = default;

    #endregion


    #region Fields

    private MaterialPropertyBlock _mpb;

    private SpriteRenderer[] _renderers;

    #endregion


    #region Proeprties

    public TotemAvatar Avatar
    {
      get => avatar;
      set {
        avatar = value;
        UpdatePermutation();
      }
    }

    #endregion


    #region MonoBehviour

    private void Start ()
    {
      _renderers = GetComponentsInChildren<SpriteRenderer>();
      _mpb = new MaterialPropertyBlock();
      UpdatePermutation();
    }

    private void OnValidate ()
    {
      if ( Application.isPlaying )
        return;

      _renderers = GetComponentsInChildren<SpriteRenderer>();
      _mpb = new MaterialPropertyBlock();
      UpdatePermutation();
    }

    #endregion


    #region Methods

    /// <summary>
    ///   Resolve and recolor sprites according to the current avatar permutation.
    /// </summary>
    private void UpdatePermutation ()
    {
      //
      // Sprites
      //

      // Hair

      hairFront.SetCategoryAndLabel("Hair Front", avatar.hairStyle.ToString());

      switch ( avatar.hairStyle )
      {
        case HairStyleEnum.Ponytail:
        case HairStyleEnum.Long:
        case HairStyleEnum.Dreadlocks:
          hairBack.gameObject.SetActive(true);
          hairBack.SetCategoryAndLabel("Hair Back",
                                       avatar.hairStyle.ToString());
          break;
        default:
          hairBack.gameObject.SetActive(false);
          break;
      }

      // Bra

      bool isFemale = avatar.sex == SexEnum.Female;
      bra.gameObject.SetActive(isFemale);
      if ( isFemale )
      {
        bra.gameObject.SetActive(true);
        bra.SetCategoryAndLabel("Bra", avatar.bodyFat switch
        {
          BodyFatEnum.Thin when avatar.bodyMuscles is BodyMusclesEnum.Muscular
            => "Thn Msc",
          BodyFatEnum.Thin when avatar.bodyMuscles is BodyMusclesEnum.Wimp =>
            "Thn Wmp",
          BodyFatEnum.Fat when avatar.bodyMuscles is BodyMusclesEnum.Muscular =>
            "Fat Msc",
          BodyFatEnum.Fat when avatar.bodyMuscles is BodyMusclesEnum.Wimp =>
            "Fat Wmp",
          _ => throw new ArgumentOutOfRangeException()
        });
      }

      // Head

      head.SetCategoryAndLabel("Head", isFemale switch
      {
        false when avatar.bodyFat is BodyFatEnum.Thin => "Male Thn",
        false when avatar.bodyFat is BodyFatEnum.Fat => "Male Fat",
        true when avatar.bodyFat is BodyFatEnum.Thin => "Female Thn",
        true when avatar.bodyFat is BodyFatEnum.Fat => "Female Fat",
        _ => throw new ArgumentOutOfRangeException()
      });

      // Neck & Arms

      string musclesLabel = avatar.bodyMuscles switch
      {
        BodyMusclesEnum.Wimp => "Wmp",
        BodyMusclesEnum.Muscular => "Msc",
        _ => throw new ArgumentOutOfRangeException()
      };
      neck.SetCategoryAndLabel("Neck", musclesLabel);
      armFront.SetCategoryAndLabel("Arm Front", musclesLabel);
      armBack.SetCategoryAndLabel("Arm Back", musclesLabel);

      // Underwear

      underwear.SetCategoryAndLabel("Underwear", isFemale switch
      {
        false when avatar.bodyFat is BodyFatEnum.Thin &&
                   avatar.bodyMuscles is BodyMusclesEnum.Muscular =>
          "Male Thn Msc",
        false when avatar.bodyFat is BodyFatEnum.Thin &&
                   avatar.bodyMuscles is BodyMusclesEnum.Wimp => "Male Thn Wmp",
        false when avatar.bodyFat is BodyFatEnum.Fat => "Male Fat",
        true when avatar.bodyFat is BodyFatEnum.Thin => "Female Thn",
        true when avatar.bodyFat is BodyFatEnum.Fat => "Female Fat",
        _ => throw new ArgumentOutOfRangeException()
      });

      // Legs

      string legLabel = isFemale switch
      {
        false when avatar.bodyMuscles is BodyMusclesEnum.Muscular => "Male Msc",
        false when avatar.bodyMuscles is BodyMusclesEnum.Wimp => "Male Wmp",
        true when avatar.bodyMuscles is BodyMusclesEnum.Muscular =>
          "Female Msc",
        true when avatar.bodyMuscles is BodyMusclesEnum.Wimp => "Female Wmp",
        _ => throw new ArgumentOutOfRangeException()
      };
      pantLegFront.SetCategoryAndLabel("Pant Leg Front", avatar.sex.ToString());
      legFront.SetCategoryAndLabel("Leg Front", legLabel);
      legBack.SetCategoryAndLabel("Leg Back", legLabel);

      // Body

      body.SetCategoryAndLabel("Body", isFemale switch
      {
        false when avatar.bodyFat is BodyFatEnum.Thin &&
                   avatar.bodyMuscles is BodyMusclesEnum.Muscular =>
          "Male Thn Msc",
        false when avatar.bodyFat is BodyFatEnum.Thin &&
                   avatar.bodyMuscles is BodyMusclesEnum.Wimp => "Male Thn Wmp",
        false when avatar.bodyFat is BodyFatEnum.Fat &&
                   avatar.bodyMuscles is BodyMusclesEnum.Muscular =>
          "Male Fat Msc",
        false when avatar.bodyFat is BodyFatEnum.Fat &&
                   avatar.bodyMuscles is BodyMusclesEnum.Wimp => "Male Fat Wmp",
        true when avatar.bodyFat is BodyFatEnum.Thin &&
                  avatar.bodyMuscles is BodyMusclesEnum.Muscular =>
          "Female Thn Msc",
        true when avatar.bodyFat is BodyFatEnum.Thin &&
                  avatar.bodyMuscles is BodyMusclesEnum.Wimp =>
          "Female Thn Wmp",
        true when avatar.bodyFat is BodyFatEnum.Fat &&
                  avatar.bodyMuscles is BodyMusclesEnum.Muscular =>
          "Female Fat Msc",
        true when avatar.bodyFat is BodyFatEnum.Fat &&
                  avatar.bodyMuscles is BodyMusclesEnum.Wimp =>
          "Female Fat Wmp",
        _ => throw new ArgumentOutOfRangeException()
      });

      //
      // Shader Colors
      //

      // Note: We must re-activate hidden sprites before re-coloring, otherwise
      // the re-activated sprites will feature the previous color.

      foreach ( SpriteRenderer sRenderer in _renderers )
      {
        sRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(EYES_COLOR, avatar.eyeColor);
        _mpb.SetColor(HAIR_COLOR, avatar.hairColor);
        _mpb.SetColor(SKIN_COLOR, avatar.skinColor);
        sRenderer.SetPropertyBlock(_mpb);
      }
    }

    #endregion
  }
}