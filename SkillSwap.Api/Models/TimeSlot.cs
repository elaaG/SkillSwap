namespace SkillSwap.API.Models;

public class TimeSlot
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public TimeSlot() {}

    public TimeSlot(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new Exception("Invalid time slot");

        Start = start;
        End = end;
    }
}