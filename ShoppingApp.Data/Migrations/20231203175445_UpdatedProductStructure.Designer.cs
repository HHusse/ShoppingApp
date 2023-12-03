﻿// <auto-generated />
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ShoppingApp.Data.Migrations
{
    [DbContext(typeof(ShoppingAppDbContext))]
    [Migration("20231203175445_UpdatedProductStructure")]
    partial class UpdatedProductStructure
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ShoppingApp.Data.Models.AccountDTO", b =>
                {
                    b.Property<string>("Uid")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Uid");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Accounts", (string)null);
                });

            modelBuilder.Entity("ShoppingApp.Data.Models.OrderHeaderDTO", b =>
                {
                    b.Property<string>("Uid")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Total")
                        .HasColumnType("float");

                    b.HasKey("Uid");

                    b.HasIndex("AccountId");

                    b.ToTable("OrderHeaders", (string)null);
                });

            modelBuilder.Entity("ShoppingApp.Data.Models.OrderLineDTO", b =>
                {
                    b.Property<string>("Uid")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OrderHeaderUid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("ProductUid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Uid");

                    b.ToTable("OrderLines", (string)null);
                });

            modelBuilder.Entity("ShoppingApp.Data.Models.ProductDTO", b =>
                {
                    b.Property<string>("Uid")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Uid");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("ShoppingApp.Data.Models.OrderHeaderDTO", b =>
                {
                    b.HasOne("ShoppingApp.Data.Models.AccountDTO", "Account")
                        .WithMany("OrderHeaders")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("ShoppingApp.Data.Models.OrderLineDTO", b =>
                {
                    b.HasOne("ShoppingApp.Data.Models.OrderHeaderDTO", "OrderHeader")
                        .WithMany("OrderLines")
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ShoppingApp.Data.Models.ProductDTO", "Product")
                        .WithMany("OrderLines")
                        .HasForeignKey("Uid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OrderHeader");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShoppingApp.Data.Models.AccountDTO", b =>
                {
                    b.Navigation("OrderHeaders");
                });

            modelBuilder.Entity("ShoppingApp.Data.Models.OrderHeaderDTO", b =>
                {
                    b.Navigation("OrderLines");
                });

            modelBuilder.Entity("ShoppingApp.Data.Models.ProductDTO", b =>
                {
                    b.Navigation("OrderLines");
                });
#pragma warning restore 612, 618
        }
    }
}
