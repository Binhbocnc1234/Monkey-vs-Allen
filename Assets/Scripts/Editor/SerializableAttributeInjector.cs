// #if UNITY_EDITOR
// using UnityEditor;
// using System.IO;
// using System.Text.RegularExpressions;

// public class SerializableAttributeInjector : AssetPostprocessor
// {
//     static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, 
//                                        string[] movedAssets, string[] movedFromAssetPaths)
//     {
//         foreach (string path in importedAssets)
//         {
//             if (!path.EndsWith(".cs")) continue;

//             string content = File.ReadAllText(path);
            
//             // Tìm class kế thừa IBehavio
//             if (Regex.IsMatch(content, @"class\s+\w+\s*:\s*.*IBehaviour"))
//             {
//                 // Kiểm tra xem đã có [System.Serializable] chưa
//                 if (!content.Contains("[System.Serializable]") && 
//                     !content.Contains("[Serializable]"))
//                 {
//                     // Thêm attribute trước class definition
//                     content = Regex.Replace(content, 
//                         @"(class\s+\w+\s*:\s*.*IBehaviour)", 
//                         "[System.Serializable]\n    $1");
                    
//                     File.WriteAllText(path, content);
//                     AssetDatabase.Refresh();
//                 }
//             }
//         }
//     }
// }
// #endif