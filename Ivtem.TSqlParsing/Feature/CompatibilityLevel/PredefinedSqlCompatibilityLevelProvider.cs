namespace Ivtem.TSqlParsing.Feature.CompatibilityLevel;

public sealed class PredefinedSqlCompatibilityLevelProvider : ISqlCompatibilityLevelProvider
{
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql80Provider 
        = new(TSqlCompatibilityLevel.TSql80);
    
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql90Provider 
        = new(TSqlCompatibilityLevel.TSql90);
    
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql100Provider 
        = new(TSqlCompatibilityLevel.TSql100);
    
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql110Provider 
        = new(TSqlCompatibilityLevel.TSql110);
    
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql120Provider 
        = new(TSqlCompatibilityLevel.TSql120);
    
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql130Provider 
        = new(TSqlCompatibilityLevel.TSql130);
    
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql140Provider 
        = new(TSqlCompatibilityLevel.TSql140);
    
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql150Provider 
        = new(TSqlCompatibilityLevel.TSql150);
    
    public static readonly PredefinedSqlCompatibilityLevelProvider TSql160Provider 
        = new(TSqlCompatibilityLevel.TSql160);
    
    private TSqlCompatibilityLevel CompatibilityLevel { get; }

    public PredefinedSqlCompatibilityLevelProvider(TSqlCompatibilityLevel compatibilityLevel)
    {
        CompatibilityLevel = compatibilityLevel;
    }

    public TSqlCompatibilityLevel GetCompatibilityLevel() => CompatibilityLevel;
}