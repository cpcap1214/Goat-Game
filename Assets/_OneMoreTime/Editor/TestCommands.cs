using System.IO;using UnityEditor.TestTools.TestRunner.Api;using UnityEngine;
namespace OneMoreTime.EditorTools {
public static class TestCommands {
 static TestRunnerApi api;static string result;
 public static void Run(bool play){result="Artifacts/Logs/"+(play?"playmode":"editmode")+".xml";api=ScriptableObject.CreateInstance<TestRunnerApi>();api.RegisterCallbacks(new Callbacks());api.Execute(new ExecutionSettings(new Filter{testMode=play?TestMode.PlayMode:TestMode.EditMode,assemblyNames=new[]{play?"OneMoreTime.PlayTests":"OneMoreTime.EditTests"}}));}
 sealed class Callbacks:ICallbacks {
 public void RunStarted(ITestAdaptor t){}public void TestStarted(ITestAdaptor t){}public void TestFinished(ITestResultAdaptor r){}
 public void RunFinished(ITestResultAdaptor r){TestRunnerApi.SaveResultToFile(r,result);File.WriteAllText(result+".summary",$"total={r.PassCount+r.FailCount+r.SkipCount} passed={r.PassCount} failed={r.FailCount} skipped={r.SkipCount} status={r.TestStatus}");}
 }
}
}
