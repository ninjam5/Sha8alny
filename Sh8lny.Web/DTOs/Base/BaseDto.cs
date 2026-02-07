namespace Sh8lny.Web.DTOs.Base;

/// <summary>
/// Generic abstract base class for DTOs with a configurable ID type.
/// Provides standard audit properties for all derived DTOs.
/// </summary>
/// <typeparam name="TId">The type of the identifier (e.g., int, Guid, string).</typeparam>
public abstract class BaseDto<TId>
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public TId Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Non-generic base DTO class for standard integer-based entities.
/// Inherits from BaseDto&lt;int&gt; for convenience.
/// </summary>
public abstract class BaseDto : BaseDto<int>
{
}
