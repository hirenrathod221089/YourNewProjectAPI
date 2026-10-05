using FluentMigrator;

namespace YourNewProjectAPI.Infrastructure.Migrations;

[Migration(2026100502)] // Unique chronological version stamp for our second layout segment
public class CreateIdentityUserTables : Migration
{
    public override void Up()
    {
        // 1. CREATE THE SYSTEM ROLES DEFINITIONS LOOKUP TABLE
        if (!Schema.Table("AppRoles").Exists())
        {
            Create.Table("AppRoles")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("RoleName").AsString(50).NotNullable().Unique()
                .WithColumn("Description").AsString(250).Nullable();

            // Seed your core structural governance boundaries immediately!
            Insert.IntoTable("AppRoles").Row(new { RoleName = "Chitnish", Description = "Revenue Approving Authority" });
            Insert.IntoTable("AppRoles").Row(new { RoleName = "Officer", Description = "Standard Reporting Personnel" });
        }

        // 2. CREATE THE MASTER USER REGISTRATION PROFILES TABLE
        if (!Schema.Table("UserProfiles").Exists())
        {
            Create.Table("UserProfiles")
                .WithColumn("Id").AsInt32().PrimaryKey().Identity()
                .WithColumn("LoginId").AsString(50).NotNullable().Unique()
                .WithColumn("UserName").AsString(150).NotNullable()
                .WithColumn("PasswordHash").AsString(500).NotNullable()
                .WithColumn("RoleId").AsInt32().NotNullable().ForeignKey("FK_UserProfiles_AppRoles", "AppRoles", "Id")
                .WithColumn("DCode").AsString(10).NotNullable()
                .WithColumn("TCode").AsString(10).NotNullable()
                .WithColumn("OfficeCode").AsString(50).NotNullable()
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true)

                // Add the standard enterprise auditing columns we established!
                .WithColumn("CreatedBy").AsString(100).NotNullable().WithDefaultValue("System")
                .WithColumn("CreatedDate").AsDateTime().NotNullable().WithDefaultValue(SystemMethods.CurrentDateTime);
        }
    }

    public override void Down()
    {
        // Tear down constraints safely in reverse hierarchy sequence if rolled back
        if (Schema.Table("UserProfiles").Exists())
        {
            Delete.Table("UserProfiles");
        }

        if (Schema.Table("AppRoles").Exists())
        {
            Delete.Table("AppRoles");
        }
    }
}
