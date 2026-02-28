namespace TbdDevelop.Mediator.Outbox.Postgres.Models;

public class OutboxMessage
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int Retries { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateProcessed { get; set; }
}