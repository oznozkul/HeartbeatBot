using HeartbeatBot.Job.Models;

namespace HeartbeatBot.Job.Services.OutboxMessages
{
    public interface IOutboxMessageService
    {
        Task<IEnumerable<OutboxMessage>> GetAllAsync();
        Task<OutboxMessage> GetByIdAsync(int id);
        Task<OutboxMessage> CreateAsync(OutboxMessage message);
        Task<bool> UpdateAsync(OutboxMessage message);
        Task<bool> DeleteAsync(int id);
        Task ChangeStatus(int id);
    }
}
