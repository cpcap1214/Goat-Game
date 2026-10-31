using UnityEditor;using UnityEngine;
namespace OneMoreTime.EditorTools {
public static class MechanismPrefabs {
 // Templates have intentionally unassigned cross-instance links; BuildLevel assigns each plate/door pair.
 public static void Generate(){var plate=new GameObject("PressurePlate");var p=plate.AddComponent<PressurePlate>();p.graphic=plate.AddComponent<SpriteRenderer>();p.graphic.sprite=PresentationBuilder.White();p.graphic.drawMode=SpriteDrawMode.Sliced;p.graphic.size=new Vector2(1.4f,.12f);p.graphic.sortingOrder=3;Save(plate,"PressurePlate");var door=new GameObject("LinkedDoor");door.layer=DemoBuilder.WorldLayer;var d=door.AddComponent<LinkedDoor>();d.solid=door.AddComponent<BoxCollider2D>();d.solid.size=new Vector2(.6f,3.5f);d.graphic=door.AddComponent<SpriteRenderer>();d.graphic.sprite=PresentationBuilder.White();d.graphic.drawMode=SpriteDrawMode.Sliced;d.graphic.size=d.solid.size;Save(door,"LinkedDoor");var crumble=new GameObject("CrumblingPlatform");crumble.layer=DemoBuilder.WorldLayer;var c=crumble.AddComponent<CrumblingPlatform>();c.solid=crumble.AddComponent<BoxCollider2D>();c.solid.size=new Vector2(2,.2f);c.graphic=crumble.AddComponent<SpriteRenderer>();c.graphic.sprite=PresentationBuilder.Atlas("EarthSolid",new Rect(16,304,16,16));c.graphic.drawMode=SpriteDrawMode.Tiled;c.graphic.size=c.solid.size;Save(crumble,"CrumblingPlatform");}
 static void Save(GameObject go,string name){PrefabUtility.SaveAsPrefabAsset(go,DemoBuilder.Root+"/Prefabs/"+name+".prefab");Object.DestroyImmediate(go);}
 public static GameObject Spawn(string name,Transform parent){var g=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(DemoBuilder.Root+"/Prefabs/"+name+".prefab"),parent.gameObject.scene);g.transform.SetParent(parent);return g;}
}
}
