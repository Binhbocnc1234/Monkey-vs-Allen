DESIGN PURPOSES:
- Reduce compile time. To achieve that, a assembly(.asmdef) that other assembly depend on it should be stable
- Readability: the architecture should shout out what project Contains
- Minimal references between assemblies -> easier bug fixxing

PROJECT ARCHITECTURE
System-to-be:

Assemblies can freely use C# functionalities, MonoBehaviour, Transform, and other utilities provided by UnityEngine, because C# and UnityEngine is constant and does not change in the future. Below are some assemblies. 

This architecture is neither purely layered(grouping components by technical concerns) nor purely vertical(Classification of assemblies by function). It combines both. To understand why I choose hybrid approach. You can see following example, if I use pure layered architecture and modify one file in Domain:

recompile Domain
→ recompile Application
→ recompile Infrastructure
→ recompile Presentation
→ recompile Bootstrap

That’s expensive. Especially because Domain changes often in game dev.
Meanwhile feature-based:

Entity
Card
Effect
GridSystem
AI
UI

If I modify Effect: recompile Effect → only dependents of Effect

Here is my system-as-is structure:
Core: Contains abstract classes, which contain only fields and signatures and no logic, and C# documentation for important functions. Therefore, Core is an assembly that requires less recompilation and is ideal for other assemblies to depend on(reference to) and implement. The reason Core exists: because classes need to interact with each other, an environment is necessary. 
   - Entities: IEntity, EntityStat
   - Effects: IEffect
   - Cards: IBattleCard
   - ScriptableObjects: chứa thông tin, thiên về số liệu, có thể hiển thi cho người chơi và phục vụ mục đích cân bằng game

Application: This is not a Assembly, this is a folder/layer that contain assemblies. Application implements all abstract classes and contain a lot concrete Entity logic(such as Basic Monkey, MasterChef). Code in this assembly is volatile and often recomplied and having bugs. But fortunately, when one assembly in this folder recompiled, no other assembly affected. Here are some assemblies:
 - Entities: Entity.cs, EntityBehaviour.cs, Attack.cs, Move.cs, BasicMonkey.cs...
 - Cards: BattleCard(implements IBattleCard), 
 - GridSystem:
 - Effects: Stun, Frozen, Frenzy
 - AI: AIManager and its supporter

Presentation: This is not a Assembly, this is a folder/layer that contain assemblies. They depend on TextMeshPro and UnityEngine.UI
   - UI: những gì hiển thị lên để người chơi thấy
   - Presenters: là cầu nối giữa Bussiness logic với UI. Ví dụ khi tài nguyên thay đổi thì Presenters nhận được thông báo rồi truyền nó cho component UI
   - But in reality, separation is unnecessary; scripts often handle both functions: UI and Presenter, so just merge them
   - Model: describe how each types of Entity look like. They listen to Entity's action, then play corresponding animation. For example, when helmet's durability dropped to zero, the Model create a broken helmet that dropped to ground, and that for visual only

Infrastructure: depend on Core
   - SaveAndLoad: lưu dữ liệu người chơi vào hardware, để lần sau mở game thì dữ liệu từ lần chơi trước vẫn tồn tại
   - Factories: EntityFactory để tạo prefab Entity dựa trên EntitySO cung cấp
   - AudioServices
   - Ads 
   

   
- Bootstrap: depend on all assemblies. Containing very few scripts so that compilation time is minimized. This assembly is about managers(orchestrator) and bootstrapper(determine which components will be initialized and their order). For example:
   - BattleManager.cs
   - LobbyManager.cs

System-as-is:
Tổng quan: tương đối tuân theo, chưa phải toàn bộ
UI đang nằm rải rác.
Entity đang reference tới Model để điều khiển Model, điều này khiến hệ thống gặp trục trặc khi AI cần simulation mà không dùng tới Model
AlienWithHelmentInitializer.cs chứa cả logic nghiệp vụ và logic về presentation
GameConfig need to move to bootstrap 
UI folder depend on TraitUI, singleton, direction, 

Phase 1: Chốt nguyên tắc dependency direction và freeze graph hiện tại, không cho thêm edge mới từ domain sang UI.
Phase 1: Dọn nhanh các using UI/TMP không dùng trong assembly không-UI để giảm coupling rác.
Phase 2: Tạo assembly contract ổn định (interface, abstract, value-object ít đổi), không chứa MonoBehaviour.
Phase 2: Chuyển domain assembly phụ thuộc contract thay vì phụ thuộc chéo type definition.
Phase 3: Cô lập UI/TMP vào presentation; domain chỉ phát event/state, UI subscribe để render.
Phase 3: Tạo assembly simulation thuần C# (no UnityEngine) cho AI forecast và battle prediction.
Phase 4: Giữ MonoBehaviour ở vai trò adapter mỏng; animation/view tiêu thụ kết quả logic, không quyết định logic cốt lõi.
Phase 4: Giảm fan-out từ framework/core bằng cách tách phần đổi thường xuyên sang assembly ngoài lõi.
Phase 5: Áp checklist kiến trúc bắt buộc ở PR review để tránh trôi kiến trúc.

PHASE 1 - Dependency Direction Freeze (Domain -> UI ban)

Goal
- Lock dependency direction to reduce compile cascades and reduce coupling.
- Freeze the current dependency graph and do not allow any new edge from domain to UI.

Dependency direction principles
- UI is the outermost layer (presentation) and only consumes data/events from domain.
- Domain/Core/Entities/Cards/Battlefield/Grid/Effect/Event must not depend on UI.
- Assemblies in inner layers must not reference assemblies in outer layers.
- Assemblies that contain contracts/interfaces/abstract types must stay stable, change rarely, and must not depend on rendering.

Mandatory rules (effective immediately)
- Do not add any new reference to MvA.UI in domain asmdefs.
- Do not add any new reference to Unity.TextMeshPro in domain asmdefs.
- Do not add UI/TMP namespaces in domain classes (for example: UnityEngine.UI, TMPro).
- If display is needed, domain publishes events/state; UI assemblies subscribe and render.

List of prohibited new edges
- MvA.Entities -> MvA.UI (no new additions; target is removal)
- MvA.Cards -> MvA.UI (no new additions; target is removal)
- MvA.Battlefield -> MvA.UI (no new additions; target is removal)
- MvA.Cards -> Unity.TextMeshPro (no new additions; target is removal)
- MvA.Battlefield -> Unity.TextMeshPro (no new additions; target is removal)

Phase 1 pass/fail criteria
- PASS: No domain asmdef adds any new dependency to MvA.UI or Unity.TextMeshPro.
- PASS: New domain code does not use UI/TMP namespaces.
- FAIL: Any PR that adds a domain -> UI/TMP edge without an approved exception.

Temporary exception policy
- Exceptions are allowed only with a clear technical reason and a removal deadline.
- Every exception must include a migration issue and due date.
- No exception may expand its scope to other assemblies.

Execution notes
- Starting from Phase 1, every UI need in domain must go through event/adapter.
- Next objective (Phase 2): extract stable contracts to reduce fan-out from Core/Framework.

Archived Prompt:
Mình muốn project của mình tuân theo clean architecture nhưng Hiện tại có một vấn đề: nếu mình để Entity không kế thừa MonoBehaviour thì mình không thể quan sát các field của nó trên Inspector. Còn nếu để Entity kế thừa MonoBehaviour thì việc simulation sẽ trở nên khó khăn hơn