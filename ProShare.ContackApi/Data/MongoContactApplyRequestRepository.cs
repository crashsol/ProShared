using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProShare.ContactApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading;

namespace ProShare.ContactApi.Data
{
    public class MongoContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _context;

        public MongoContactApplyRequestRepository(ContactContext contactContext)
        {
            _context = contactContext;
        }

        public Task<bool> AddRequest(ContactApplyRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Approval(int applierId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ContactApplyRequest>> GetRequestList(int userId,CancellationToken cancellationToken)
        {
            
            var requests = await _context.ContactApplyRequests.FindAsync(b => b.UserId == userId);
            return requests.ToList(cancellationToken);
        }
    }
}
