using System.Collections.Generic;
using System;
public interface IEffectable{
    public void ApplyEffect(Effect effect);
    public void RemoveEffect(Effect effect);
    public bool HaveEffect(Type type);
    public void ProcessDamageOutput(DamageContext ctx);
    public void ProcessDamageInput(DamageContext ctx);
    public void ProcessDamageTaken(DamageContext ctx);
    public void NotifyOnAssistOrKill();
}
