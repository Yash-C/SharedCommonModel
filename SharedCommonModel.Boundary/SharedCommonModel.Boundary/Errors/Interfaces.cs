namespace SharedCommonModel.Boundary.Errors;

public interface IAppSyncError : IModelScopedIdentity, IModelScope
{
    int ErrorLevel { get; set; }
    string ErrorCode { get; set; }
    string ErrorMessage { get; set; }
}

public interface IAppSyncHasErrors
{
    bool HasErrors { get; }
}

public interface IAppSyncHasErrorsArray : IAppSyncHasErrors
{
    IAppSyncError[] Errors { get; set; }
}

public interface IAppSyncHasErrorsList : IAppSyncHasErrors
{
    IList<IAppSyncError> Errors { get; }
}