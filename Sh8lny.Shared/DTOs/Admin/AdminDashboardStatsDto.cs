namespace Sh8lny.Shared.DTOs.Admin;

/// <summary>
/// DTO containing admin dashboard statistics.
/// </summary>
public class AdminDashboardStatsDto
{
    // User Statistics
    /// <summary>
    /// Total number of registered students.
    /// </summary>
    public int TotalStudents { get; set; }

    /// <summary>
    /// Total number of registered companies.
    /// </summary>
    public int TotalCompanies { get; set; }

    /// <summary>
    /// Total number of registered users (all types).
    /// </summary>
    public int TotalUsers { get; set; }

    /// <summary>
    /// Number of active (non-banned) users.
    /// </summary>
    public int ActiveUsers { get; set; }

    /// <summary>
    /// Number of banned users.
    /// </summary>
    public int BannedUsers { get; set; }

    // Project Statistics
    /// <summary>
    /// Total number of projects.
    /// </summary>
    public int TotalProjects { get; set; }

    /// <summary>
    /// Number of active (open) projects.
    /// </summary>
    public int ActiveProjects { get; set; }

    /// <summary>
    /// Number of closed/completed projects.
    /// </summary>
    public int ClosedProjects { get; set; }

    // Application Statistics
    /// <summary>
    /// Total number of applications.
    /// </summary>
    public int TotalApplications { get; set; }

    /// <summary>
    /// Number of completed applications.
    /// </summary>
    public int CompletedApplications { get; set; }

    // Financial Statistics
    /// <summary>
    /// Total transaction volume (sum of all completed payments).
    /// </summary>
    public decimal TotalTransactionVolume { get; set; }

    /// <summary>
    /// Total number of transactions.
    /// </summary>
    public int TotalTransactions { get; set; }

    // Recent Activity
    /// <summary>
    /// Number of new users registered in the last 30 days.
    /// </summary>
    public int NewUsersLast30Days { get; set; }

    /// <summary>
    /// Number of new projects created in the last 30 days.
    /// </summary>
    public int NewProjectsLast30Days { get; set; }
}
