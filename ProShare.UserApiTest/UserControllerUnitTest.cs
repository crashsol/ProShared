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
        /// 内存模拟数据
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
        /// 返回元祖
        /// </summary>
        /// <returns></returns>
        private (UserController contoller,UserContext userContext) GetController()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<UserController>>();
            var logger = loggerMoq.Object; //获得模拟的logger

            //模拟创建Controller
             return (new UserController(logger, context),context);

        }
        [Fact]
        public async Task 验证_数据返回_类型_值()
        {

            var data = GetController();
            var response = await data.contoller.Get();
            //检查返回类型是否正确
            Assert.IsType<JsonResult>(response);
            var result = response.Should().BeOfType<JsonResult>().Subject;
            var appuser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appuser.Id.Should().Be(1);
            appuser.Name.Should().Be("Crashsol");

        }


        


       [Fact]
        public async Task 验证_修改_用户属性()
        {
            var data = GetController();
            var document = new JsonPatchDocument<AppUser>();
            //document.Add(b => b.Name, "Ceshi"); //添加操作
            document.Replace(b => b.Name, "Ceshi"); //修改操作
            var response =  await data.contoller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            //验证结果
            var appuser =  result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appuser.Name.Should().Be("Ceshi");
            //验证数据库中
            var entity = await data.userContext.Users.SingleOrDefaultAsync(b => b.Id == appuser.Id);
            entity.Should().NotBeNull();
            entity.Name.Should().Be("Ceshi");

        }

        [Fact]
        public async Task 验证_添加_用户属性()
        {
            var data = GetController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(b => b.Properties, new List<UserProperty> {
                new UserProperty{ Key ="industry" ,Value="互联网", Text="互联网" }

            }); //添加操作
         
            var response = await data.contoller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            //验证结果
            var appuser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appuser.Properties.Count.Should().Be(1);
            appuser.Properties.First().Value.Should().Be("互联网");
            appuser.Properties.First().Text.Should().Be("互联网");

            //验证数据库中
            var entity = await data.userContext.Users.SingleOrDefaultAsync(b => b.Id == appuser.Id);
            entity.Properties.Count.Should().Be(1);
            entity.Properties.First().Value.Should().Be("互联网");
            entity.Properties.First().Text.Should().Be("互联网");
        }


        [Fact]
        public async Task 验证_删除_用户属性()
        {
            var data = GetController();
            var document = new JsonPatchDocument<AppUser>();
            document.Replace(b => b.Properties, new List<UserProperty> {             

            }); //添加操作
            var response = await data.contoller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            //验证结果
            var appuser = result.Value.Should().BeAssignableTo<AppUser>().Subject;
            appuser.Properties.Should().BeEmpty();         

            //验证数据库中
            var entity = await data.userContext.Users.SingleOrDefaultAsync(b => b.Id == appuser.Id);
            appuser.Properties.Should().BeEmpty();
        }


    }
}
