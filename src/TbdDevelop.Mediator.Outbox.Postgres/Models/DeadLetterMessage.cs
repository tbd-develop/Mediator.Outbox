namespace TbdDevelop.Mediator.Outbox.Postgres.Models;

public class DeadLetterMessage
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime DateAdded { get; set; }
}