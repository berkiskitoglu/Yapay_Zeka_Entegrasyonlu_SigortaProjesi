using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsureYouAI.Migrations
{
    /// <inheritdoc />
    public partial class mig_add_class_revenue_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Revenues",
                newName: "Detail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Detail",
                table: "Revenues",
                newName: "Description");
        }
    }
}
