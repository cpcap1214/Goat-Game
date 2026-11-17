using UnityEngine;using UnityEngine.SceneManagement;
namespace OneMoreTime {
public static class SceneFlow {
 public static readonly string[] Scenes={"MainMenu","L01_Freeze","L02_Pressure","L03_Trace","L04_Timing","Ending"};
 public static void LoadWorldOneGallery(){Time.timeScale=1;SceneManager.LoadScene("Debug_WorldOneArt");}
 static bool upgradedReplayRequested;
 public static bool ConsumeReplayUpgrade(){bool requested=upgradedReplayRequested;upgradedReplayRequested=false;return requested;}
 public static void LoadRewindPlaytest(bool upgraded=false){upgradedReplayRequested=upgraded;Time.timeScale=1;
 #if UNITY_EDITOR
 if(!Application.CanStreamedLevelBeLoaded(RewindPlaytest.SceneName)){UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+RewindPlaytest.SceneName+".unity",new LoadSceneParameters(LoadSceneMode.Single));return;}
 #endif
 SceneManager.LoadScene(RewindPlaytest.SceneName);}
 public static void LoadMainRoom(){Time.timeScale=1;
 #if UNITY_EDITOR
 if(!Application.CanStreamedLevelBeLoaded(MainRoom.SceneName)){UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+MainRoom.SceneName+".unity",new LoadSceneParameters(LoadSceneMode.Single));return;}
 #endif
 SceneManager.LoadScene(MainRoom.SceneName);}
 public static void LoadChapter(){Time.timeScale=1;
 #if UNITY_EDITOR
 if(!Application.CanStreamedLevelBeLoaded(ChapterLayout.Scene)){UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+ChapterLayout.Scene+".unity",new LoadSceneParameters(LoadSceneMode.Single));return;}
 #endif
 SceneManager.LoadScene(ChapterLayout.Scene);}
 public static void Load(int index){Time.timeScale=1;
 #if UNITY_EDITOR
 if(!Application.CanStreamedLevelBeLoaded(Scenes[index])){UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode("Assets/_OneMoreTime/Scenes/"+Scenes[index]+".unity",new LoadSceneParameters(LoadSceneMode.Single));return;}
 #endif
 SceneManager.LoadScene(Scenes[index]);}
}
}
