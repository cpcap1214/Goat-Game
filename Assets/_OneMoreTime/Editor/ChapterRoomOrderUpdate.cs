using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneMoreTime.EditorTools {
// One-time asset migration plus repeatable entrance/exit configuration. MoveAsset
// preserves scene GUIDs, so references continue to point at the same room content.
public static class ChapterRoomOrderUpdate {
 const string Directory="Assets/_OneMoreTime/Scenes/";
 static string Path(int room)=>Directory+"Main_Room0"+room+".unity";

 [MenuItem("Tools/One More Time/Apply Chapter Room Order 6 Before 4")]
 public static string Apply(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Exit Play mode before changing room order.");
  for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)
   throw new InvalidOperationException("Save your scene edits before changing room order.");
  bool move=HasAcquisition(4);
  if(!move&&!HasAcquisition(5))throw new InvalidOperationException("Cannot identify the recording acquisition room; no scenes moved.");
  if(move){
   var settings=EditorBuildSettings.scenes;
   // Keep an unaffected scene open while the three assets are moved.
   if(SceneManager.GetActiveScene().path==Path(4)||SceneManager.GetActiveScene().path==Path(5)||SceneManager.GetActiveScene().path==Path(6))
    EditorSceneManager.OpenScene(Path(3),OpenSceneMode.Single);
   foreach(int n in new[]{4,5,6}){var loaded=SceneManager.GetSceneByPath(Path(n));if(loaded.IsValid()&&loaded.isLoaded)EditorSceneManager.CloseScene(loaded,true);}
   string temp4=Directory+"RoomOrder_Acquisition_Temporary.unity",temp5=Directory+"RoomOrder_Bridges_Temporary.unity";
   var moved=new List<KeyValuePair<string,string>>();
   try {
    Move(Path(4),temp4,moved);Move(Path(5),temp5,moved);Move(Path(6),Path(4),moved);
    Move(temp4,Path(5),moved);Move(temp5,Path(6),moved);
   }catch{
    for(int i=moved.Count-1;i>=0;i--)AssetDatabase.MoveAsset(moved[i].Value,moved[i].Key);
    throw;
   }
   // Build indices remain in the existing numeric main-room order. All other
   // build entries and their enabled flags retain their original positions.
   EditorBuildSettings.scenes=settings.Select(s=>new EditorBuildSettingsScene(s.path,s.enabled)).ToArray();
  }
  string river=ChapterOneExpansionBuilder.BuildRoom(4);
  PatchScene(5,s=>RoomFourUpdate.UpdateEntrance(s));
  string bridges=ChapterOneExpansionBuilder.BuildRoom(6);
  PatchScene(3,s=>{
   var room=s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<MainRoom>(true)).Single();
   if(room.nextRoomScene=="Main_Room04")return;
   room.nextRoomScene="Main_Room04";EditorUtility.SetDirty(room);EditorSceneManager.MarkSceneDirty(s);
  });
  return (move?"Moved existing scene assets with GUIDs preserved. ":"Room order already migrated. ")+river+" Room 5 entrance updated in place. "+bridges;
 }
 static bool HasAcquisition(int number){
  var scene=SceneManager.GetSceneByPath(Path(number));bool loaded=scene.IsValid()&&scene.isLoaded;
  if(!loaded)scene=EditorSceneManager.OpenScene(Path(number),OpenSceneMode.Additive);
  try{return scene.GetRootGameObjects().Any(g=>g.GetComponentInChildren<Room4AbilityTutorial>(true)!=null);}
  finally{if(!loaded)EditorSceneManager.CloseScene(scene,true);}
 }
 static void Move(string source,string destination,List<KeyValuePair<string,string>> moved){
  string error=AssetDatabase.MoveAsset(source,destination);
  if(!string.IsNullOrEmpty(error))throw new InvalidOperationException(error);
  moved.Add(new KeyValuePair<string,string>(source,destination));
 }
 static void PatchScene(int number,Action<Scene> patch){
  var previous=SceneManager.GetActiveScene();var scene=SceneManager.GetSceneByPath(Path(number));bool loaded=scene.IsValid()&&scene.isLoaded;
  if(!loaded)scene=EditorSceneManager.OpenScene(Path(number),OpenSceneMode.Additive);
  try{SceneManager.SetActiveScene(scene);patch(scene);if(scene.isDirty)EditorSceneManager.SaveScene(scene);}
  finally{if(!loaded)EditorSceneManager.CloseScene(scene,true);if(previous.IsValid()&&previous.isLoaded)SceneManager.SetActiveScene(previous);}
 }
}
}
