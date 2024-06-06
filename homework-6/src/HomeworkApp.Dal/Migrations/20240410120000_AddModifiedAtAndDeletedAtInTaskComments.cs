using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20240410120000, TransactionBehavior.None)]
public class AddModifiedAtAndDeletedAtInTaskComments : Migration 
{

    public override void Up()
    {
        Create.Column("modified_at")
            .OnTable("task_comments")
            .AsDateTimeOffset()
            .Nullable();
        
        Create.Column("deleted_at")
            .OnTable("task_comments")
            .AsDateTimeOffset()
            .Nullable();
    }

    public override void Down()
    {
        Delete.Column("modified_at").FromTable("task_comments");
        Delete.Column("deleted_at").FromTable("task_comments");
    }
}