﻿// <auto-generated />
using System;
using Cryptic.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cryptic.Shared.Persistence.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240818194635_InitialDbCreation")]
    partial class InitialDbCreation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Cryptic.Shared.Features.Notes.DataModels+Note", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("content");

                    b.Property<string>("ControlTokenHash")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("control_token_hash");

                    b.Property<DateTimeOffset>("DeleteAfterTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("delete_after_time");

                    b.Property<bool>("DeleteOnReceipt")
                        .HasColumnType("boolean")
                        .HasColumnName("delete_on_receipt");

                    b.Property<string>("EncryptionKeyOptions")
                        .HasColumnType("text")
                        .HasColumnName("encryption_key_options");

                    b.Property<string>("Signature")
                        .HasColumnType("text")
                        .HasColumnName("signature");

                    b.Property<string>("SigningKeyOptions")
                        .HasColumnType("text")
                        .HasColumnName("signing_key_options");

                    b.HasKey("Id")
                        .HasName("pk_notes");

                    b.ToTable("notes", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}