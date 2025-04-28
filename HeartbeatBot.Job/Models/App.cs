using HeartbeatBot.Job.Models.Enums;
using System.ComponentModel.DataAnnotations;

public class App
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string ApplicationName { get; set; }

    [Required]
    public string Url { get; set; }
    [Required]
    public bool IsLock { get; set; } = false;

    [Required]
    public HealtCheckType HealtCheckType { get; set; }
    [Required]
    public bool IsActive { get; set; } = false;
}
