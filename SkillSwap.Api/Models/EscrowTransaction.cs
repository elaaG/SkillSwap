namespace SkillSwap.Api.Models;

public class EscrowTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal Amount { get; set; }
    public EscrowStatus Status { get; set; } = EscrowStatus.Hold;
}