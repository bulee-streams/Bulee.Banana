﻿// <auto-generated />
using System;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("API.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<Guid>("EmailConfirmationToken");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<Guid>("PassworResetToken");

                    b.Property<string>("Password");

                    b.Property<byte[]>("Salt");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("User");

                    b.HasData(
                        new
                        {
                            Id = new Guid("72112158-50f3-43a6-80c3-8bc3ba33cf9d"),
                            Email = "user@email.com",
                            EmailConfirmationToken = new Guid("a6a46a35-5165-4ab5-9e19-12764cfc2144"),
                            EmailConfirmed = false,
                            PassworResetToken = new Guid("214065dd-36b2-4e5e-a67b-37aab766bafa"),
                            Password = "Kg8UhoPIigwyNIUDxbSC+nkyX+TQ34kYksFZkWuAw/4=",
                            Salt = new byte[] { 54, 54, 218, 252, 128, 49, 75, 36, 109, 26, 136, 21, 86, 217, 9, 189 },
                            Username = "user"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
