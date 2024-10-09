namespace Starblast.Environments.Boundaries
{
    public interface IBoundaryWarningSystem
    {
        void ResetWarnings();
        void DisplayWarning();
        void DisplayDangerWarning();
        void DisplayDeadWarning();
    }
}