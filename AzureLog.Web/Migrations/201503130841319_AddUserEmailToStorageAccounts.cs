namespace AzureLog.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserEmailToStorageAccounts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StorageAccounts", "UserEmail", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StorageAccounts", "UserEmail");
        }
    }
}
