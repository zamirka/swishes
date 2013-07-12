namespace swishes.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWishes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Wishes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 250),
                        Description = c.String(),
                        Link = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        UserProfile_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfile", t => t.UserProfile_UserId)
                .Index(t => t.UserProfile_UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Wishes", new[] { "UserProfile_UserId" });
            DropForeignKey("dbo.Wishes", "UserProfile_UserId", "dbo.UserProfile");
            DropTable("dbo.Wishes");
        }
    }
}
