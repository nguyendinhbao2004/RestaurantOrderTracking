using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RestaurantOrderTracking.Infrastructure.Data;

#nullable disable

namespace RestaurantOrderTracking.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260326091500_FixChefSpecialtyTextToInt")]
    public partial class FixChefSpecialtyTextToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Chefs""
                ALTER COLUMN ""Specialty"" TYPE integer
                USING (
                    CASE
                        WHEN trim(""Specialty"") ~ '^[0-9]+$' THEN trim(""Specialty"")::integer
                        WHEN trim(""Specialty"") = 'HeadChef' THEN 1
                        WHEN trim(""Specialty"") = 'AsiaChef' THEN 2
                        WHEN trim(""Specialty"") = 'WesternChef' THEN 3
                        WHEN trim(""Specialty"") = 'ChefAssistant' THEN 4
                        ELSE 4
                    END
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Chefs""
                ALTER COLUMN ""Specialty"" TYPE text
                USING ""Specialty""::text;
            ");
        }
    }
}
