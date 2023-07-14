using Ivtem.TSqlParsing.Feature.CompatibilityLevel;

namespace Ivtem.DatabaseTools.Feature.DatabaseService;

public interface IDatabaseService
{
    string DataSource { get; }
    string InitialCatalog { get; }
    string ConnectionString { get; }
    TSqlCompatibilityLevel CompatibilityLevel { get; }
}