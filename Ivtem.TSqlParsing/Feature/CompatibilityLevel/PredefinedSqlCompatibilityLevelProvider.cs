namespace Ivtem.TSqlParsing.Feature.CompatibilityLevel;

public sealed class PredefinedSqlCompatibilityLevelProvider : ISqlCompatibilityLevelProvider
{
    private TSqlCompatibilityLevel CompatibilityLevel { get; }

    public PredefinedSqlCompatibilityLevelProvider(TSqlCompatibilityLevel compatibilityLevel)
    {
        CompatibilityLevel = compatibilityLevel;
    }

    public TSqlCompatibilityLevel GetCompatibilityLevel() => CompatibilityLevel;
}