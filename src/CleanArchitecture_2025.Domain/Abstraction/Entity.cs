﻿namespace CleanArchitecture_2025.Domain.Abstraction
{
    public abstract class Entity
    {
        public Entity()
        {
            Id = Guid.CreateVersion7();
        }
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Guid CreateUserId { get; set; } = default!;
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? UpdateUserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Guid? DeleteUserId { get; set; }
    }
}
