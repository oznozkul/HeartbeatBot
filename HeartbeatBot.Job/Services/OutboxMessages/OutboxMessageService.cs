using Microsoft.EntityFrameworkCore;
using HeartbeatBot.Job.Context;
using HeartbeatBot.Job.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeartbeatBot.Job.Services.OutboxMessages
{
    internal class OutboxMessageService : IOutboxMessageService
    {
        public async Task<IEnumerable<OutboxMessage>> GetAllAsync()
        {
            using (var db = new HealtCheckContext())
            {
                return await db.OutboxMessages.Include(o => o.Application).Where(x => !x.IsSent).ToListAsync();
            }
        }

        public async Task<OutboxMessage> GetByIdAsync(int id)
        {
            using (var db = new HealtCheckContext())
            {
                return await db.OutboxMessages.Include(o => o.Application)
                                              .FirstOrDefaultAsync(o => o.Id == id);
            }
        }

        public async Task<OutboxMessage> CreateAsync(OutboxMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            using (var db = new HealtCheckContext())
            {
                db.OutboxMessages.Add(message);
                await db.SaveChangesAsync();
                return message;
            }
        }

        public async Task<bool> UpdateAsync(OutboxMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            using (var db = new HealtCheckContext())
            {
                var existingMessage = await db.OutboxMessages.FindAsync(message.Id);
                if (existingMessage == null) return false;

                existingMessage.ApplicationId = message.ApplicationId;
                existingMessage.Message = message.Message;
                existingMessage.IsSent = message.IsSent;

                await db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var db = new HealtCheckContext())
            {
                var message = await db.OutboxMessages.FindAsync(id);
                if (message == null) return false;

                db.OutboxMessages.Remove(message);
                await db.SaveChangesAsync();
                return true;
            }
        }

        public async Task ChangeStatus(int id)
        {
            using (var db = new HealtCheckContext())
            {
                var message = await db.OutboxMessages.FindAsync(id);
                if (message != null)
                {
                    message.IsSent = true;
                    db.OutboxMessages.Update(message);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
