using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace OneMoreTime.EditorTools {
// Exit guidance is decoration only: no collision, trigger, or progression logic.
public static class ExitSignBuilder {
 const string Root="Assets/_OneMoreTime";
 const string ManagedName="Exit direction sign";
 static readonly Vector2[] Bases={
  new Vector2(74.2f,12.4f),new Vector2(41.6f,5),new Vector2(28.5f,39),
  new Vector2(94.2f,12),new Vector2(66.5f,5),new Vector2(85.7f,9),
  new Vector2(9.5f,63.7f),new Vector2(116.7f,47.3f)
 };

 static GameObject SignPrefab(bool upward) {
  string name=upward?"WoodenExitSignUp":"WoodenExitSign";
  string prefabPath=Root+"/Prefabs/WorldOne/"+name+".prefab";
  var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
  if(prefab!=null)return prefab;
  string texturePath=Root+"/Art/WorldOne/"+name+".png";
  AssetDatabase.ImportAsset(texturePath,ImportAssetOptions.ForceSynchronousImport);
  var importer=(TextureImporter)AssetImporter.GetAtPath(texturePath);
  if(importer==null)throw new InvalidOperationException("Missing exit sign artwork: "+texturePath);
  // Match the room's coarse pixel grid at normal gameplay zoom. Keep the full
  // supplied source on disk; resolution reduction is a Unity import setting.
  importer.textureType=TextureImporterType.Default;importer.maxTextureSize=64;
  importer.npotScale=TextureImporterNPOTScale.None;importer.filterMode=FilterMode.Point;
  importer.textureCompression=TextureImporterCompression.Uncompressed;
  importer.mipmapEnabled=false;importer.alphaIsTransparency=true;importer.isReadable=true;
  importer.wrapMode=TextureWrapMode.Clamp;importer.SaveAndReimport();
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
  var pixels=texture.GetPixels32();int left=texture.width,right=-1,bottom=texture.height,top=-1;
  for(int y=0;y<texture.height;y++)for(int x=0;x<texture.width;x++)if(pixels[y*texture.width+x].a>=128){
   left=Mathf.Min(left,x);right=Mathf.Max(right,x);bottom=Mathf.Min(bottom,y);top=Mathf.Max(top,y);
  }
  if(right<left)throw new InvalidOperationException("Exit sign artwork is empty.");
  float postX=0;int count=0;
  for(int x=left;x<=right;x++)if(pixels[bottom*texture.width+x].a>=128){postX+=x+.5f;count++;}
  // Pivot at the actual foot of the post so signs sit on authored surfaces.
  var rect=new Rect(left,bottom,right-left+1,top-bottom+1);
  var pivot=new Vector2((postX/count-left)/rect.width,0);
  var sprite=Sprite.Create(texture,rect,pivot,32,0,SpriteMeshType.FullRect);sprite.name=name;
  AssetDatabase.CreateAsset(sprite,Root+"/Generated/WorldOne/"+name+".asset");
  string materialPath=Root+"/Generated/WorldOne/ExitSign.mat";
  var material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
  if(material==null){material=new Material(Shader.Find("Sprites/Default"));AssetDatabase.CreateAsset(material,materialPath);}
  var temporary=new GameObject(name);
  try {
   var renderer=temporary.AddComponent<SpriteRenderer>();renderer.sprite=sprite;
   renderer.sharedMaterial=material;renderer.sortingOrder=5;
   prefab=PrefabUtility.SaveAsPrefabAsset(temporary,prefabPath);
  } finally {Object.DestroyImmediate(temporary);}
  return prefab;
 }

 public static void Patch(Scene scene,int number) {
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before placing signs.");
  if(number<1||number>8)throw new ArgumentOutOfRangeException(nameof(number));
  var room=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<MainRoom>(true)).Single();
  var old=room.GetComponentsInChildren<Transform>(true).Where(t=>t.name==ManagedName||t.name=="Exit trail marker").ToArray();
  foreach(var t in old){
   if(t.GetComponentsInChildren<Collider2D>(true).Length!=0)throw new InvalidOperationException("Refusing to replace a sign with gameplay collision.");
   Object.DestroyImmediate(t.gameObject);
  }
  var sign=(GameObject)PrefabUtility.InstantiatePrefab(SignPrefab(number==7),scene);
  sign.name=ManagedName;sign.transform.SetParent(room.transform,false);sign.transform.position=Bases[number-1];
  // Room 7 exits vertically at the camera ceiling; keep its arrow fully visible.
  if(number==7)sign.transform.localScale=Vector3.one*.75f;
  EditorSceneManager.MarkSceneDirty(scene);
 }

 [MenuItem("Tools/One More Time/Place Main Room Exit Signs")]
 public static void ApplyFromMenu(){Debug.Log(Apply());}
 public static string Apply() {
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before placing signs.");
  for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Save scene edits before placing signs.");
  var previous=SceneManager.GetActiveScene();
  for(int number=1;number<=8;number++){
   string path=Root+"/Scenes/Main_Room0"+number+".unity";
   var scene=SceneManager.GetSceneByPath(path);bool loaded=scene.IsValid()&&scene.isLoaded;
   if(!loaded)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
   try {Patch(scene,number);EditorSceneManager.SaveScene(scene);}
   finally {if(!loaded)EditorSceneManager.CloseScene(scene,true);}
  }
  AssetDatabase.SaveAssets();if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);
  return "Placed eight grounded wooden exit signs. Room 7 points up; Room 8 points into the cavern behind the waterfall.";
 }
}
}
