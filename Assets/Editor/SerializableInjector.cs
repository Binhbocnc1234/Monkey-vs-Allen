// #if UNITY_EDITOR
// using System.IO;
// using System.Linq;
// using Unity.CompilationPipeline.Common.ILPostProcessing;
// using Mono.Cecil; // Bạn cần thêm package hoặc sử dụng thư viện Cecil đi kèm của Unity

// public class SerializableInjector : ILPostProcessor
// {
//     public override ILPostProcessor GetInstance() => this;

//     // Định nghĩa assembly nào sẽ bị quét (ví dụ: Assembly-CSharp)
//     public override bool WillProcess(ICompiledAssembly compiledAssembly)
//     {
//         return compiledAssembly.Name == "Assembly-CSharp";
//     }

//     public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
//     {
//         if (!WillProcess(compiledAssembly)) return null;

//         // Đọc dữ liệu Assembly bằng Mono.Cecil
//         var stream = new MemoryStream(compiledAssembly.InMemoryAssembly.PeData);
//         var symbols = new MemoryStream(compiledAssembly.InMemoryAssembly.PdbData);
//         var assemblyDef = AssemblyDefinition.ReadAssembly(stream, new ReaderParameters
//         {
//             ReadSymbols = true,
//             SymbolStream = symbols
//         });

//         bool modified = false;
//         var mainModule = assemblyDef.MainModule;

//         // Tìm reference tới System.SerializableAttribute
//         var serializableAttrType = typeof(System.SerializableAttribute);
//         var serializableAttrRef = mainModule.ImportReference(serializableAttrType);
//         var serializableConstructor = mainModule.ImportReference(serializableAttrType.GetConstructor(System.Type.EmptyTypes));

//         // Duyệt qua tất cả các class trong dự án
//         foreach (var typeDef in mainModule.Types)
//         {
//             if (!typeDef.IsClass || typeDef.IsAbstract) continue;

//             // Kiểm tra xem class này có kế thừa từ IBehaviour không (bằng cách duyệt chuỗi BaseType)
//             var currentBase = typeDef.BaseType;
//             bool inheritsFromBehaviour = false;
//             while (currentBase != null)
//             {
//                 if (currentBase.Name == "IBehaviour") 
//                 {
//                     inheritsFromBehaviour = true;
//                     break;
//                 }
//                 try { currentBase = currentBase.Resolve()?.BaseType; } catch { break; }
//             }

//             // Nếu đúng là lớp con và CHƯA có [Serializable]
//             if (inheritsFromBehaviour && !typeDef.CustomAttributes.Any(a => a.AttributeType.FullName == serializableAttrType.FullName))
//             {
//                 // Thêm [System.Serializable] vào class này một cách tự động
//                 var customAttribute = new CustomAttribute(serializableConstructor);
//                 typeDef.CustomAttributes.Add(customAttribute);
//                 modified = true;
//             }
//         }

//         if (!modified) return null;

//         // Ghi lại file assembly đã sửa đổi vào bộ nhớ của Unity
//         var peStream = new MemoryStream();
//         var pdbStream = new MemoryStream();
//         assemblyDef.Write(new WriterParameters { WriteSymbols = true, SymbolStream = pdbStream }, peStream);

//         return new ILPostProcessResult(new InMemoryAssembly(peStream.ToArray(), pdbStream.ToArray()));
//     }
// }
// #endif