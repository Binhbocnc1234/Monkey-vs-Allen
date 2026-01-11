[System.Serializable]
public class LevelProgress {
    public enum State {
        Incomplete,
        Completed,
        Visible,
        Paused,
    }
    public State state;
    
}