using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB;
using MongoDB.Driver;
using ProShare.ContactApi.Models;
using ProShare.ContactApi.Models.Dtos;
using Microsoft.Extensions.Options;

namespace ProShare.ContactApi.Data
{
    public class ContactContext
    {
        private IMongoDatabase _database;
        private AppSetting _appSetting;

        public ContactContext(IOptionsSnapshot<AppSetting> setting)
        {
            _appSetting = setting.Value;
            var client = new MongoClient(_appSetting.MongoConnectionString);
            if (client != null)
            {
                _database = client.GetDatabase(_appSetting.ContactDataBaseName);
            }

        }

        /// <summary>
        /// 确保表空间已经创建
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        private  void CheckAndCreateCollection(string collectionName)
        {
            var collectionList = _database.ListCollections().ToList();
            var collectionNames = new List<string>();
            ///获取所有表名称
            collectionList.ForEach(b => collectionNames.Add(b["name"].AsString));
            //判断是否已经创建表
            if(!collectionNames.Contains(collectionName))
            {
                //如果没有创建，进行表空间的创建
                _database.CreateCollection(collectionName);
            }
            
        }

        /// <summary>
        /// 用户通讯录集合
        /// </summary>
        public IMongoCollection<ContactBook> ContactBooks
        {
            get
            {
                CheckAndCreateCollection("ContactBook");
                return _database.GetCollection<ContactBook>("ContactBook");
            }
        }


        /// <summary>
        /// 好友申请请求集合
        /// </summary>
        public IMongoCollection<ContactApplyRequest> ContactApplyRequests
        {
            get
            {
                CheckAndCreateCollection("ContactApplyRequest");
                return _database.GetCollection<ContactApplyRequest>("ContactApplyRequest");
            }
        }
    }
}
