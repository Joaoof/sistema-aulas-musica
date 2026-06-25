namespace PortalAluno.Application.Common;

public static class CacheKeys
{
    public static string StudentDashboard(Guid studentId) => $"dashboard:student:{studentId}";
}
