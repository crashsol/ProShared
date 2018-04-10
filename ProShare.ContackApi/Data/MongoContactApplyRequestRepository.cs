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

        public async Task<bool> AddRequestAsync(ContactApplyRequest request, CancellationToken cancellationToken)
        {
            //检查是否已经存在好友申请请求
            if((await _context.ContactApplyRequests.CountAsync(dd => dd.UserId == request.UserId && dd.ApplierId == request.ApplierId) ==1))
            {
                //条件
                var filter = Builders<ContactApplyRequest>.Filter.And(
                       Builders<ContactApplyRequest>.Filter.Eq(b=>b.ApplierId,request.ApplierId),
                       Builders<ContactApplyRequest>.Filter.Eq(b =>b.UserId,request.UserId)
                    );
                //如果存在,更新申请时间
                var update = Builders<ContactApplyRequest>.Update.Set(b => b.ApplyTime, DateTime.Now);
                var result = await _context.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);
                return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;
            }
            //进行新增操作
            await _context.ContactApplyRequests.InsertOneAsync(request,null,cancellationToken);
            return true;
            
        }

        public async Task<bool> ApprovalAsync(int userId,int applierId, CancellationToken cancellationToken)
        {
            var filter = Builders<ContactApplyRequest>.Filter.And(
                            Builders<ContactApplyRequest>.Filter.Eq(b =>b.UserId,userId),
                            Builders<ContactApplyRequest>.Filter.Eq(b =>b.ApplierId, applierId)
                       );
            //更新添加好友申请，修改状态和添加处理时间
            var update = Builders<ContactApplyRequest>.Update
                            .Set(b => b.Approvaled, 1)
                            .Set(b => b.HandleTime, DateTime.Now);
            var result =await _context.ContactApplyRequests.UpdateOneAsync(filter, update, null, cancellationToken);

            return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;
        }
               
        public async Task<List<ContactApplyRequest>> GetRequestList(int userId,CancellationToken cancellationToken)
        {            
            var requests = await _context.ContactApplyRequests.FindAsync(b => b.UserId == userId);
            return requests.ToList(cancellationToken);
        }
    }
}
