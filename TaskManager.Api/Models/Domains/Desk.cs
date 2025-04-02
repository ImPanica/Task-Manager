using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Api.Models.Domains.Domains;

namespace TaskManager.Api.Models.Domains;

public class Desk : CommonObject
{
    [Key]
    public int Id { get; set; }
    public bool IsPrivate { get; set; }
    
    public virtual ICollection<Column> Columns { get; set; } = new List<Column>();
    
    public int? AdminId { get; set; }
    [ForeignKey("AdminId")]
    public virtual User Admin { get; set; }
    
    public int? ProjectId { get; set; }
    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }
    
    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}