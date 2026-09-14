using FluentMigrator;

namespace YourNewProjectAPI.Infrastructure.Migrations;

// Stamping the migration with a unique chronological version code (YYYYMMDDNN)
[Migration(2026091401)]
public class M20260914_CreatePatrakTable : Migration
{
    public override void Up()
    {
        // 1. Create the PatrakRegisters table automatically if it isn't there
        if (!Schema.Table("PatrakRegisters").Exists())
        {
            Create.Table("PatrakRegisters")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("PatrakName").AsString(100).NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

            // 2. Insert seed data so your Dapper repository instantly finds records to pull!
            Insert.IntoTable("PatrakRegisters").Row(new { PatrakName = "Monthly" });
            Insert.IntoTable("PatrakRegisters").Row(new { PatrakName = "Quarterly" });
            Insert.IntoTable("PatrakRegisters").Row(new { PatrakName = "Six Monthly" });
        }
    }

    public override void Down()
    {
        // Rollback strategy: allows you to drop the table if you ever revert changes
        Delete.Table("PatrakRegisters");
    }
}
