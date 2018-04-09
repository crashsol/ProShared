using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProShare.ContactApi.Models.Dtos;
using MongoDB.Driver;
using ProShare.ContactApi.Models;

namespace ProShare.ContactApi.Data
{
    public class MongoContactBookRepository : IContactBookRepository
    {

        private readonly ContactContext _context;

        public MongoContactBookRepository(ContactContext contactContext)
        {
            _context = contactContext;
        }
        public async Task<bool> UpdateUserInfoAsync(BaseUserInfo baseUserInfo, CancellationToken cancellationToken)
        {
            //查询要更新用户的通讯录，获取他的好友信息
            var contactBook = (await _context.ContactBooks.FindAsync(b => b.UserId == baseUserInfo.UserId, null, cancellationToken)).FirstOrDefault(cancellationToken);
            if (contactBook == null)
            {
                //如果用户没有通讯录，及没有好友
                return true;
            }
            //查询该用户所有的好友 Id
            var contactIds = contactBook.Contacts.Select(b => b.UserId);

            //查询组合 返回ContactBooks
            var filter = Builders<ContactBook>.Filter.And(
                    //条件过滤，查询当前用户所有好友的通讯录
                    Builders<ContactBook>.Filter.In(c => c.UserId, contactIds),
                    //在上一个条件过滤基础上,遍历结果中的 Contacts 数组，并检查联系人是否为当前修改信息的ID
                    Builders<ContactBook>.Filter.ElemMatch(b => b.Contacts, contact => contact.UserId == baseUserInfo.UserId)
                );

            //设置更新 Contacts数组下的元素的属性
            var update = Builders<ContactBook>.Update                 
                    .Set("Contacts.$.Name", baseUserInfo.Name)
                    .Set("Contacts.$.Avatar", baseUserInfo.Avatar)
                    .Set("Contacts.$.Title", baseUserInfo.Title)
                    .Set("Contacts.$.Company", baseUserInfo.Company);

            //根据过滤条件和更协条件进行批量更新
            var updateResult = await _context.ContactBooks.UpdateManyAsync(filter, update, null, cancellationToken);                           

            //如果匹配数据和更新数据一致，返回true;
            return updateResult.MatchedCount == updateResult.ModifiedCount;
        }
    }
}
