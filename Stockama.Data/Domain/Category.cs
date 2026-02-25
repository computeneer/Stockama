namespace Stockama.Data.Domain;

public class Category : BaseEntity
{
   public string Name { get; set; }

   public Guid? ParentId { get; set; } = null;

   // Navigation Properties
   public Category Parent { get; set; }
}