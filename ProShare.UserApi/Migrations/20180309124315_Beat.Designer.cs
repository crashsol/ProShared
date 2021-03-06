﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Storage.Internal;
using ProShare.UserApi.Data;
using System;

namespace ProShare.UserApi.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20180309124315_Beat")]
    partial class Beat
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("ProShare.UserApi.Models.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Avatar");

                    b.Property<string>("City");

                    b.Property<string>("CityId");

                    b.Property<string>("Company");

                    b.Property<string>("Email");

                    b.Property<byte>("Gender");

                    b.Property<string>("Name");

                    b.Property<string>("NameCard");

                    b.Property<string>("Phone");

                    b.Property<string>("Province");

                    b.Property<string>("ProvinceId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProShare.UserApi.Models.BPFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AppUserId");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<string>("FileName");

                    b.Property<string>("FromatFilePash");

                    b.Property<string>("OriginFilePash");

                    b.HasKey("Id");

                    b.ToTable("BPFiles");
                });

            modelBuilder.Entity("ProShare.UserApi.Models.UserProperty", b =>
                {
                    b.Property<int>("AppUserId");

                    b.Property<string>("Key")
                        .HasMaxLength(100);

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.Property<string>("Text");

                    b.HasKey("AppUserId", "Key", "Value");

                    b.ToTable("UserProperties");
                });

            modelBuilder.Entity("ProShare.UserApi.Models.UserTag", b =>
                {
                    b.Property<int>("AppUserId");

                    b.Property<string>("Tag")
                        .HasMaxLength(100);

                    b.HasKey("AppUserId", "Tag");

                    b.ToTable("UserTags");
                });

            modelBuilder.Entity("ProShare.UserApi.Models.UserProperty", b =>
                {
                    b.HasOne("ProShare.UserApi.Models.AppUser")
                        .WithMany("Properties")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
