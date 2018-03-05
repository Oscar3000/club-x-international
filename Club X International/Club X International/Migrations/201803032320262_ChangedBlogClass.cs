namespace Club_X_International.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedBlogClass : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Blogs", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Blogs", "Name", c => c.String(nullable: false));
        }
    }
}
