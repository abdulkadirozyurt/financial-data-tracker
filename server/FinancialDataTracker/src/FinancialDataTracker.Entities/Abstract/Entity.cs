using System;

namespace FinancialDataTracker.Entities.Abstract;

public abstract class Entity
{
    public Entity()
    {
        Id = Guid.CreateVersion7();
    }

    public Guid Id { get; set; }
}
