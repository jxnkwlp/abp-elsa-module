using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NodaTime;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Guids;
using ActivityDefinition = Elsa.Models.ActivityDefinition;
using BlockingActivity = Elsa.Models.BlockingActivity;
using BookmarkModel = Elsa.Models.Bookmark;
using ConnectionDefinitionModel = Elsa.Models.ConnectionDefinition;
using TriggerModel = Elsa.Models.Trigger;
using WorkflowDefinitionModel = Elsa.Models.WorkflowDefinition;
using WorkflowExecutionLogRecordModel = Elsa.Models.WorkflowExecutionLogRecord;
using WorkflowInstanceModel = Elsa.Models.WorkflowInstance;

namespace Passingwind.Abp.ElsaModule.Stores;

public class StoreMapper : IStoreMapper
{
    private readonly Volo.Abp.Timing.IClock _clock;
    private readonly IGuidGenerator _guidGenerator;

    public StoreMapper(Volo.Abp.Timing.IClock clock, IGuidGenerator guidGenerator)
    {
        _clock = clock;
        _guidGenerator = guidGenerator;
    }

    protected virtual DateTime ToDateTime(Instant instant)
    {
        if (_clock.Kind == DateTimeKind.Utc)
        {
            return instant.ToDateTimeUtc();
        }
        else
        {
            return instant.ToDateTimeUtc().ToLocalTime();
        }
    }

    protected virtual Instant ToInstant(DateTime dateTime)
    {
        if (_clock.Kind == DateTimeKind.Utc)
        {
            return Instant.FromDateTimeUtc(dateTime);
        }
        else
        {
            return Instant.FromDateTimeUtc(dateTime.ToUniversalTime());
        }
    }

    public virtual WorkflowDefinitionModel MapToModel(WorkflowDefinitionVersion entity, WorkflowDefinition definition)
    {
        var model = new WorkflowDefinitionModel
        {
            Id = entity.Id.ToString(), // version id
            DefinitionId = definition.Id.ToString(),
            // VersionId = entity.Id.ToString(), // same as id
            Version = entity.Version,
            Description = definition.Description,

            DisplayName = definition.DisplayName,
            Name = definition.Name,

            IsSingleton = definition.IsSingleton,
            DeleteCompletedInstances = definition.DeleteCompletedInstances,
            Tag = definition.Tag,
            TenantId = definition.TenantId?.ToString(),

            Channel = definition.Channel,

            ContextOptions = definition.ContextOptions,

            CustomAttributes = new Elsa.Models.Variables(definition.CustomAttributes ?? new Dictionary<string, object>()),
            Variables = new Elsa.Models.Variables(definition.Variables ?? new Dictionary<string, object>()),
            PersistenceBehavior = definition.PersistenceBehavior,


            IsLatest = entity.IsLatest,
            IsPublished = entity.IsPublished,

            Connections = entity.Connections.Select(x => MapToModel(x)).ToArray(),
            Activities = entity.Activities.Select(x => MapToModel(x)).ToArray(),

            CreatedAt = ToInstant(entity.CreationTime),
        };

        return model;
    }

    public virtual ActivityDefinition MapToModel(Activity entity)
    {
        return new ActivityDefinition
        {
            ActivityId = entity.ActivityId.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            DisplayName = entity.DisplayName,
            LoadWorkflowContext = entity.LoadWorkflowContext,
            PersistWorkflow = entity.PersistWorkflow,
            Properties = entity.Properties,
            SaveWorkflowContext = entity.SaveWorkflowContext,
            Type = entity.Type,
            PropertyStorageProviders = entity.PropertyStorageProviders,
        };
    }

    public virtual ConnectionDefinitionModel MapToModel(ActivityConnection entity)
    {
        return new ConnectionDefinitionModel
        {
            Outcome = entity.Outcome,
            SourceActivityId = entity.SourceId.ToString(),
            TargetActivityId = entity.TargetId.ToString(),
        };
    }

