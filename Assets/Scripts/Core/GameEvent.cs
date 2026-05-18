
using System;
public static class GameEvents {
    public static event Action<int> OnCardCostChange;
    public static void AdjustCostChangeGlobally(int delta) => OnCardCostChange?.Invoke(delta);
    
}
