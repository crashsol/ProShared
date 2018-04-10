using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProShare.ContactApi.Models.Dtos;
using MongoDB.Driver;
using ProShare.ContactApi.Models;
using Microsoft.Extensions.Logging;

namespace ProShare.ContactApi.Data
{
    public class MongoContactBookRepository : IContactBookRepository
    {

        private readonly ContactContext _context;

        private readonly ILogger<MongoContactBookRepository> _logger;

        public MongoContactBookRepository(ContactContext contactContext, ILogger<MongoContactBookRepository> logger)
        {
            _context = contactContext;
            _logger = logger;
        }

        public async Task<bool> AddContactAsync(int userId, BaseUserInfo baseUserInfo, CancellationToken cancellationToken)
        {
            //检查该用户是否已经创建通讯录
            if ((await _context.ContactBooks.CountAsync(b => b.UserId == userId, null, cancellationToken) == 0))
            {
                _logger.LogInformation($"为用户:{userId} 创建通讯录! ");
                //创建通讯录
                await _context.ContactBooks.InsertOneAsync(new ContactBook{ UserId = userId},null,cancellationToken);
            }
            //检索条件
            var filter = Builders<ContactBook>.Filter.Eq(b => b.UserId, userId);
            //更新设置
            var update = Builders<ContactBook>.Update.AddToSet(b => b.Contacts, new Contact
            {
                UserId = baseUserInfo.UserId,
                Name =baseUserInfo.Name,
                Title =baseUserInfo.Title,
                Company = baseUserInfo.Company,
                Avatar = baseUserInfo.Avatar
            });
            //查询并添加
            var result = await _context.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);

            return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;

        }

        public async Task<List<Contact>> GetContactsAsync(int userId, CancellationToken cancellationToken)
        {
            var contactBook =await (await _context.ContactBooks.FindAsync(b => b.UserId == userId)).FirstOrDefaultAsync();
            if(contactBook !=null)
            {
                return contactBook.Contacts;
            }
            return new List<Contact>();
        }

        public async Task<bool> TagContactAsync(int userId, int contactId, List<string> tags, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"用户{ userId} 更新自己的好友 {contactId} 标签信息 ");
            //查询userId通讯录,并匹配到 contactId好友
            var filter = Builders<ContactBook>.Filter.And(
                        Builders<ContactBook>.Filter.Eq(b =>b.UserId,userId),
                        Builders<ContactBook>.Filter.ElemMatch(c =>c.Contacts,contact =>contact.UserId == contactId)
                );

            //设置更新 contactId 的 Tags 属性
            var update = Builders<ContactBook>.Update.Set("Contacts.$.Tags", tags);

            var result = await _context.ContactBooks.UpdateOneAsync(filter, update, null, cancellationToken);

            return result.MatchedCount == result.ModifiedCount && result.ModifiedCount == 1;


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
