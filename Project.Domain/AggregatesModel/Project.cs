using System;
using System.Collections.Generic;
using System.Text;
using Project.Domain.SeedWork;
using System.Linq;
using Project.Domain.Events;

namespace Project.Domain.AggregatesModel
{
    /// <summary>
    /// 项目实体
    /// </summary>
    public class Project : Entity, IAggregateRoot
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }


        /// <summary>
        /// 项目LOGO
        /// </summary>
        public string Avatar { get; set; }


        /// <summary>
        /// 原BP文件地址
        /// </summary>
        public string OriginBPFile { get; set; }

        /// <summary>
        /// 转换后的BP文件地址
        /// </summary>
        public string FormatBPFile { get; set; }


        /// <summary>
        /// 是否显示敏感信息
        /// </summary>
        public bool ShowSecurityInfo { get; set; }


        /// <summary>
        /// 公司所在省份编号
        /// </summary>
        public int ProvinceId { get; set; }

        public string ProvinceName { get; set; }

        public int CityId { get; set; }

        public string CityName { get; set; }

        public int  AreaId { get; set; }

        public string AreaName { get; set; }


        /// <summary>
        /// 公司成立时间
        /// </summary>
        public DateTime RegisterTime { get; set; }

        /// <summary>
        /// 公司基本信息
        /// </summary>
        public string Introduction { get; set; }




        /// <summary>
        /// 融资阶段
        /// </summary>
        public string FinStage { get; set; }



        /// <summary>
        /// 出让的比例
        /// </summary>
        public string FinPercentage { get; set; }


        /// <summary>
        /// 融资金额 单位(万)
        /// </summary>
        public string FinMoney { get; set; }

        /// <summary>
        /// 收入 (万元)
        /// </summary>
        public int Income { get; set; }


        /// <summary>
        /// 利润 (万元)
        /// </summary>
        public int Revenue { get; set; }

        /// <summary>
        /// 估值 （万元）
        /// </summary>
        public int Valuation { get; set; }

        /// <summary>
        /// 佣金分配方式
        /// </summary>
        public int BrokerageOptions { get; set; }


        /// <summary>
        /// 是否委托给平台
        /// </summary>
        public bool OnPlatform { get; set; }

        /// <summary>
        /// 可见范围
        /// </summary>
        public ProjectVisibleRule VisibleRule { get; set; }

        /// <summary>
        /// 根引用项目ID
        /// </summary>
        public int SourceId { get; set; }


        /// <summary>
        /// 上级引用项目ID
        /// </summary>
        public int ReferenceId { get; set; }

        /// <summary>
        /// 项目标签
        /// </summary>
        public string Tags { get; set; }


        /// <summary>
        /// 项目属性: 行业领域、融资币种
        /// </summary>
        public List<ProjectProperty> Properties { get; set; }


        /// <summary>
        /// 贡献者列表
        /// </summary>
        public List<ProjectContributor> Contributors { get; set; }

        /// <summary>
        /// 查看者
        /// </summary>
        public List<ProjectViewer> Viewers { get; set; }


        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedTime { get; set; }


        private Project CloneProject(Project source = null)
        {
            if (source == null)
                source = this;

            var newProject = new Project
            {
                Company = source.Company,
                BrokerageOptions = source.BrokerageOptions,
                Avatar = source.Avatar,
                CityId = source.CityId,
                CityName = source.CityName,
                ProvinceId = source.ProvinceId,
                ProvinceName = source.ProvinceName,
                AreaName = source.AreaName,
                AreaId = source.AreaId,
                FinMoney = source.FinMoney,
                FinPercentage = source.FinPercentage,
                FinStage = source.FinStage,
                OriginBPFile = source.OriginBPFile,
                FormatBPFile = source.FormatBPFile,
                Income = source.Income,
                Introduction = source.Introduction,
                OnPlatform = source.OnPlatform,
                Revenue = source.Revenue,
                RegisterTime = source.RegisterTime,
                Contributors = new List<ProjectContributor> { },
                Viewers = new List<ProjectViewer> { },
                CreatedTime = DateTime.Now,
                Valuation = source.Valuation,
                ShowSecurityInfo = source.ShowSecurityInfo,
                VisibleRule = source.VisibleRule == null ? null : new ProjectVisibleRule
                {
                    Visible = source.VisibleRule.Visible,
                    Tags = source.VisibleRule.Tags
                },
                Tags = source.Tags                

            };
            newProject.Properties = new List<ProjectProperty> { };
            foreach (var item in source.Properties)
            {
                newProject.Properties.Add(new ProjectProperty (
                  item.Key,
                  item.Value,
                  item.Text 
                ));
            }          
            return newProject;
        }


        /// <summary>
        /// 拷贝项目信息
        /// </summary>
        /// <param name="contributorId">贡献者</param>
        /// <param name="source">项目</param>
        /// <returns></returns>
        public Project ContributorFork(int contributorId,Project source =null)
        {
            if (source == null)
                source = this;
            var newproject = CloneProject(source);
            newproject.UserId = contributorId;
            newproject.SourceId = source.SourceId == 0 ? source.Id : source.SourceId;
            newproject.ReferenceId = source.ReferenceId == 0 ? source.Id : source.ReferenceId;
            newproject.UpdateTime = DateTime.Now;
            return newproject;
        }



        public Project()
        {
            Viewers = new List<ProjectViewer>();
          
            Contributors = new List<ProjectContributor>();
            CreatedTime = DateTime.Now;

            //添加项目创建事件
            AddDomainEvent(new ProjectCreatedEvent { Project = this });
        }
        /// <summary>
        /// 添加项目查看者
        /// </summary>
        public void AddViewer(int userId,string userName,string avator)
        {
            var viewer = new ProjectViewer() {
                UserId = userId,
                UserName = userName,
                Avatar = avator,
                CreatedTime = DateTime.Now
                
            };

            //如果不在查看列表中，需添加
            if(!Viewers.Any(b =>b.UserId ==userId))
            {
                Viewers.Add(viewer);

                //添加查看项目
                AddDomainEvent(new ProjectViewedEvent
                {  
                    Company =this.Company,
                    Introduction =this.Introduction,
                    ProjectViewer = viewer
                });
            }

        }

        /// <summary>
        /// 添加项目贡献者
        /// </summary>
        /// <param name="projectContributor"></param>
        public void AddContributor(ProjectContributor projectContributor)
        {
            //如果不在查看列表中，需添加
            if (!Contributors.Any(b => b.UserId == projectContributor.UserId))
            {
                Contributors.Add(projectContributor);

                //添加 参与项目事件
                AddDomainEvent(new ProjectJoinEvent {
                    Company =this.Company,
                    Introduction =this.Introduction,
                    ProjectContributor = projectContributor
                });
            }
        }
       
    }
}