    public virtual WorkflowDefinitionVersion MapToEntity(WorkflowDefinitionModel model)
    {
        throw new NotImplementedException();
        //    var id = Guid.Empty;
        //    Guid.TryParse(model.Id, out id);
        //    var definitionId = Guid.Empty;
        //    Guid.TryParse(model.DefinitionId, out id);

        //    return new WorkflowDefinitionVersion(id)
        //    {
        //        Definition = new Common.WorkflowDefinition
        //        {
        //            Description = model.Description,
        //            Name = model.Name,
        //            DisplayName = model.DisplayName,
        //            Channel = model.Channel,
        //            IsSingleton = model.IsSingleton,
        //            DeleteCompletedInstances = model.DeleteCompletedInstances,
        //            Tag = model.Tag,

        //            ContextOptions = model.ContextOptions,

        //            CustomAttributes = model.CustomAttributes,
        //            PersistenceBehavior = model.PersistenceBehavior,
        //            Variables = model.Variables,
        //        },

        //        Activities = model.Activities.Select(x => MapToEntity(x)).ToList(),
        //        Connections = model.Connections.Select(x => MapToEntity(x)).ToList(),

        //        DefinitionId = definitionId,
        //        Version = model.Version,
        //    };
    }

    public virtual WorkflowDefinitionVersion MapToEntity(WorkflowDefinitionModel model, WorkflowDefinitionVersion entity)
    {
        throw new NotImplementedException();
        //    var id = Guid.Empty;
        //    Guid.TryParse(model.Id, out id);
        //    var definitionId = Guid.Empty;
        //    Guid.TryParse(model.DefinitionId, out id);

        //    if (entity.DefinitionId == null)
        //        entity.Definition = new Common.WorkflowDefinition();

        //    entity.Definition.Description = model.Description;
        //    entity.Definition.Name = model.Name;
        //    entity.Definition.DisplayName = model.DisplayName;
        //    entity.Definition.Channel = model.Channel;
        //    entity.Definition.IsSingleton = model.IsSingleton;
        //    entity.Definition.DeleteCompletedInstances = model.DeleteCompletedInstances;
        //    // entity.Definition.IsPublished = model.IsPublished;
        //    entity.Definition.Tag = model.Tag;
        //    entity.Definition.ContextOptions = model.ContextOptions;
        //    entity.Definition.CustomAttributes = model.CustomAttributes;
        //    entity.Definition.PersistenceBehavior = model.PersistenceBehavior;
        //    entity.Definition.Variables = model.Variables;

        //    entity.Activities = model.Activities.Select(x => MapToEntity(x)).ToList();
        //    entity.Connections = model.Connections.Select(x => MapToEntity(x)).ToList();
        //    entity.DefinitionId = definitionId;
        //    entity.Version = model.Version;

        //    return entity;
    }

    public virtual Activity MapToEntity(ActivityDefinition model)
    {
        return new Activity(
            Guid.Parse(model.ActivityId),
            model.Type,
            model.Name,
            model.DisplayName,
            model.Description,
            model.PersistWorkflow,
            model.LoadWorkflowContext,
            model.SaveWorkflowContext,
            default,
            model.Properties?.ToList(),
            model.PropertyStorageProviders.ToDictionary(x => x.Key, x => x.Value));
    }

    public virtual ActivityConnection MapToEntity(ConnectionDefinitionModel model)
    {
        return new ActivityConnection(Guid.Parse(model.SourceActivityId), Guid.Parse(model.TargetActivityId), model.Outcome, default);
    }

    public virtual Bookmark MapToEntity(BookmarkModel model)
    {
        return new Bookmark(model.Id.ToGuid().Value)
        {
            ActivityId = Guid.Parse(model.ActivityId),
            ActivityType = model.ActivityType,
            CorrelationId = model.CorrelationId,
            Hash = model.Hash,
            Model = model.Model,
            ModelType = model.ModelType,
            WorkflowInstanceId = Guid.Parse(model.WorkflowInstanceId),
        };
    }

    public virtual Bookmark MapToEntity(BookmarkModel model, Bookmark entity)
    {
        entity.ActivityId = Guid.Parse(model.ActivityId);
        entity.ActivityType = model.ActivityType;
        entity.CorrelationId = model.CorrelationId;
        entity.Hash = model.Hash;
        entity.Model = model.Model;
        entity.ModelType = model.ModelType;
        // entity.WorkflowInstanceId = Guid.Parse(model.WorkflowInstanceId);

        return entity;
    }

