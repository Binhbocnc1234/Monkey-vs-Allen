

public class TextTutorial : Tutorial{
    public Timer timer;
    public override void Initialize(){
        base.Initialize();
        StartTutorial();
        timer = new Timer(4, true);
    } 
    TextTutorial SetText(string text){
        this.text = text;
        return this;
    }
    void Update(){
        if (timer.Count()){
            CompleteTutorial();
        }
    }
}