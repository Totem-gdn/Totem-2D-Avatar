using UnityEngine;

// TODO: Runtime (allow for performant updating)

[ExecuteAlways]
public class AvatarColor : MonoBehaviour
{
  #region Constants

  private static readonly int EYES_COLOR = Shader.PropertyToID("_EyesColor");

  private static readonly int HAIR_COLOR = Shader.PropertyToID("_HairColor");

  private static readonly int BODY_COLOR = Shader.PropertyToID("_BodyColor");

  #endregion


  #region Fields

  private MaterialPropertyBlock _mpb;

  #endregion


  #region Inspector

  [SerializeField]
  private Color eyesColor = Color.white;

  [SerializeField]
  private Color hairColor = Color.white;

  [SerializeField]
  private Color bodyColor = Color.white;

  #endregion


  #region MonoBehviour

  private void Update ()
  {
    if ( _mpb == null )
      _mpb = new MaterialPropertyBlock();

    SpriteRenderer[]
      z = GetComponentsInChildren<SpriteRenderer>(); // TODO: Cache
    foreach ( SpriteRenderer sRenderer in z )
    {
      sRenderer.GetPropertyBlock(_mpb);
      _mpb.SetColor(EYES_COLOR, eyesColor);
      _mpb.SetColor(HAIR_COLOR, hairColor);
      _mpb.SetColor(BODY_COLOR, bodyColor);
      sRenderer.SetPropertyBlock(_mpb);
    }
  }

  #endregion
}