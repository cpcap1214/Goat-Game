using UnityEngine;using UnityEngine.SceneManagement;
namespace OneMoreTime {
public static class SceneFlow {
 public static readonly string[] Scenes={"MainMenu","L01_Freeze","L02_Pressure","L03_Trace","L04_Timing","Ending"};
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
