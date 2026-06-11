using UnityEngine;

public class EntityModelRegistry : MonoBehaviour
{
    public static EntityModelRegistry Ins { get; private set; }

    public Transform holder;

    void Awake() {
        Ins = this;
    }

    private void OnEnable() {
        IEntityRegistry.OnEntityCreated += HandleEntityCreated;
    }

    private void OnDisable() {
        IEntityRegistry.OnEntityCreated -= HandleEntityCreated;
    }

    private void HandleEntityCreated(IEntity e) {
        if (!e.isSimulated) {
            e.model = CreateInstance(e);
        }
    }

    private Model CreateInstance(IEntity e) {
        // Instantiate prefab at world position, parent to holder
        GameObject prefabInstance = Instantiate(e.GetSO().prefab, IGrid.Ins.GridToWorldPosition(e.gridPos), Quaternion.identity, holder);
        // Bind Entity to its visual via Model (the central wrapper component)
        EntityModel model = prefabInstance.GetComponent<EntityModel>();
        if(model != null) {
            model.AssignEntity(e);
        }
        SingletonRegister.Get<ShadowContainer>().Get().Initialize(e);
        return model;
    }
}