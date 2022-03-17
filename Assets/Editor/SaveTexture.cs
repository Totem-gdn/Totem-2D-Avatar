using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Object = UnityEngine.Object;

// TODO: Conditional for edit mode

/// <summary>
///   TODO
/// </summary>
internal static class SaveTexture
{
  #region Constants

  private const string MENU_PATH = "Assets/2D Avatar Pipeline/Save as PNG";

  #endregion


  #region Methods

  [MenuItem(MENU_PATH, false, 0)]
  private static void Perform ()
  {
    // Iterate selected objects, save textures as a PNG file.
    foreach ( Object obj in Selection.objects )
    {
      // Ignore non-texture assets 
      if ( obj is not Texture2D texture )
        continue;

      // Validate texture is readable
      if ( !texture.isReadable )
        throw new InvalidOperationException(
          $"Texture '{texture.name}' is not readable, the texture memory can not be accessed from scripts. You can make the texture readable in the Texture Import Settings.");

      // TODO
      if ( GraphicsFormatUtility.IsCompressedFormat(texture.graphicsFormat) )
        throw new Exception($""); // TODO

      // Create a new file at the project's root
      byte[] data = texture.EncodeToPNG();
      string path = $"{Application.dataPath}/{texture.name}.png";
      File.WriteAllBytes(path, data);
      AssetDatabase.Refresh();
    }
  }

  [MenuItem(MENU_PATH, true)]
  private static bool Validate ()
  {
    // Are any of the selected assets a texture?
    foreach ( Object obj in Selection.objects )
      if ( obj is Texture2D )
        return true;

    return false;
  }

  #endregion
}