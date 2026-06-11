using Unity.CodeEditor;
using UnityEditor;

public static class TestWorkflowProjectFiles {
    public static void Generate() {
        CodeEditor.CurrentEditor.SyncAll();
        AssetDatabase.Refresh();
    }
}
