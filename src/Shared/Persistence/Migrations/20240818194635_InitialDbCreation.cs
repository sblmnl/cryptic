using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cryptic.Shared.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDbCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    delete_on_receipt = table.Column<bool>(type: "boolean", nullable: false),
                    delete_after_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    control_token_hash = table.Column<string>(type: "text", nullable: false),
                    signature = table.Column<string>(type: "text", nullable: true),
                    encryption_key_options = table.Column<string>(type: "text", nullable: true),
                    signing_key_options = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notes", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notes");
        }
    }
}
