using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure;

namespace Project.API.Application.Queries
{
    public class ProjectQueries : IProjectQueries
    {

        private readonly ProjectContext _dbContext;

       public ProjectQueries(ProjectContext projectContext)
        {
            _dbContext = projectContext;
        }
        public async Task<dynamic> GetProjectDetailAsync(int projectId)
        {
            using (var conn = _dbContext.Database.GetDbConnection())
            {
                conn.Open();
                var sql = @"SELECT 
                            Projects.Company,
                            Projects.CityName,
                            Projects.ProvinceName,
                            Projects.AreaName,
                            Projects.FinStage,
                            Projects.FinMoney,
                            Projects.Valuation,
                            Projects.FinPercentage,
                            Projects.Introduction,
                            Projects.UserId,
                            Projects.Income,
                            Projects.Revenue,
                            Projects.Avatar,
                            Projects.BrokerageOptions,
                            ProjectVisibleRules.Tags,
                            ProjectVisibleRules.Visible
                            FROM  Projects INNER JOIN ProjectVisibleRules ON
                            Projects.Id  = ProjectVisibleRules.ProjectId
                            WHERE Projects.Id =@projectId ";
                var result =  await conn.QueryAsync<dynamic>(sql, new { projectId });

                return result;
            }
        }

        public async Task<dynamic> GetProjectsByUserIdAsync(int userId)
        {
            using (var conn = _dbContext.Database.GetDbConnection())
            {
                conn.Open();
                var sql = @"SELECT 
                            Projects.Id,
                            Projects.Avatar,
                            Projects.Company,
                            Projects.FinStage,
                            Projects.Introduction,
                            Projects.ShowSecurityInfo,
                            Projects.CreatedTime
                            FROM Projects WHERE Projects.UserId =@userId";

                var result = await conn.QueryAsync<dynamic>(sql, new { userId });
                return result;

            }
        }
    }
}
