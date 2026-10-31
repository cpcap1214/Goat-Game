using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace OneMoreTime.EditorTools {
// Explicit migration: ordinary Generate preserves subsequent Inspector edits.
public static class ChallengeLayouts {
 [MenuItem("Tools/One More Time/Apply Four Challenge Layouts")]
 public static void Apply() {
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play first");
  string backup="Artifacts/SceneBackups/Layouts-"+DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
  Directory.CreateDirectory(backup);
  foreach(string name in new[]{"L01_Freeze","L02_Pressure","L03_Trace","L04_Timing"}) {
   string path=DemoBuilder.Root+"/Config/"+name+".asset";
   if(File.Exists(path))File.Copy(path,backup+"/"+name+".asset",true);
   var c=DemoBuilder.Asset<LevelConfig>(name);
   c.initialized=true;c.layoutRevision=2;c.killY=-5;c.spawn=new Vector2(2,0);
   c.plates=new Vector2[0];c.doors=new Rect[0];c.doorSources=new int[0];c.crumbles=new Rect[0];c.hazards=new Rect[0];
   c.cameraSize=6;c.cameraY=3.5f;c.cameraMin=8;c.cameraMax=24;
   switch(name) {
    case "L01_Freeze":
     c.level=1;c.title="留在半途";c.hint="斷崖沒有落腳處。跳出去，在下一輪還踩得到的位置按 E。";
     c.goal=new Rect(26,4.3f,1,2);
     c.terrain=new[]{new Rect(-2,-3,9,3),new Rect(11,-3,7,7.3f),new Rect(21,-3,8,7.3f),new Rect(-2,0,1,12)};
     c.hazards=new[]{new Rect(7,-2,4,.35f),new Rect(18,-2,3,.35f)};
     break;
    case "L02_Pressure":
     c.level=2;c.title="一個位置，兩個任務";c.hint="先登上踏板石座。定格要同時壓住 A，並讓你跳上門前高台。";
     c.goal=new Rect(32,4.9f,1,2);c.cameraMax=29;
     c.terrain=new[]{new Rect(-2,-3,9,3),new Rect(9,-3,5,4.4f),new Rect(14,-3,5,7.9f),new Rect(22,-3,4,7.9f),new Rect(29,-3,7,7.9f),new Rect(14,9,22,1),new Rect(-2,0,1,12),new Rect(36,0,1,12)};
     c.plates=new[]{new Vector2(12.8f,1.4f)};c.doors=new[]{new Rect(15,4.9f,.6f,4.1f)};
     c.hazards=new[]{new Rect(7,-2,2,.35f),new Rect(19,-2,3,.35f),new Rect(26,-2,3,.35f)};
     break;
    case "L03_Trace":
     c.level=3;c.title="搭上移動的昨天";c.hint="在左岸 Q 錄製：等候上車，跳到右岸，再等一下、長跳，最後 Q 保存。";
     c.goal=new Rect(32,6.3f,1,2);c.cameraMax=29;c.cameraSize=6.5f;c.cameraY=4;
     c.terrain=new[]{new Rect(-2,-3,14,3),new Rect(16,-3,6,3),new Rect(23,-3,13,9.3f),new Rect(-2,0,1,14)};
     c.hazards=new[]{new Rect(12,-2,4,.35f),new Rect(22,-2,1,.35f)};
     break;
    case "L04_Timing":
     c.level=4;c.title="等我，再回來";c.hint="下層錄下 A → B → A，各留一段等待；上層跨過斷橋，在安全島等下一扇門。";
     c.spawn=new Vector2(2,5);c.goal=new Rect(37,5,1,2);c.cameraSize=7;c.cameraY=4;c.cameraMax=32;
     c.terrain=new[]{new Rect(-2,-2,43,2),new Rect(-2,0,1,13),new Rect(41,0,1,13),new Rect(-2,4.5f,5,.5f),new Rect(5,4.5f,8,.5f),new Rect(21,4.5f,6,.5f),new Rect(32,4.5f,9,.5f),new Rect(5,3.5f,36,1),new Rect(5,10,36,1)};
     c.plates=new[]{new Vector2(6,0),new Vector2(16,0)};
     c.doors=new[]{new Rect(11,5,.6f,5),new Rect(25,5,.6f,5),new Rect(35,5,.6f,5)};c.doorSources=new[]{0,1,0};
     c.crumbles=new[]{new Rect(14.5f,4.8f,2,.2f),new Rect(18.5f,4.8f,1.5f,.2f),new Rect(28.5f,4.8f,1.5f,.2f)};
     c.hazards=new[]{new Rect(13,4.45f,8,.28f),new Rect(27,4.45f,5,.28f)};
     break;
   }
   EditorUtility.SetDirty(c);
  }
  AssetDatabase.SaveAssets();DemoBuilder.Generate();
 }
}
}
