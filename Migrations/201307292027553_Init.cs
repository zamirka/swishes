namespace swishes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Wishes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Link = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        UserId = c.String(nullable: false),
                        Profile_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.Profile_UserId, cascadeDelete: true)
                .Index(t => t.Profile_UserId);
            
            CreateTable(
                "dbo.webpages_OAuthMembership",
                c => new
                    {
                        Provider = c.String(nullable: false, maxLength: 30),
                        ProviderUserId = c.String(nullable: false, maxLength: 100),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Provider, t.ProviderUserId })
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.webpages_Membership",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        CreateDate = c.DateTime(),
                        ConfirmationToken = c.String(maxLength: 128),
                        IsConfirmed = c.Boolean(nullable: false),
                        LastPasswordFailureDate = c.DateTime(),
                        PasswordFailuresSinceLastSuccess = c.Int(nullable: false),
                        Password = c.String(nullable: false, maxLength: 128),
                        PasswordChangedDate = c.DateTime(),
                        PasswordSalt = c.String(nullable: false, maxLength: 128),
                        PasswordVerificationToken = c.String(maxLength: 128),
                        PasswordVerificationTokenExpirationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.webpages_Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false, maxLength: 256),
                        UserInRole_UserId = c.Int(),
                        UserInRole_RoleId = c.Int(),
                    })
                .PrimaryKey(t => t.RoleId)
                .ForeignKey("dbo.webpages_UsersInRoles", t => new { t.UserInRole_UserId, t.UserInRole_RoleId })
                .Index(t => new { t.UserInRole_UserId, t.UserInRole_RoleId });
            
            CreateTable(
                "dbo.webpages_UsersInRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId });
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.webpages_Roles", new[] { "UserInRole_UserId", "UserInRole_RoleId" });
            DropIndex("dbo.webpages_OAuthMembership", new[] { "UserId" });
            DropIndex("dbo.Wishes", new[] { "Profile_UserId" });
            DropForeignKey("dbo.webpages_Roles", new[] { "UserInRole_UserId", "UserInRole_RoleId" }, "dbo.webpages_UsersInRoles");
            DropForeignKey("dbo.webpages_OAuthMembership", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.Wishes", "Profile_UserId", "dbo.UserProfile");
            DropTable("dbo.webpages_UsersInRoles");
            DropTable("dbo.webpages_Roles");
            DropTable("dbo.webpages_Membership");
            DropTable("dbo.webpages_OAuthMembership");
            DropTable("dbo.Wishes");
            DropTable("dbo.UserProfile");
        }
    }
}
