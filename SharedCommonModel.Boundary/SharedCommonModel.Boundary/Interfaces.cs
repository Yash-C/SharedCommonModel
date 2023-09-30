namespace SharedCommonModel.Boundary;

public interface IRecordHasId
{
    int Id { get; init; }
}

public interface IRecordHasUuid
{
    Guid Uuid { get; init; }
}

public interface IRecordHasDataState
{
    DataState DataState { get; init; }
}

public interface IRecordHasRowVersion
{
    byte[] RowVersion { get; init; }
}
public interface IRecordIdentifiedBasic : IRecordHasId, IRecordHasUuid { }

public interface IRecordIdentified : IRecordIdentifiedBasic, IRecordHasDataState, IRecordHasRowVersion
{
    bool Inactive { get; init; }
    bool Deleted { get; init; }
}



public interface IModelHasId
{
    int Id { get; set; }
}

public interface IModelHasUuid
{
    Guid Uuid { get; set; }
}

public interface IModelHasDataState
{
    DataState DataState { get; set; }
}

public interface IModelHasRowVersion
{
    byte[] RowVersion { get; set; }
}


public interface IModelHasDictionaryKey
{
    string DictionaryKey { get; }
}

public interface IModelIdentifiedBasic : IModelHasId, IModelHasUuid { }

public interface IModelIdentified : IModelIdentifiedBasic, IModelHasDataState, IModelHasRowVersion
{
    bool Inactive { get; set; }
    bool Deleted { get; set; }
}

public interface IModelScopedIdentity
{
    long? ScopedObjectId { get; set; }
    Guid? ScopedObjectGuid { get; set; }
    string ScopedObjectText { get; set; }
}

public interface IModelScope
{
    int SysObjectId { get; set; }
    int SystemId { get; set; }
}

public interface IModelScopeApplication : IModelScope
{
    string ApplicationInstance { get; set; }
}

public interface IModelScopeDataSet : IModelScope
{
    public string DataSetType { get; set; }
    public string DataSetSubType { get; set; }
}
public interface IModelScopeApplicationDataSet : IModelScopeApplication, IModelScopeDataSet
{
}

public interface IModelScopeEntityObject
{
    int SysObjectId { get; set; }
    string ScopedObjectId { get; set; }
}

public interface IStateModel : IRecordHasId
{
    string Name { get; init; }
}

public interface IStateModel<T>  : IStateModel
    where T : struct
{
    T EnumerationValue { get; init; }
}
public interface IStateCodedModel : IStateModel
{
    string Code { get; init; }
}

public interface IStateCodedModel<T>  : IStateCodedModel, IStateModel<T>
    where T : struct
{
}