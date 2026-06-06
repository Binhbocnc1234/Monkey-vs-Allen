using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AutoSaveEditor 
{
    // Khoảng thời gian giữa các lần lưu (ví dụ: 90 giây)
    private const float AutoSaveInterval = 60f;

    // Biến tĩnh để theo dõi thời gian của lần lưu cuối cùng
    private static float lastSaveTime = 0f;

    // Biến tĩnh để kiểm tra xem AutoSave đã được bật chưa
    private static bool isAutoSaveEnabled = false;

    // Hàm được gọi khi Unity khởi động hoặc script được compile
    [InitializeOnLoadMethod]
    private static void InitializeAutoSave()
    {
        // Do not run autosave in headless/batch runs (e.g., CI test execution)
        if (Application.isBatchMode)
        {
            return;
        }

        // Gán event handler chỉ một lần
        if (!isAutoSaveEnabled)
        {
            // Đăng ký phương thức UpdateAutoSave vào event update của Editor
            EditorApplication.update += UpdateAutoSave;
            isAutoSaveEnabled = true;
            Debug.Log("🎉 AutoSave Editor đã được kích hoạt.");
        }
    }

    // Phương thức được gọi trên mỗi lần làm mới khung hình của Editor
    private static void UpdateAutoSave()
    {
        // Tránh chạy khi đang ở chế độ Play
        if (Application.isPlaying)
        {
            return;
        }

        // Kiểm tra xem đã đủ thời gian để lưu chưa
        if (Time.realtimeSinceStartup - lastSaveTime >= AutoSaveInterval)
        {
            PerformSave();
            lastSaveTime = Time.realtimeSinceStartup;
        }
    }

    // Thực hiện chức năng lưu
    private static void PerformSave()
    {
        // 1. Lưu các Scene đang mở
        EditorSceneManager.SaveOpenScenes();

        // 2. Lưu tất cả các Asset đã thay đổi
        AssetDatabase.SaveAssets();

        Debug.Log("💾 **[AutoSave]** Scene và Assets đã được lưu tự động.");
    }
}