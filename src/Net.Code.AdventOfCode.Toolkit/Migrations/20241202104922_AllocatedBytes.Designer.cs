﻿// <auto-generated />
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Net.Code.AdventOfCode.Toolkit.Data;

#nullable disable

namespace Net.Code.AdventOfCode.Toolkit.Migrations
{
    [DbContext(typeof(AoCDbContext))]
    [Migration("20241202104922_AllocatedBytes")]
    partial class AllocatedBytes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1");

            modelBuilder.Entity("Net.Code.AdventOfCode.Toolkit.Core.DayResult", b =>
                {
                    b.Property<int>("Key")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Day")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("INTEGER")
                        .HasComputedColumnSql("Key%100");

                    b.Property<long>("Elapsed")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("INTEGER")
                        .HasComputedColumnSql("Part1_Elapsed + Part2_Elapsed");

                    b.Property<int>("Year")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("INTEGER")
                        .HasComputedColumnSql("Key/100");

                    b.ComplexProperty<Dictionary<string, object>>("Part1", "Net.Code.AdventOfCode.Toolkit.Core.DayResult.Part1#Result", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<long>("Elapsed")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Status")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<long>("bytes")
                                .HasColumnType("INTEGER");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Part2", "Net.Code.AdventOfCode.Toolkit.Core.DayResult.Part2#Result", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<long>("Elapsed")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Status")
                                .HasColumnType("INTEGER");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<long>("bytes")
                                .HasColumnType("INTEGER");
                        });

                    b.HasKey("Key");

                    b.HasIndex("Year", "Day");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("Net.Code.AdventOfCode.Toolkit.Core.Puzzle", b =>
                {
                    b.Property<int>("Key")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Day")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("INTEGER")
                        .HasComputedColumnSql("Key%100");

                    b.Property<string>("Input")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("INTEGER")
                        .HasComputedColumnSql("Key/100");

                    b.ComplexProperty<Dictionary<string, object>>("Answer", "Net.Code.AdventOfCode.Toolkit.Core.Puzzle.Answer#Answer", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("part1")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("part2")
                                .IsRequired()
                                .HasColumnType("TEXT");
                        });

                    b.HasKey("Key");

                    b.HasIndex("Year", "Day");

                    b.ToTable("Puzzles");
                });
#pragma warning restore 612, 618
        }
    }
}