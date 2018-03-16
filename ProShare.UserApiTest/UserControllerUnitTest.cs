using ProShare.UserApi.Data;
using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ProShare.UserApi.Models;
using Microsoft.Extensions.Logging;
using Moq;
using ProShare.UserApi.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Linq;

namespace ProShare.UserApiTest
{
    public class UserControllerUnitTest
    {
        /// <summary>
        /// �ڴ�ģ������
        /// </summary>
        /// <returns></returns>
        private UserContext GetUserContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                 .UseInMemoryDatabase(Guid.NewGuid().ToString())
                 .Options;

            var userContext = new UserContext(options);
            userContext.Users.Add(new AppUser
            {
                Id = 1,
                Name = "Crashsol"
            });
            userContext.SaveChanges();
            return userContext;

        }


        /// <summary>
        /// ����Ԫ��
        /// </summary>
        /// <returns></returns>
        private (UserController contoller,UserContext userContext) GetController()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<UserController>>();
            var logger = loggerMoq.Object; //���ģ���logger

            //ģ�ⴴ��Controller
             return (new UserController(logger, context),context);

        }
        [Fact]
        public async Task ��֤_���ݷ���_����_ֵ()
        {

            var data = GetController();
            var response = await data.contoller.Get();
            //��鷵�������Ƿ���ȷ
            Assert.IsType<JsonResult>(response);
            var result = response.Should().BeOfType<JsonResult>().Subject;
            var appuser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appuser.Id.Should().Be(1);
            appuser.Name.Should().Be("Crashsol");

        }


        


       [Fact]
        public async Task ��֤_�޸�_�û�����()
        {
            var data = GetController();
            var document = new JsonPatchDocument<AppUser>();
            //document.Add(b => b.Name, "Ceshi"); //��Ӳ���
            document.Replace(b => b.Name, "Ceshi"); //�޸Ĳ���
            var response =  await data.contoller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            //��֤���
            var appuser =  result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appuser.Name.Should().Be("Ceshi");
            //��֤���ݿ���
            var entity = await data.userContext.Users.SingleOrDefaultAsync(b => b.Id == appuser.Id);
            entity.Should().NotBeNull();
            entity.Name.Should().Be("Ceshi");

        }

        [Fact]
        public async Task ��֤_���_�û�����()
        {
            var data = GetController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(b => b.Properties, new List<UserProperty> {
                new UserProperty{ Key ="industry" ,Value="������", Text="������" }

            }); //��Ӳ���
         
            var response = await data.contoller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            //��֤���
            var appuser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appuser.Properties.Count.Should().Be(1);
            appuser.Properties.First().Value.Should().Be("������");
            appuser.Properties.First().Text.Should().Be("������");

            //��֤���ݿ���
            var entity = await data.userContext.Users.SingleOrDefaultAsync(b => b.Id == appuser.Id);
            entity.Properties.Count.Should().Be(1);
            entity.Properties.First().Value.Should().Be("������");
            entity.Properties.First().Text.Should().Be("������");
        }


        [Fact]
        public async Task ��֤_ɾ��_�û�����()
        {
            var data = GetController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(b => b.Properties, new List<UserProperty> {             

            }); //��Ӳ���
            var response = await data.contoller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            //��֤���
            var appuser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appuser.Properties.Should().BeEmpty();         

            //��֤���ݿ���
            var entity = await data.userContext.Users.SingleOrDefaultAsync(b => b.Id == appuser.Id);
            appuser.Properties.Should().BeEmpty();
        }


    }
}
