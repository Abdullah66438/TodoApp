using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Core.Domain
{
    /// <summary>Domain model for a task.</summary>
    public sealed class ProjectTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(120)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>UTC due date (nullable)</summary>
        public DateTime? DueDate { get; set; }

        /// <summary>Nullable reference to an Assignee.</summary>
        public Guid? AssigneeId { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Normal;
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
