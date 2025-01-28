namespace BITCollege_FT.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedSetNextMethods : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Courses", "CourseNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Courses", "CourseNumber", c => c.String(nullable: false));
        }
    }
}
