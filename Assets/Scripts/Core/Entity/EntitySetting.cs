using System.Collections.Generic;

public class EntitySetting {
    public EntitySO so;
    public int lane;
    public float x;
    public Team team = Team.Left;
    public int level = 1;
    public bool isSimulated = false;
    public List<string> additionalSettings;
}