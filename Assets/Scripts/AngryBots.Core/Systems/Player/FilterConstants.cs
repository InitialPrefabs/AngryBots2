namespace AngryBots2.Core {

    public static class Filters { 

        public const uint PlayerMask = 1u << 0;
        public const uint GroundMask = 1u << 1;
        public const uint PropMask   = 1u << 2;
        public const uint RayMask    = 1u << 31;
        public const uint Everything = ~0u;
    }
}
