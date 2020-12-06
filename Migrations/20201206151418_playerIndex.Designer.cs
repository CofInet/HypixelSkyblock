﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using hypixel;

namespace hypixel.Migrations
{
    [DbContext(typeof(HypixelContext))]
    [Migration("20201206151418_playerIndex")]
    partial class playerIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("dev.BazaarPull", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp");

                    b.ToTable("BazaarPull");
                });

            modelBuilder.Entity("dev.BuyOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<short>("Orders")
                        .HasColumnType("smallint");

                    b.Property<double>("PricePerUnit")
                        .HasColumnType("double");

                    b.Property<int?>("ProductInfoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductInfoId");

                    b.ToTable("BuyOrder");
                });

            modelBuilder.Entity("dev.ProductInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ProductId")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<int?>("PullInstanceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("PullInstanceId");

                    b.ToTable("BazaarPrices");
                });

            modelBuilder.Entity("dev.QuickStatus", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("BuyMovingWeek")
                        .HasColumnType("bigint");

                    b.Property<int>("BuyOrders")
                        .HasColumnType("int");

                    b.Property<double>("BuyPrice")
                        .HasColumnType("double");

                    b.Property<long>("BuyVolume")
                        .HasColumnType("bigint");

                    b.Property<int?>("QuickStatusID")
                        .HasColumnType("int");

                    b.Property<long>("SellMovingWeek")
                        .HasColumnType("bigint");

                    b.Property<int>("SellOrders")
                        .HasColumnType("int");

                    b.Property<double>("SellPrice")
                        .HasColumnType("double");

                    b.Property<long>("SellVolume")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("QuickStatusID")
                        .IsUnique();

                    b.ToTable("QuickStatus");
                });

            modelBuilder.Entity("dev.SellOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<short>("Orders")
                        .HasColumnType("smallint");

                    b.Property<double>("PricePerUnit")
                        .HasColumnType("double");

                    b.Property<int?>("ProductInfoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductInfoId");

                    b.ToTable("SellOrder");
                });

            modelBuilder.Entity("hypixel.Enchantment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<short>("Level")
                        .HasColumnType("smallint");

                    b.Property<int?>("SaveAuctionId")
                        .HasColumnType("int");

                    b.Property<byte>("Type")
                        .HasColumnType("TINYINT(2)");

                    b.HasKey("Id");

                    b.HasIndex("SaveAuctionId");

                    b.ToTable("Enchantment");
                });

            modelBuilder.Entity("hypixel.NbtData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte[]>("data")
                        .HasColumnType("varbinary(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.ToTable("NbtData");
                });

            modelBuilder.Entity("hypixel.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("ChangedFlag")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(16)")
                        .HasMaxLength(16);

                    b.Property<DateTime>("UpdatedAt")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime");

                    b.Property<string>("UuId")
                        .HasColumnType("char(32)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.HasIndex("UuId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("hypixel.SaveAuction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<short>("AnvilUses")
                        .HasColumnType("smallint");

                    b.Property<string>("AuctioneerId")
                        .HasColumnType("char(32)");

                    b.Property<int>("AuctioneerIntId")
                        .HasColumnType("int");

                    b.Property<byte>("Category")
                        .HasColumnType("TINYINT(2)");

                    b.Property<bool>("Claimed")
                        .HasColumnType("bit");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime");

                    b.Property<long>("HighestBidAmount")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ItemCreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("ItemName")
                        .HasColumnType("varchar(45)")
                        .HasMaxLength(45)
                        .HasAnnotation("MySQL:Charset", "utf8");

                    b.Property<int?>("NbtDataId")
                        .HasColumnType("int");

                    b.Property<string>("ProfileId")
                        .HasColumnType("char(32)");

                    b.Property<byte>("Reforge")
                        .HasColumnType("TINYINT(2)");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime");

                    b.Property<long>("StartingBid")
                        .HasColumnType("bigint");

                    b.Property<string>("Tag")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<byte>("Tier")
                        .HasColumnType("TINYINT(2)");

                    b.Property<string>("Uuid")
                        .HasColumnType("char(32)");

                    b.HasKey("Id");

                    b.HasIndex("AuctioneerIntId");

                    b.HasIndex("End");

                    b.HasIndex("ItemName");

                    b.HasIndex("NbtDataId");

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.ToTable("Auctions");
                });

            modelBuilder.Entity("hypixel.SaveBids", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<string>("Bidder")
                        .HasColumnType("char(32)");

                    b.Property<int>("BidderId")
                        .HasColumnType("int");

                    b.Property<string>("ProfileId")
                        .HasColumnType("char(32)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.Property<int?>("Uuid")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Bidder");

                    b.HasIndex("BidderId");

                    b.HasIndex("Uuid");

                    b.ToTable("Bids");
                });

            modelBuilder.Entity("hypixel.SubscribeItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("GeneratedAt")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime");

                    b.Property<string>("Initiator")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.Property<string>("ItemTag")
                        .HasColumnType("varchar(45)")
                        .HasMaxLength(45);

                    b.Property<string>("PlayerUuid")
                        .HasColumnType("char(32)");

                    b.Property<long>("Price")
                        .HasColumnType("bigint");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SubscribeItem");
                });

            modelBuilder.Entity("hypixel.UuId", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("SaveAuctionId")
                        .HasColumnType("int");

                    b.Property<int?>("SaveAuctionId1")
                        .HasColumnType("int");

                    b.Property<string>("value")
                        .HasColumnType("char(32)");

                    b.HasKey("Id");

                    b.HasIndex("SaveAuctionId");

                    b.HasIndex("SaveAuctionId1");

                    b.ToTable("UuId");
                });

            modelBuilder.Entity("dev.BuyOrder", b =>
                {
                    b.HasOne("dev.ProductInfo", null)
                        .WithMany("BuySummery")
                        .HasForeignKey("ProductInfoId");
                });

            modelBuilder.Entity("dev.ProductInfo", b =>
                {
                    b.HasOne("dev.BazaarPull", "PullInstance")
                        .WithMany("Products")
                        .HasForeignKey("PullInstanceId");
                });

            modelBuilder.Entity("dev.QuickStatus", b =>
                {
                    b.HasOne("dev.ProductInfo", "Info")
                        .WithOne("QuickStatus")
                        .HasForeignKey("dev.QuickStatus", "QuickStatusID");
                });

            modelBuilder.Entity("dev.SellOrder", b =>
                {
                    b.HasOne("dev.ProductInfo", null)
                        .WithMany("SellSummary")
                        .HasForeignKey("ProductInfoId");
                });

            modelBuilder.Entity("hypixel.Enchantment", b =>
                {
                    b.HasOne("hypixel.SaveAuction", null)
                        .WithMany("Enchantments")
                        .HasForeignKey("SaveAuctionId");
                });

            modelBuilder.Entity("hypixel.SaveAuction", b =>
                {
                    b.HasOne("hypixel.Player", null)
                        .WithMany("Auctions")
                        .HasForeignKey("AuctioneerIntId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hypixel.NbtData", "NbtData")
                        .WithMany()
                        .HasForeignKey("NbtDataId");
                });

            modelBuilder.Entity("hypixel.SaveBids", b =>
                {
                    b.HasOne("hypixel.Player", null)
                        .WithMany("Bids")
                        .HasForeignKey("BidderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("hypixel.SaveAuction", "Auction")
                        .WithMany("Bids")
                        .HasForeignKey("Uuid");
                });

            modelBuilder.Entity("hypixel.UuId", b =>
                {
                    b.HasOne("hypixel.SaveAuction", null)
                        .WithMany("ClaimedBids")
                        .HasForeignKey("SaveAuctionId");

                    b.HasOne("hypixel.SaveAuction", null)
                        .WithMany("CoopMembers")
                        .HasForeignKey("SaveAuctionId1");
                });
#pragma warning restore 612, 618
        }
    }
}