    public virtual BookmarkModel MapToModel(Bookmark entity)
    {
        return new BookmarkModel()
        {
            Id = entity.Id.ToString(),
            TenantId = entity.TenantId?.ToString(),
            ActivityId = entity.ActivityId.ToString(),
            ActivityType = entity.ActivityType,
            CorrelationId = entity.CorrelationId,
            Hash = entity.Hash,
            Model = entity.Model,
            ModelType = entity.ModelType,
            WorkflowInstanceId = entity.WorkflowInstanceId.ToString(),
        };
    }

    public virtual Trigger MapToEntity(TriggerModel model)
    {
        return new Trigger(model.Id.ToGuid().Value)
        {
            ActivityId = Guid.Parse(model.ActivityId),
            ActivityType = model.ActivityType,
            Hash = model.Hash,
            Model = model.Model,
            ModelType = model.ModelType,
            WorkflowDefinitionId = Guid.Parse(model.WorkflowDefinitionId),
            // WorkflowDefinitionVersionId = Guid.Parse(model.WorkflowDefinitionId),
        };
    }

    public virtual Trigger MapToEntity(TriggerModel model, Trigger entity)
    {
        entity.ActivityId = Guid.Parse(model.ActivityId);
        entity.ActivityType = model.ActivityType;
        entity.Hash = model.Hash;
        entity.Model = model.Model;
        entity.ModelType = model.ModelType;
        // entity.WorkflowDefinitionVersionId = Guid.Parse(model.WorkflowDefinitionId);

        return entity;
    }

    public virtual TriggerModel MapToModel(Trigger entity)
    {
        return new TriggerModel
        {
            Id = entity.Id.ToString(),
            TenantId = entity.TenantId?.ToString(),
            ActivityId = entity.ActivityId.ToString(),
            ActivityType = entity.ActivityType,
            Hash = entity.Hash,
            Model = entity.Model,
            ModelType = entity.ModelType,
            WorkflowDefinitionId = entity.WorkflowDefinitionId.ToString(),
        };
    }

    public virtual WorkflowExecutionLog MapToEntity(WorkflowExecutionLogRecordModel model)
    {
        return new WorkflowExecutionLog()
        {
            ActivityId = Guid.Parse(model.ActivityId),
            ActivityType = model.ActivityType,
            Data = model.Data?.ToObject<Dictionary<string, object>>(),
            EventName = model.EventName,
            Message = model.Message,
            Source = model.Source,
            Timestamp = ToDateTime(model.Timestamp),
            WorkflowInstanceId = Guid.Parse(model.WorkflowInstanceId),
        };
    }

    public virtual WorkflowExecutionLog MapToEntity(WorkflowExecutionLogRecordModel model, WorkflowExecutionLog entity)
    {
        entity.ActivityId = Guid.Parse(model.ActivityId);
        entity.ActivityType = model.ActivityType;
        entity.Data = model.Data?.ToObject<Dictionary<string, object>>();
        entity.EventName = model.EventName;
        entity.Message = model.Message;
        entity.Source = model.Source;
        entity.Timestamp = ToDateTime(model.Timestamp);
        // entity.WorkflowInstanceId = Guid.Parse(model.WorkflowInstanceId);

        return entity;
    }

    public virtual WorkflowExecutionLogRecordModel MapToModel(WorkflowExecutionLog entity)
    {
        return new WorkflowExecutionLogRecordModel
        {
            Id = entity.Id.ToString(),
            ActivityId = entity.ActivityId.ToString(),
            ActivityType = entity.ActivityType,
            Data = entity.Data == null ? null : JObject.FromObject(entity.Data),
            EventName = entity.EventName,
            Message = entity.Message,
            Source = entity.Source,
            Timestamp = ToInstant(entity.Timestamp),
            WorkflowInstanceId = entity.WorkflowInstanceId.ToString(),
        };
    }

