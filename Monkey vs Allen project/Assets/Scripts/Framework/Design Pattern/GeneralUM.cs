
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