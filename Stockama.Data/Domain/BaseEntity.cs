namespace Stockama.Data.Domain;

public class BaseEntity : IEntity
{
   public Guid Id { get; set; }
   public bool IsDeleted { get; set; } = false;
   public bool IsActive { get; set; } = true;
   public DateTimeOffset CreatedAt { get; set; }
   public Guid? CreatedBy { get; set; }
   public DateTimeOffset? UpdatedAt { get; set; }
   public Guid? UpdatedBy { get; set; }
}