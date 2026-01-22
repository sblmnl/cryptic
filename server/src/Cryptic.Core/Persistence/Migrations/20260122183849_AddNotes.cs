using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptic.Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "character varying(16384)", maxLength: 16384, nullable: false),
                    delete_after = table.Column<int>(type: "integer", nullable: false),
                    delete_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    control_token_hash = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    client_metadata = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notes", x => x.id);
                });
        }
    }
}
