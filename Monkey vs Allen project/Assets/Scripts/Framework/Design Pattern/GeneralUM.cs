
public class GeneralUM : UpdateManager<IUpdatePerFrame> {
    void Awake() {
        SingletonRegister.Register(this);
    }
}