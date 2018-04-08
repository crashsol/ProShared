using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProShare.ContactApi.Models;

namespace ProShare.ContactApi.Data
{
    public class MongoContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _context;

        public MongoContactApplyRequestRepository(ContactContext contactContext)
        {
            _context = contactContext;
        }
        
        public Task<bool> AddRequest(ContactApplyRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Approval(int applierId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetRequestList(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
