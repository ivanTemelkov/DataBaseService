namespace Ivtem.TSqlParsing.Feature.CompatibilityLevel;

public interface ISqlCompatibilityLevelProvider
{
    TSqlCompatibilityLevel? CompatibilityLevel { get; }
    string DataSource { get; }
    string InitialCatalog { get; }
    string ConnectionString { get; }

    TSqlCompatibilityLevel GetCompatibilityLevel();
}