using System;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Core.Domain
{
    /// <summary>Person who can be assigned tasks.</summary>
    public sealed class Assignee
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress, StringLength(120)]
        public string? Email { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
