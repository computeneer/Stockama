namespace Stockama.Data.Domain;

public interface IEntity
{
   Guid Id { get; set; }
   bool IsDeleted { get; set; }
   bool IsActive { get; set; }
   DateTimeOffset CreatedAt { get; set; }
   Guid? CreatedBy { get; set; }
   DateTimeOffset? UpdatedAt { get; set; }
   Guid? UpdatedBy { get; set; }
}