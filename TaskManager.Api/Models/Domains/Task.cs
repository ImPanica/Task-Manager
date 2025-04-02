using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Api.Models.Domains;

public class Task : CommonObject
{
    [Key]
    public int Id { get; set; }
    public DateTime StartDate { get; set; }      
    public DateTime EndDate { get; set; }      
    public byte[]? File { get; set; }
    public int DeskId { get; set; }
    [ForeignKey("DeskId")]
    public Desk Desk { get; set; }
    public int ColumnId { get; set; }
    [ForeignKey("ColumnId")]
    public virtual Column Column { get; set; }
    public int? CreatorId { get; set; }
    [ForeignKey("CreatorId")]
    public virtual User? Creator { get; set; }
    public int? ExecutorId { get; set; }
    [ForeignKey("ExecutorId")]
    public virtual User? Executor { get; set; }
}