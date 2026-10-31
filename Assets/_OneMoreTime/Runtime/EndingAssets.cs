using UnityEngine;
namespace OneMoreTime {
public sealed class EndingAssets:ScriptableObject {
 public Texture2D background;
 public AnimationLibrary goat;
 // Normalized coordinates are matched to the generated background.
 public Vector2 doorway=new Vector2(.813f,.80f);
}
}
