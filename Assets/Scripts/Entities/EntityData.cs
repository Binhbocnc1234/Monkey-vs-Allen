using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Class này gói gọn những thông tin cần phải lưu để phục vụ save and load
/// </summary>
[System.Serializable]
public class EntityData {
    public string soId;
    public Team team;
    public bool isDead;
    public string animationName;
    public float animationProgress; // normalized
    public int lane;
    public float x;
    [JsonProperty] private List<EntityStat> stats;
    public List<Effect> effects;
    public EntityStat this[ST st] {
        get {
            foreach(EntityStat stat in stats) {
                if(stat.GetStatType() == st) {
                    return stat;
                }
            }
            return null;
        }
    }
    public EntitySO GetSO() => SORegistry.Get<EntitySO>(soId);
}