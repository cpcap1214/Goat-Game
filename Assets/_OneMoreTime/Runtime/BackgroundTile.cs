using UnityEngine;
using UnityEngine.Tilemaps;
namespace OneMoreTime {
[CreateAssetMenu(menuName="One More Time/Background Tile")]
public sealed class BackgroundTile:TileBase {
 public TileBase source;
 public Color tint=new Color(.5f,.5f,.5f,1);
 public override void GetTileData(Vector3Int position,ITilemap map,ref TileData data){
  if(source==null)return;source.GetTileData(position,map,ref data);data.color*=tint;data.colliderType=Tile.ColliderType.None;data.gameObject=null;data.flags|=TileFlags.LockColor;
 }
 public override bool GetTileAnimationData(Vector3Int position,ITilemap map,ref TileAnimationData data)=>source!=null&&source.GetTileAnimationData(position,map,ref data);
}
}
