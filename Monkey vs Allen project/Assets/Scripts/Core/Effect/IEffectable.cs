using System.Collections.Generic;

public interface IEffectable{
    public void ApplyEffect(IEffect effect);
    public void RemoveEffect(IEffect effect);
}