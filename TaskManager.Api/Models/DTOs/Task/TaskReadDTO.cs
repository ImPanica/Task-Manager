using TaskManager.Api.Models.Domains;

namespace TaskManager.Api.Models.DTOs;

public class TaskReadDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreateDateTime { get; set; }
    public byte[]? File { get; set; }
    public byte[]? Photo { get; set; }

    public int DeskId { get; set; }
    public string DeskName { get; set; }

    public int ColumnId { get; set; }
    public string ColumnName { get; set; }

    public int? CreatorId { get; set; }
    public string CreatorName { get; set; }

    public int? ExecutorId { get; set; }
    public string ExecutorName { get; set; }
}