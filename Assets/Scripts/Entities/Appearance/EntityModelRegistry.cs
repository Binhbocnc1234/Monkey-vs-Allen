using UnityEngine;
public class EntityModelRegistry : IEntityModelRegistry
{
    // public static IEntityModelRegistry Ins { get; protected set; }
    public Transform holder;
    void Awake() {
        Ins = this;
    }
    public override Model CreateInstance(IEntity e) {
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