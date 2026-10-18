// ISortable.cs

namespace Yggdrasil.Models.Interfaces;

public interface ISortable{
    int ID { get; }
    string Name { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
}
