using UnityEngine;

public static class PhysicsConstants
{
    public const int TICKS_PER_SECOND = 60;

    public const int FP_DT = 16667;          // 1/60 in fixed-point

    public const int FP_SCALE = 1_000_000;

    public const int FP_DAMPING_FACTOR = 800000; // 0.999833 * 1_000_000

    // Gravity
    public const int FP_GRAVITY_X = 0;
    public const int FP_GRAVITY_Y = 0 /*-9_800_000*/;  // -9.8 * FP_SCALE
}
