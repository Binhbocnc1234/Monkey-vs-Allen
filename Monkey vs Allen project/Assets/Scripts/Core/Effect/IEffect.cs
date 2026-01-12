using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CardEffectType {
    None,
    Costly,
    PauseAllenQueue,

}
/// <summary>
/// Trait là những thuộc tính vốn có của Entity. Mỗi Trait cho Entity một khả năng đặc biệt <br/>
/// Vì những trait dưới đây phổ biến, nhiều Entity sở hữu nó, nên tôi đặt cho chúng một cái tên
/// Trong trận đấu, chúng không biểu hiện cụ thể ra bên ngoài, để biết một Entity sở hũu những trait gì, bạn chỉ cần mở Almanac là xong
/// </summary>
public enum TraitType {
    Deadly = 0, // Darter
    Strikethrough = 1, // Terri, Paladin
    AntiTower = 2, // Valhein
    Frenzy = 3, // Enzo, Hungry Gargantuar
    Amphibious = 4, // many...
    Armored = 5, // Vanguard(1), Undying Pharaoh(1), Football Mecha(2), Supernova Gargantuar(2), Slug(1), Knight(3), Gladiator(1)
    BullEye = 6, // Counter of Armored, Fenick, 
    FrostBite = 7, // FrostQueen
    Splash = 8, // Watermelon seller, Slider, 
    LifeSteal = 9, // Vimpire, Vampire, Richter
    Flame = 10, // Dragon Cub, Yorn
    Thorn, // Parallax, ShieldCrusherViking. // Phản lại 50% sát thương nhận vào, Biểu hiện: khi nhận sát thương xuất hiện vòng tròn có gai nhọn bao quanh
    PreciseShot, // Elsu, Tower of the Sun,  Incandescent Core
    Venomy, // Snake Tower
    Blitz, // Zeus, Wizard, 
    SplashII, // Dark dragon tower, Slug
    Freeze, // Đóng băng mọi hành động, biểu hiện: cơ thể có màu xanh biển và dưới chân có kim băng
    Slow, // Di chuyển chậm hơn, biểu hiện: cơ thể có màu xanh biển
    OnFire, // Thiêu đốt mỗi 1 giây gây 3 sát thương, Biểu hiện: ngọn lửa nhỏ
    Immobilized, // Không thể di chuyển, dưới chân là các dây leo quấn lấy
    Poisoning, // Gây 2 sát thương phép sau mỗi 3 giây, Biểu hiện: nhấp nháy màu xanh
    Hypnotized, // Thôi miên, đổi thuộc tính team và thêm chút hiệu ứng đồ họa, Biểu hiện: cơ thể có màu tím, có bong bóng nổi lên
    Momentum, // Tăng tốc chạy 70%, Biểu hiện: chân có xung kích
    Excited, // Tăng 70% tốc đánh, Biểu hiện: đầu phát quang màu vàng
    Fury, // Tăng sát thương thêm 4, Biểu hiện: tay có ánh sáng đỏ
    LongReach, // Tăng tầm đánh thêm 3 đối với các tướng đánh xa, Biểu hiện: dưới chân có vòng hào quang màu xanh biển
}
/// <summary>
/// Hiệu ứng không phải thuộc tính vốn có của Entity mà nó do khách quan tác động vào. <br\>
/// Có những hiệu ứng là Tốt, có những hiệu ứng là Xấu(Debuff) <br\>
/// Mỗi effect có biểu hiện cụ thể trên Entity
/// </summary>
public abstract class IEffect : IUpdatePerFrame, IDestroyable {
    public bool isDebuff { get; protected set; }
    protected readonly int duration;
    public int strength{ get; protected set; }
    protected IEntity owner;
    protected Timer lifeTimer;
    private bool isDead;
    public IEffect(IEntity owner, int duration = -1, int strength = 1) {
        this.owner = owner;
        this.duration = duration;
        this.strength = strength;
        isDebuff = true;
        if(duration != -1) {
            lifeTimer = new Timer(duration, false);
        }
    }
    public virtual void Update() {
        if(lifeTimer != null && lifeTimer.Count()) {
            DestroyThis();
        }
    }
    public virtual int GetDangerPoint(){ return 0; }
    public void DestroyThis() {
        isDead = true;
    }
    public bool IsDead() => isDead;
    public virtual bool IsIdentical(IEffect effect) {
        return this.GetType() == effect.GetType();
    }
    public void ResetDuration() {
        if(duration != -1) {
            lifeTimer.Reset();
        }
    }
}
[Serializable]
public struct EffectSnapShot {
    public TraitType effectType;
    public float durationLeft;
    public int strength;
}
public interface IOnDamageTaken {
    public void OnDamageTaken(DamageContext ctx);
}
public interface IDamageInputModifier {
    public void ModifyDamage(DamageContext ctx);
}
public interface IDamageOutputModifier {
    public void ModifyDamage(DamageContext ctx);
}
public interface IStackable {
    public void Stack(int amount);
}
