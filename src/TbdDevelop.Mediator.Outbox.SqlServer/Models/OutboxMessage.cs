namespace TbdDevelop.Mediator.Outbox.SqlServer.Models;

public class OutboxMessage
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime DateAdded { get; set; }
    public DateTime? DateProcessed { get; set; }
}