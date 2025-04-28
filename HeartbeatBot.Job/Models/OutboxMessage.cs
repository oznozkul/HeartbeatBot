using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeartbeatBot.Job.Models
{

    public class OutboxMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public string Message { get; set; }

        public bool IsSent { get; set; } = false; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ApplicationId")]
        public virtual App Application { get; set; }
    }

}
