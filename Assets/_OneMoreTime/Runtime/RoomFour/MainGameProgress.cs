namespace OneMoreTime {
// Main rooms currently keep ability progression for the running journey, not a disk save.
public static class MainGameProgress {
 public static bool BasicReplayUnlocked {get;private set;}
 public static bool ReplayUpgradeUnlocked {get;private set;}
 public static bool IsMainJourney {get;private set;}
 public static bool IsDebugMode {get;private set;}
 public static int DeathCount {get;private set;}
 public static void GrantReplay(){BasicReplayUnlocked=true;}
 public static void GrantReplayUpgrade(){if(!IsDebugMode)return;BasicReplayUnlocked=true;ReplayUpgradeUnlocked=true;}
 public static void BeginDebug(){IsMainJourney=false;IsDebugMode=true;BasicReplayUnlocked=false;ReplayUpgradeUnlocked=false;}
 public static void StartOver(){BasicReplayUnlocked=false;ReplayUpgradeUnlocked=false;DeathCount=0;IsMainJourney=true;IsDebugMode=false;}
 public static void EndJourney(){IsMainJourney=false;}
 // Leaving for the menu rearms Room 5's acquisition; only New Game clears deaths.
 public static void ReturnToMenu(){BasicReplayUnlocked=false;ReplayUpgradeUnlocked=false;IsDebugMode=false;EndJourney();}
 public static void RecordDeath(){if(IsMainJourney&&DeathCount<int.MaxValue)DeathCount++;}
 [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
 static void ResetSession(){BasicReplayUnlocked=false;ReplayUpgradeUnlocked=false;DeathCount=0;IsMainJourney=false;IsDebugMode=false;}
}
}