    public virtual WorkflowInstance MapToEntity(WorkflowInstanceModel model)
    {
        return new WorkflowInstance(model.Id.ToGuid().Value)
        {
            Name = model.Name,

            Version = model.Version,
            WorkflowStatus = (WorkflowInstanceStatus)(int)model.WorkflowStatus,

            CancelledTime = model.CancelledAt.HasValue ? ToDateTime(model.CancelledAt.Value) : null,
            FaultedTime = model.FaultedAt.HasValue ? ToDateTime(model.FaultedAt.Value) : null,
            FinishedTime = model.FinishedAt.HasValue ? ToDateTime(model.FinishedAt.Value) : null,
            LastExecutedTime = model.LastExecutedAt.HasValue ? ToDateTime(model.LastExecutedAt.Value) : null,

            ContextId = model.ContextId,
            ContextType = model.ContextType,
            CorrelationId = model.CorrelationId,
            CurrentActivity = model.CurrentActivity == null ? default : new WorkflowInstanceScheduledActivity { ActivityId = Guid.Parse(model.CurrentActivity.ActivityId), Input = model.CurrentActivity.Input },
            LastExecutedActivityId = model.LastExecutedActivityId.ToGuid(),

            WorkflowDefinitionId = Guid.Parse(model.DefinitionId),
            WorkflowDefinitionVersionId = Guid.Parse(model.DefinitionVersionId),

            Faults = model.Faults.Select(x => new WorkflowInstanceFault(_guidGenerator.Create())
            {
                ActivityInput = x.ActivityInput,
                FaultedActivityId = x.FaultedActivityId?.ToGuid(),
                Message = x.Message,
                Resuming = x.Resuming,
                Exception = SimpleExceptionModel.FromException(x.Exception),
            }).ToList(),
            Input = model.Input,
            Output = model.Output,

            Metadata = model.Metadata.Select(x => new WorkflowInstanceMetadata { Key = x.Key, Value = x.Value }).ToList(),
            Variables = model.Variables.Data.Select(x => new WorkflowInstanceVariable { Key = x.Key, Value = x.Value }).ToList(),
            ActivityData = model.ActivityData.ToList().Select(x => new WorkflowInstanceActivityData() { ActivityId = Guid.Parse(x.Key), Data = (Dictionary<string, object>)x.Value }).ToList(),
            BlockingActivities = model.BlockingActivities.Select(x => new WorkflowInstanceBlockingActivity { ActivityId = Guid.Parse(x.ActivityId), ActivityType = x.ActivityType, Tag = x.Tag, }).ToList(),
            ScheduledActivities = model.ScheduledActivities.Select(x => new WorkflowInstanceScheduledActivity { ActivityId = Guid.Parse(x.ActivityId), Input = x.Input, }).ToList(),
            ActivityScopes = model.Scopes.Select(x => new WorkflowInstanceActivityScope { ActivityId = Guid.Parse(x.ActivityId), Variables = (Dictionary<string, object>)x.Variables.Data, }).ToList(),

        };
    }

    public virtual WorkflowInstance MapToEntity(WorkflowInstanceModel model, WorkflowInstance entity)
    {
        if (!string.IsNullOrEmpty(model.Name))
            entity.Name = model.Name;

        entity.Version = model.Version;
        entity.WorkflowStatus = (WorkflowInstanceStatus)(int)model.WorkflowStatus;

        entity.CancelledTime = model.CancelledAt.HasValue ? ToDateTime(model.CancelledAt.Value) : null;
        entity.FaultedTime = model.FaultedAt.HasValue ? ToDateTime(model.FaultedAt.Value) : null;
        entity.FinishedTime = model.FinishedAt.HasValue ? ToDateTime(model.FinishedAt.Value) : null;
        entity.LastExecutedTime = model.LastExecutedAt.HasValue ? ToDateTime(model.LastExecutedAt.Value) : null;

        entity.ContextId = model.ContextId;
        entity.ContextType = model.ContextType;
        entity.CorrelationId = model.CorrelationId;
        entity.CurrentActivity = model.CurrentActivity == null ? default : new WorkflowInstanceScheduledActivity { ActivityId = Guid.Parse(model.CurrentActivity.ActivityId), Input = model.CurrentActivity.Input };
        entity.LastExecutedActivityId = model.LastExecutedActivityId.ToGuid();
         
        // find new
        var newFaults = (model.Faults.Select(x => new WorkflowInstanceFault(_guidGenerator.Create())
        {
            WorkflowInstanceId = entity.Id,
            ActivityInput = x.ActivityInput,
            FaultedActivityId = x.FaultedActivityId.ToGuid(),
            Message = x.Message,
            Resuming = x.Resuming,
            Exception = SimpleExceptionModel.FromException(x.Exception),
        })).Except(entity.Faults, new WorkflowInstanceFaultEqualityComparer());
        entity.Faults.AddRange(newFaults);

        entity.Input = model.Input;
        entity.Output = model.Output;
        // TODO
        entity.Metadata = model.Metadata.Select(x => new WorkflowInstanceMetadata { Key = x.Key, Value = x.Value }).ToList();
        entity.Variables = model.Variables.Data.Select(x => new WorkflowInstanceVariable { Key = x.Key, Value = x.Value }).ToList();
        entity.ActivityData = model.ActivityData.ToList().Select(x => new WorkflowInstanceActivityData() { ActivityId = Guid.Parse(x.Key), Data = (Dictionary<string, object>)x.Value }).ToList();
        entity.BlockingActivities = model.BlockingActivities.Select(x => new WorkflowInstanceBlockingActivity { ActivityId = Guid.Parse(x.ActivityId), ActivityType = x.ActivityType, Tag = x.Tag, }).ToList();
        entity.ScheduledActivities = model.ScheduledActivities.Select(x => new WorkflowInstanceScheduledActivity { ActivityId = Guid.Parse(x.ActivityId), Input = x.Input, }).ToList();
        entity.ActivityScopes = model.Scopes.Select(x => new WorkflowInstanceActivityScope { ActivityId = Guid.Parse(x.ActivityId), Variables = (Dictionary<string, object>)x.Variables.Data, }).ToList();

        return entity;
    }

