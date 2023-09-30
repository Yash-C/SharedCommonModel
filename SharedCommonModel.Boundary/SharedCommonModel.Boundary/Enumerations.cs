namespace SharedCommonModel.Boundary;

[Flags]
public enum DataState
{
    None = 0,                       //    0
    Working = 1 << 0,               //    1
    Posted = 1 << 1,                //    2
    Open = 1 << 2,                  //    4
    History = 1 << 3,               //    8
    Archive = 1 << 4,               //   16
    ToRemove = 1 << 5,              //   32
    Inactive = 1 << 6,              //   64
    Deleted = 1 << 7,               //  128
    PendingApproval = 1 << 8,       //  256
    WorkingApproved = 1 << 9,       //  512
    WorkingDeclined = 1 << 10,      //1,024
    PendingUpdate = 1 << 11,        //2,048
}

[Flags]
public enum CrudTargetFlags
{
    Any = 0,
    Create = 1 << 0,
    Insert = 1 << 1,
    Update = 1 << 2,
    Delete = 1 << 3
}

[Flags]
public enum SyncTargetDeleteMethod
{
    Inactive = 1 << 0,
    DeletedFlag = 1 << 1,
    HardDelete = 1 << 2,
    DataState = 1 << 3
}

[Flags]
public enum SyncTypeFlags
{
    None = 0,
    Get = 1 << 0,
    Push = 1 << 1,
    WriteNewToDb = 1 << 2,
    NotifyNew = 1 << 3,
    NewOnlyToApi = 1 << 4,
    UpdateOnlyToApi = 1 << 5,
    UpsertToApi = 1 << 6
}