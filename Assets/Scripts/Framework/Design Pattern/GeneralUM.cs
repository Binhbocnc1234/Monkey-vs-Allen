/// <summary>
/// Dành cho những object không có MonoBehaviour, không có transform nhưng vẫn muốn cập nhật qua từng frame
/// </summary>
public interface IUpdatePerFrame{
    public void Update();
}
public class GeneralUM : UpdateManager<IUpdatePerFrame> {
    public static GeneralUM Ins{ get; private set; }
    void Awake() {
        Ins = this;
    }
    public new void AddElement(IUpdatePerFrame element) {
        base.AddElement(element);
    }
    public new void RemoveElement(IUpdatePerFrame element) {
        base.RemoveElement(element);
    }
}