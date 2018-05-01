using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Proshare.RecommendApi.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectRecommend",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Company = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    FinStage = table.Column<string>(nullable: true),
                    FromUserAvatar = table.Column<string>(nullable: true),
                    FromUserId = table.Column<int>(nullable: false),
                    FromUserName = table.Column<string>(nullable: true),
                    Introducation = table.Column<string>(nullable: true),
                    ProjectAvatar = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    RecommendTime = table.Column<DateTime>(nullable: false),
                    RecommendType = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRecommend", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectRecommend");
        }
    }
}
