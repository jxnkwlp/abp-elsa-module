using System.Collections.Generic;
using EcsShop.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore;

public static class ElsaModuleDbContextModelCreatingExtensions
{
    public static void ConfigureElsaModule(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        /* Configure all entities here. Example:

        builder.Entity<Question>(b =>
        {
            //Configure table & schema name
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "Questions", ElsaModuleDbProperties.DbSchema);

            b.ConfigureByConvention();

            //Properties
            b.Property(q => q.Title).IsRequired().HasMaxLength(QuestionConsts.MaxTitleLength);

            //Relations
            b.HasMany(question => question.Tags).WithOne().HasForeignKey(qt => qt.QuestionId);

            //Indexes
            b.HasIndex(q => q.CreationTime);
        });
        */


        builder.Entity<WorkflowDefinition>(b =>
        {
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "WorkflowDefinitions", ElsaModuleDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(64);
            b.Property(x => x.DisplayName).HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(256);
            b.Property(x => x.Channel).HasMaxLength(64);

            b.Property(x => x.Tag).HasMaxLength(64);
            b.Property(x => x.ContextOptions).HasConversion(new ElsaEFJsonValueConverter<Elsa.Models.WorkflowContextOptions>(), ValueComparer.CreateDefault(typeof(Elsa.Models.WorkflowContextOptions), false));
            b.Property(x => x.Variables).HasConversion(new ElsaEFJsonValueConverter<Dictionary<string, object>>(), ValueComparer.CreateDefault(typeof(Dictionary<string, object>), false));
            b.Property(x => x.CustomAttributes).HasConversion(new ElsaEFJsonValueConverter<Dictionary<string, object>>(), ValueComparer.CreateDefault(typeof(Dictionary<string, object>), false));

            // b.HasMany(x => x.Versions).WithOne(x => x.Definition).HasForeignKey(x => x.DefinitionId);
        });

        builder.Entity<WorkflowDefinitionVersion>(b =>
        {
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "WorkflowDefinitionVersions", ElsaModuleDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.HasMany(x => x.Activities);
            b.HasMany(x => x.Connections);

            b.HasIndex(x => new { x.DefinitionId, x.Version });

            b.HasOne(x => x.Definition).WithMany().HasForeignKey(x => x.DefinitionId);

            b.HasMany(x => x.Activities).WithOne().HasForeignKey(x => x.DefinitionVersionId);
            b.HasMany(x => x.Connections).WithOne().HasForeignKey(x => x.DefinitionVersionId);
        });

        builder.Entity<Activity>(b =>
        {
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "Activities", ElsaModuleDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.DefinitionVersionId, x.ActivityId });

            b.Property(x => x.Type).IsRequired().HasMaxLength(32);
            b.Property(x => x.Name).IsRequired().HasMaxLength(64);
            b.Property(x => x.DisplayName).HasMaxLength(128);
            b.Property(x => x.Description).HasMaxLength(256);

            b.Property(x => x.Properties).HasConversion(new ElsaEFJsonValueConverter<List<Elsa.Models.ActivityDefinitionProperty>>(), ValueComparer.CreateDefault(typeof(List<Elsa.Models.ActivityDefinitionProperty>), false));
            b.Property(x => x.Arrtibutes).HasConversion(new ElsaEFJsonValueConverter<Dictionary<string, object>>(), ValueComparer.CreateDefault(typeof(Dictionary<string, object>), false));
            // b.Property(x => x.PropertyStorageProviders).HasConversion(new JsonValueConverter<Dictionary<string, string>>(), ValueComparer.CreateDefault(typeof(Dictionary<string, string>), false));
        });

        builder.Entity<ActivityConnection>(b =>
        {
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "ActivityConnections", ElsaModuleDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.DefinitionVersionId, x.SourceId, x.TargetId, x.Outcome, });

            b.Property(x => x.Outcome).IsRequired().HasMaxLength(64);
            b.Property(x => x.Arrtibutes).HasConversion(new ElsaEFJsonValueConverter<Dictionary<string, object>>(), ValueComparer.CreateDefault(typeof(Dictionary<string, object>), false));
        });

        builder.Entity<Bookmark>(b =>
        {
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "Bookmarks", ElsaModuleDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Hash).HasMaxLength(128);
            // b.Property(x => x.Model).HasMaxLength(128);
            b.Property(x => x.ModelType).HasMaxLength(256);
            b.Property(x => x.ActivityType).HasMaxLength(256);
            b.Property(x => x.CorrelationId).HasMaxLength(128);

        });

        builder.Entity<Trigger>(b =>
        {
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "Triggers", ElsaModuleDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Hash).HasMaxLength(128);
            // b.Property(x => x.Model).HasMaxLength(128);
            b.Property(x => x.ModelType).HasMaxLength(256);
            b.Property(x => x.ActivityType).HasMaxLength(256);

        });

        builder.Entity<WorkflowInstance>(b =>
        {
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "WorkflowInstances", ElsaModuleDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.CorrelationId).HasMaxLength(128);
            b.Property(x => x.ContextType).HasMaxLength(128);
            b.Property(x => x.ContextId).HasMaxLength(128);

            b.Property(x => x.Variables).HasConversion(new ElsaEFJsonValueConverter<Dictionary<string, object>>(), ValueComparer.CreateDefault(typeof(Dictionary<string, object>), false));
            b.Property(x => x.Input).HasConversion(new ElsaEFJsonValueConverter<Elsa.Services.Models.WorkflowInputReference>(), ValueComparer.CreateDefault(typeof(Elsa.Services.Models.WorkflowInputReference), false));
            b.Property(x => x.Output).HasConversion(new ElsaEFJsonValueConverter<Elsa.Services.Models.WorkflowOutputReference>(), ValueComparer.CreateDefault(typeof(Elsa.Services.Models.WorkflowOutputReference), false));
            b.Property(x => x.Fault).HasConversion(new ElsaEFJsonValueConverter<Elsa.Models.WorkflowFault>(), ValueComparer.CreateDefault(typeof(Elsa.Models.WorkflowFault), false));
            b.Property(x => x.ScheduledActivities).HasConversion(new ElsaEFJsonValueConverter<List<Elsa.Models.ScheduledActivity>>(), ValueComparer.CreateDefault(typeof(List<Elsa.Models.ScheduledActivity>), false));
            b.Property(x => x.BlockingActivities).HasConversion(new ElsaEFJsonValueConverter<List<Elsa.Models.BlockingActivity>>(), ValueComparer.CreateDefault(typeof(List<Elsa.Models.BlockingActivity>), false));
            b.Property(x => x.Scopes).HasConversion(new ElsaEFJsonValueConverter<List<Elsa.Models.ActivityScope>>(), ValueComparer.CreateDefault(typeof(List<Elsa.Models.ActivityScope>), false));
            b.Property(x => x.CurrentActivity).HasConversion(new ElsaEFJsonValueConverter<Elsa.Models.ScheduledActivity>(), ValueComparer.CreateDefault(typeof(Elsa.Models.ScheduledActivity), false));
            b.Property(x => x.ActivityData).HasConversion(new ElsaEFJsonValueConverter<Dictionary<string, IDictionary<string, object>>>(), ValueComparer.CreateDefault(typeof(Dictionary<string, IDictionary<string, object>>), false));
            b.Property(x => x.Metadata).HasConversion(new ElsaEFJsonValueConverter<Dictionary<string, object>>(), ValueComparer.CreateDefault(typeof(Dictionary<string, object>), false));

            // b.HasOne(x => x.Definition).WithOne().OnDelete(DeleteBehavior.SetNull);
            // b.HasOne(x => x.DefinitionVersion).WithOne();
        });

        builder.Entity<WorkflowExecutionLog>(b =>
        {
            b.ToTable(ElsaModuleDbProperties.DbTablePrefix + "WorkflowExecutionLogs", ElsaModuleDbProperties.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.ActivityType).HasMaxLength(128);
            b.Property(x => x.EventName).HasMaxLength(128);
            b.Property(x => x.Source).HasMaxLength(128);
            b.Property(x => x.Data).HasConversion(new ElsaEFJsonValueConverter<JObject>(), ValueComparer.CreateDefault(typeof(JObject), false));

        });



    }

}
