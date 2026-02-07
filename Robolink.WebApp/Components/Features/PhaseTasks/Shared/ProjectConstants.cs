namespace Robolink.WebApp.Components.Features.PhaseTasks.Shared;

public static class ProjectConstants
{
    // Default Client and Manager for Quick Add
    public static readonly Guid DefaultClientId = Guid.Parse("188cd869-567e-4cd2-870a-48bdb04af5cd");
    public static readonly Guid DefaultManagerId = Guid.Parse("1b8c3dbf-63bb-4207-b108-9b28706185a7");
    
    // Project Defaults
    public const int DefaultPageSize = 10;
    public const int DefaultProjectPriority = 1;
    public const int DefaultInternalBudget = 1000;
    public const int DefaultCustomerBudget = 2000;
    public const int DefaultProjectDurationDays = 30;
    
    // Project Code Prefix
    public const string ProjectCodePrefix = "APTX";
}
