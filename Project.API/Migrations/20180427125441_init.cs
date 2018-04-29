﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Project.API.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    AreaId = table.Column<int>(nullable: false),
                    AreaName = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    BrokerageOptions = table.Column<int>(nullable: false),
                    CityId = table.Column<int>(nullable: false),
                    CityName = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    FinMoney = table.Column<string>(nullable: true),
                    FinPercentage = table.Column<string>(nullable: true),
                    FinStage = table.Column<string>(nullable: true),
                    FormatBPFile = table.Column<string>(nullable: true),
                    Income = table.Column<int>(nullable: false),
                    Introduction = table.Column<string>(nullable: true),
                    OnPlatform = table.Column<bool>(nullable: false),
                    OriginBPFile = table.Column<string>(nullable: true),
                    ProvinceId = table.Column<int>(nullable: false),
                    ProvinceName = table.Column<string>(nullable: true),
                    ReferenceId = table.Column<int>(nullable: false),
                    RegisterTime = table.Column<DateTime>(nullable: false),
                    Revenue = table.Column<int>(nullable: false),
                    ShowSecurityInfo = table.Column<bool>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Valuation = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectContributors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Avatar = table.Column<string>(nullable: true),
                    ContributorType = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    IsCloser = table.Column<bool>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectContributors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectContributors_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProperties",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProperties", x => new { x.ProjectId, x.Key, x.Value });
                    table.ForeignKey(
                        name: "FK_ProjectProperties_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectViewers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    Avatar = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectViewers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectViewers_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectVisibleRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Visible = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVisibleRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectVisibleRules_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributors_ProjectId",
                table: "ProjectContributors",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViewers_ProjectId",
                table: "ProjectViewers",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVisibleRules_ProjectId",
                table: "ProjectVisibleRules",
                column: "ProjectId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectContributors");

            migrationBuilder.DropTable(
                name: "ProjectProperties");

            migrationBuilder.DropTable(
                name: "ProjectViewers");

            migrationBuilder.DropTable(
                name: "ProjectVisibleRules");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
