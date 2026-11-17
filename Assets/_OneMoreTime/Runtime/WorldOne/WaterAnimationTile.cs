using UnityEngine;
using UnityEngine.Tilemaps;
namespace OneMoreTime {
[CreateAssetMenu(menuName="One More Time/Animated Water Tile")]
public sealed class WaterAnimationTile:TileBase {
 public Sprite[] frames;public float framesPerSecond=8;
 public override void GetTileData(Vector3Int position,ITilemap tilemap,ref TileData data){data.sprite=frames!=null&&frames.Length>0?frames[0]:null;data.color=Color.white;data.transform=Matrix4x4.identity;data.flags=TileFlags.LockColor;data.colliderType=Tile.ColliderType.None;}
 public override bool GetTileAnimationData(Vector3Int position,ITilemap tilemap,ref TileAnimationData data){if(frames==null||frames.Length==0)return false;data.animatedSprites=frames;data.animationSpeed=framesPerSecond;data.animationStartTime=0;return true;}
}
}