    public virtual WorkflowInstanceModel MapToModel(WorkflowInstance entity)
    {
        return new WorkflowInstanceModel
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,

            DefinitionId = entity.WorkflowDefinitionId.ToString(),
            DefinitionVersionId = entity.WorkflowDefinitionVersionId.ToString(),

            TenantId = entity.TenantId?.ToString(),

            Version = entity.Version,
            WorkflowStatus = (Elsa.Models.WorkflowStatus)(int)entity.WorkflowStatus,

            CreatedAt = ToInstant(entity.CreationTime),
            CancelledAt = entity.CancelledTime == null ? null : ToInstant(entity.CancelledTime.Value),
            FaultedAt = entity.FaultedTime == null ? null : ToInstant(entity.FaultedTime.Value),
            FinishedAt = entity.FinishedTime == null ? null : ToInstant(entity.FinishedTime.Value),
            LastExecutedAt = entity.LastExecutedTime == null ? null : ToInstant(entity.LastExecutedTime.Value),

            ContextId = entity.ContextId,
            ContextType = entity.ContextType,
            CorrelationId = entity.CorrelationId,
            CurrentActivity = entity.CurrentActivity == null ? default : new Elsa.Models.ScheduledActivity(entity.CurrentActivity.ActivityId.ToString(), entity.CurrentActivity.Input),
            LastExecutedActivityId = entity.LastExecutedActivityId?.ToString(),

            Faults = new Elsa.Models.SimpleStack<Elsa.Models.WorkflowFault>(entity.Faults.Select(x => new Elsa.Models.WorkflowFault(x.Exception.ToException(), x.Message, x.FaultedActivityId.ToString(), x.ActivityInput, x.Resuming))),
            Input = entity.Input,
            Output = entity.Output,

            Metadata = entity.GetMetadata(),
            Variables = new Elsa.Models.Variables(entity.GetVariables()),
            ActivityData = entity.ActivityData?.ToDictionary(x => x.ActivityId.ToString(), x => (IDictionary<string, object>)x.Data) ?? new Dictionary<string, IDictionary<string, object>>(),
            BlockingActivities = entity.BlockingActivities?.Select(x => new BlockingActivity(x.ActivityId.ToString(), x.ActivityType) { Tag = x.Tag })?.ToHashSet() ?? new HashSet<BlockingActivity>(),
            ScheduledActivities = new Elsa.Models.SimpleStack<Elsa.Models.ScheduledActivity>(entity.ScheduledActivities?.Select(x => new Elsa.Models.ScheduledActivity(x.ActivityId.ToString(), x.Input)) ?? new List<Elsa.Models.ScheduledActivity>()),
            Scopes = new Elsa.Models.SimpleStack<Elsa.Models.ActivityScope>(entity.ActivityScopes?.Select(x => new Elsa.Models.ActivityScope() { ActivityId = x.ActivityId.ToString(), Variables = new Elsa.Models.Variables(x.Variables) })?.ToList() ?? new List<Elsa.Models.ActivityScope>()),

        };
    }
}
