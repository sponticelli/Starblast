namespace Starblast
{
    public static class ExecutionOrder
    {
        // Execution order
        public const int ServiceLocator = -1000;
        public const int ServiceLocatorStrapper = ServiceLocator + 1;
        public const int TransientServiceRegister = ServiceLocatorStrapper + 1;
        

        public const int Services = -100;
        public const int PoolManager = Services + 1;
        public const int RegisterToRuntimeSet = PoolManager + 1;
    }
}