namespace MallAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCMS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContentManagerSystems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        FileName = c.String(),
                        Uploader = c.String(),
                        OrigName = c.String(),
                        DirectoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directories", t => t.DirectoryId, cascadeDelete: false)
                .Index(t => t.DirectoryId);
            
            CreateTable(
                "dbo.Directories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ParentId = c.Int(),
                        Owner = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directories", t => t.ParentId)
                .Index(t => t.ParentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ContentManagerSystems", "DirectoryId", "dbo.Directories");
            DropForeignKey("dbo.Directories", "ParentId", "dbo.Directories");
            DropIndex("dbo.Directories", new[] { "ParentId" });
            DropIndex("dbo.ContentManagerSystems", new[] { "DirectoryId" });
            DropTable("dbo.Directories");
            DropTable("dbo.ContentManagerSystems");
        }
    }
}
