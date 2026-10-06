# Ivtem.TSqlParsing

A set of tools for parsing, analyzing, and generating T-SQL scripts in .NET, built on top of `Microsoft.SqlServer.TransactSql.ScriptDom`.

Copyright by Ivan Temelkov under the Apache-2.0 license.

Original GitHub repository: https://github.com/ivanTemelkov/DataBaseService

---

## Public API Reference

### 1. Dependency Injection Extensions

**Namespace**: `Ivtem.TSqlParsing.Extensions`

#### `public static class ServiceCollectionExtensions`
Extension methods for registering SQL parsing services into `Microsoft.Extensions.DependencyInjection.IServiceCollection`.

- `public static IServiceCollection AddSqlParsing(this IServiceCollection sc, string connectionString)`  
  Registers SQL parsing services (`ISqlGeneratorFactory`, `ISqlFragmentProvider`, `ISelectColumnNamesProvider`, `ISelectQueryFieldNamesProvider`, `ISqlFragmentAndGeneratorProvider`, `ISelectStatementProvider`) and configures `SqlCompatibilityLevelProvider` with the provided connection string.
- `public static IServiceCollection AddSqlParsing(this IServiceCollection sc, Func<string> connectionStringConfig)`  
  Registers SQL parsing services using a delegate to lazily resolve the connection string.
- `public static IServiceCollection AddSqlParsing(this IServiceCollection sc, Func<ISqlCompatibilityLevelProvider> getCompatibilityLevelProvider)`  
  Registers SQL parsing services with a custom `ISqlCompatibilityLevelProvider` factory delegate.

---

### 2. SQL Compatibility Level

**Namespace**: `Ivtem.TSqlParsing.Feature.CompatibilityLevel`

#### `public enum TSqlCompatibilityLevel`
Enum representing supported SQL Server compatibility levels:
- `TSql80` (SQL Server 2000)
- `TSql90` (SQL Server 2005)
- `TSql100` (SQL Server 2008)
- `TSql110` (SQL Server 2012)
- `TSql120` (SQL Server 2014)
- `TSql130` (SQL Server 2016)
- `TSql140` (SQL Server 2017)
- `TSql150` (SQL Server 2019)
- `TSql160` (SQL Server 2022)

#### `public interface ISqlCompatibilityLevelProvider`
Provides the target SQL Server compatibility level.
- `TSqlCompatibilityLevel GetCompatibilityLevel()`

#### `public sealed class PredefinedSqlCompatibilityLevelProvider : ISqlCompatibilityLevelProvider`
Provider using a fixed/predefined `TSqlCompatibilityLevel`.
- **Static Providers**:
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql80Provider`
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql90Provider`
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql100Provider`
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql110Provider`
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql120Provider`
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql130Provider`
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql140Provider`
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql150Provider`
  - `public static readonly PredefinedSqlCompatibilityLevelProvider TSql160Provider`
- **Constructor**:
  - `public PredefinedSqlCompatibilityLevelProvider(TSqlCompatibilityLevel compatibilityLevel)`
- **Methods**:
  - `public TSqlCompatibilityLevel GetCompatibilityLevel()`

#### `public class SqlCompatibilityLevelProvider : ISqlCompatibilityLevelProvider`
Resolves the SQL Server compatibility level by querying `sys.databases` over a SQL connection.
- **Properties**:
  - `public TSqlCompatibilityLevel? CompatibilityLevel { get; }`
- **Constructor**:
  - `public SqlCompatibilityLevelProvider(string connectionString)`
- **Methods**:
  - `public TSqlCompatibilityLevel GetCompatibilityLevel()`

---

### 3. SQL Parser & Fragments

**Namespace**: `Ivtem.TSqlParsing.Feature.SqlParser`

#### `public class TSqlParserFactory`
Factory for creating `TSqlParser` instances matching the target `TSqlCompatibilityLevel`.
- **Constructor**:
  - `public TSqlParserFactory(bool initialQuotedIdentifiers, SqlEngineType sqlEngineType)`
- **Methods**:
  - `public TSqlParser GetParser(TSqlCompatibilityLevel compatibilityLevel)`

**Namespace**: `Ivtem.TSqlParsing.Feature.SqlFragment`

#### `public interface ISqlFragmentProvider`
Parses SQL query text into a ScriptDom `TSqlFragment`.
- `bool TryGetSqlFragment(string query, [NotNullWhen(true)] out TSqlFragment? sqlFragment, [NotNullWhen(false)] out ParseError[]? parseErrors)`
- `bool TryGetSqlFragment(string query, TSqlCompatibilityLevel compatibilityLevel, [NotNullWhen(true)] out TSqlFragment? sqlFragment, [NotNullWhen(false)] out ParseError[]? parseErrors)`

#### `public class TSqlFragmentProvider : ISqlFragmentProvider`
Default implementation of `ISqlFragmentProvider`. Defaults to `TSqlCompatibilityLevel.TSql160` when no level is specified.
- **Methods**:
  - `public bool TryGetSqlFragment(string query, TSqlCompatibilityLevel compatibilityLevel, [NotNullWhen(true)] out TSqlFragment? sqlFragment, [NotNullWhen(false)] out ParseError[]? parseErrors)`
  - `public bool TryGetSqlFragment(string query, [NotNullWhen(true)] out TSqlFragment? sqlFragment, [NotNullWhen(false)] out ParseError[]? parseErrors)`

#### `public abstract class GetDataVisitor<T> : TSqlConcreteFragmentVisitor`
Base ScriptDom visitor for extracting data from an AST.
- **Methods**:
  - `public T GetData()`
- **Protected Members**:
  - `protected bool IsVisitCalled { get; set; }`
  - `protected abstract T GetVisitorData()`

#### `public class FromClauseVisitor : TSqlConcreteFragmentVisitor`
Visitor for traversing `FROM` clauses and extracting table references.
- **Methods**:
  - `public override void Visit(FromClause node)`
  - `public override void Visit(NamedTableReference node)`
  - `public ImmutableArray<TableReferenceInfo> GetTables()`

#### `public class GetColumnNamesVisitor : GetDataVisitor<ImmutableArray<string>>`
Visitor for extracting column names and aliases from `SelectStatement` or `SelectScalarExpression`.
- **Properties**:
  - `public string? ColumnName { get; }`
- **Methods**:
  - `public override void ExplicitVisit(SelectStatement node)`
  - `public override void ExplicitVisit(SelectScalarExpression node)`

#### `public class GetSelectStatementVisitor : GetDataVisitor<SelectStatement?>`
Visitor for retrieving the first `SelectStatement` AST node from a script or batch.
- **Methods**:
  - `public override void ExplicitVisit(TSqlScript node)`
  - `public override void ExplicitVisit(TSqlBatch node)`

#### `public sealed class UnsafeSqlVisitor : TSqlFragmentVisitor`
Visitor detecting potentially dangerous functions, extended stored procedures, and system objects.
- **Static Fields**:
  - `public static readonly HashSet<string> DangerousBuiltInFunctions`
- **Properties**:
  - `public string? Warning { get; }`
  - `public bool IsUnsafe => Warning is not null;`
- **Methods**:
  - Overrides for `OpenRowsetTableReference`, `OpenQueryTableReference`, `AdHocTableReference`, `SchemaObjectFunctionTableReference`, `FunctionCall`, `SchemaObjectName`, and `NamedTableReference`.

---

### 4. Select Query Analysis

**Namespace**: `Ivtem.TSqlParsing.Feature.SelectQuery`

#### `public interface IBlockCommentProvider`
- `bool TryGetBlockComments(TSqlFragment sqlFragment, [NotNullWhen(true)] out string[]? commentBlocks)`

#### `public class BlockCommentProvider : IBlockCommentProvider`
Extracts multiline block comments from a `TSqlFragment` token stream.
- `public bool TryGetBlockComments(TSqlFragment sqlFragment, [NotNullWhen(true)] out string[]? commentBlocks)`

#### `public interface ISelectColumnNamesProvider`
- `ImmutableArray<string> GetColumnNames(TSqlFragment sqlFragment)`

#### `public class SelectColumnNamesProvider : ISelectColumnNamesProvider`
Extracts column names from a parsed `TSqlFragment`.
- `public ImmutableArray<string> GetColumnNames(TSqlFragment sqlFragment)`

#### `public interface ISelectQueryFieldNamesProvider`
- `ImmutableArray<string> GetFieldNames(string sql, TSqlCompatibilityLevel compatibilityLevel)`
- `ImmutableArray<string> GetFieldNames(string sql)`

#### `public class SelectQueryFieldNamesProvider : ISelectQueryFieldNamesProvider`
Extracts field names directly from raw SQL queries.
- **Constructor**:
  - `public SelectQueryFieldNamesProvider(ISqlFragmentProvider sqlFragmentProvider, ISqlCompatibilityLevelProvider compatibilityLevelProvider, ISelectColumnNamesProvider selectColumnNamesProvider)`
- **Methods**:
  - `public ImmutableArray<string> GetFieldNames(string sql, TSqlCompatibilityLevel compatibilityLevel)`
  - `public ImmutableArray<string> GetFieldNames(string sql)`

#### `public interface ISelectStatementProvider`
- `bool TryGetStatement(TSqlFragment sqlFragment, [NotNullWhen(true)] out SelectStatement? selectStatement)`

#### `public class SelectStatementProvider : ISelectStatementProvider`
Extracts a `SelectStatement` AST node from a `TSqlFragment`.
- `public bool TryGetStatement(TSqlFragment sqlFragment, [NotNullWhen(true)] out SelectStatement? selectStatement)`

#### `public class TableNamesProvider`
Extracts table references from a `TSqlFragment`.
- **Methods**:
  - `public ImmutableArray<TableReferenceInfo> GetTableNames(TSqlFragment sqlFragment)`

#### `public record TableReferenceInfo(string? ServerIdentifier, string? DatabaseIdentifier, string? SchemaIdentifier, string? BaseIdentifier)`
Model representing parsed multipart table identifiers (Server, Database, Schema, Table).

#### `public sealed class UnsafeSelectQueryDetector`
Analyzes a `SelectStatement` for unsafe constructs.
- **Constructor**:
  - `public UnsafeSelectQueryDetector(SelectStatement selectStatement)`
- **Methods**:
  - `public bool IsSafe([NotNullWhen(returnValue: false)] out string? warning)`

---

### 5. SQL Script Generation

**Namespace**: `Ivtem.TSqlParsing.Feature.SqlGenerator`

#### `public class DefaultSqlScriptGenerator`
Generates formatted SQL text from a `TSqlFragment`.
- **Constructor**:
  - `public DefaultSqlScriptGenerator(SqlScriptGenerator scriptGenerator)`
- **Methods**:
  - `public string Generate(TSqlFragment fragment)`

#### `public interface ISqlGeneratorFactory`
- `DefaultSqlScriptGenerator GetGenerator(TSqlCompatibilityLevel compatibilityLevel)`

#### `public class SqlGeneratorFactory : ISqlGeneratorFactory`
Factory for creating `DefaultSqlScriptGenerator` instances configured with standard formatting options for a given compatibility level.
- **Constructor**:
  - `public SqlGeneratorFactory(SqlScriptGeneratorOptions? generatorOptions = null)`
- **Methods**:
  - `public DefaultSqlScriptGenerator GetGenerator(TSqlCompatibilityLevel compatibilityLevel)`

---

### 6. Combined Fragment & Generator Provider

**Namespace**: `Ivtem.TSqlParsing.Feature`

#### `public interface ISqlFragmentAndGeneratorProvider`
- `TSqlCompatibilityLevel GetCompatibilityLevel()`
- `DefaultSqlScriptGenerator GetSqlGenerator()`
- `TSqlFragment GetSqlFragment(string sql)`

#### `public class SqlFragmentAndGeneratorProvider : ISqlFragmentAndGeneratorProvider`
High-level service providing compatibility-level-aware fragment parsing and SQL script generation.
- **Constructor**:
  - `public SqlFragmentAndGeneratorProvider(ISqlCompatibilityLevelProvider compatibilityLevelProvider, ISqlFragmentProvider sqlFragmentProvider, ISqlGeneratorFactory sqlGeneratorFactory)`
- **Methods**:
  - `public TSqlCompatibilityLevel GetCompatibilityLevel()`
  - `public TSqlFragment GetSqlFragment(string sql)`
  - `public DefaultSqlScriptGenerator GetSqlGenerator()`

---

### 7. Property Value Models

**Namespace**: `Ivtem.TSqlParsing.Model.Properties`

#### `public record PropertyValue`
Represents a property metadata and value pair.
- **Properties**:
  - `public string Name { get; }`
  - `public Type Type { get; }`
  - `public string Caption { get; }`
  - `public string? Value { get; init; }`
- **Constructor**:
  - `public PropertyValue(string name, string? caption = null, Type? type = null, string? value = null)`

#### `public sealed record PropertyValueKey : PropertyValue`
Represents the key property of a record.
- **Constructors**:
  - `public PropertyValueKey(string name, string value, string? caption = null, Type? type = null)`
  - `public PropertyValueKey(string keyValue, PropertyValueSchema schema)`

#### `public record PropertyValueSchema`
Defines the schema (key property, property captions, and property types) for a set of properties.
- **Properties**:
  - `public string KeyPropertyName { get; }`
  - `public string KeyPropertyCaption { get; }`
  - `public Type KeyPropertyType { get; }`
  - `public string? DefaultFormatting { get; init; }`
  - `public Dictionary<string, string> PropertyCaptions { get; }`
  - `public Dictionary<string, Type> PropertyTypes { get; }`
- **Constructor**:
  - `public PropertyValueSchema(PropertyValue key, IEnumerable<PropertyValue> properties)`

#### `public class PropertyValueRow : IEnumerable<PropertyValue>`
Represents a row of property values based on a schema with a key.
- **Properties**:
  - `public PropertyValueKey Key { get; }`
  - `public string KeyPropertyName => Key.Name;`
  - `public string KeyPropertyValue => Key.Value!;`
  - `public string KeyPropertyCaption => Key.Caption;`
  - `public Type KeyPropertyType => Key.Type;`
  - `public Dictionary<string, string> PropertyCaptions => Schema.PropertyCaptions;`
  - `public Dictionary<string, Type> PropertyTypes => Schema.PropertyTypes;`
  - `public string? this[string propertyName] { get; set; }`
- **Methods & Factories**:
  - `public static PropertyValueRow CreateRow(string keyValue, PropertyValueSchema schema, IEnumerable<(string PropertyName, string? PropertyValue)> properties)`
  - `public PropertyValueRow CreateRow(string keyValue, IEnumerable<(string PropertyName, string? PropertyValue)> properties)`
  - `public PropertyValueRow CreateRow(string keyValue, IReadOnlyDictionary<string, string?> properties)`
  - `public void AddOrUpdateRow(PropertyValueRow other)`
  - `public string? GetValue(string propertyName)`
  - `public void SetValue(string propertyName, string? value)`
  - `public IEnumerator<PropertyValue> GetEnumerator()`

#### `public class PropertyValueList : IEnumerable<PropertyValueRow>`
Collection of `PropertyValueRow` items indexed by key.
- **Properties**:
  - `public PropertyValueSchema Schema { get; }`
  - `public string KeyPropertyName => Schema.KeyPropertyName;`
  - `public string KeyPropertyCaption => Schema.KeyPropertyCaption;`
  - `public Type KeyPropertyType => Schema.KeyPropertyType;`
  - `public Dictionary<string, string> PropertyCaptions => Schema.PropertyCaptions;`
  - `public Dictionary<string, Type> PropertyTypes => Schema.PropertyTypes;`
  - `public long RowCount => PropertyValueRows.Count;`
  - `public long PropertiesCount => PropertyCaptions.Count;`
  - `public string? this[string key, string propertyName] { get; set; }`
- **Constructor**:
  - `public PropertyValueList(PropertyValueSchema schema)`
- **Methods**:
  - `public void AddOrUpdate(string keyValue, IEnumerable<(string propertyName, string? propertyValue)> properties)`
  - `public IEnumerator<PropertyValueRow> GetEnumerator()`

---

### 8. Exceptions

**Namespace**: `Ivtem.TSqlParsing.Exceptions`

- **`public sealed class PropertyValueKeyNotFoundException : Exception`**  
  Thrown when a key is not found in a `PropertyValueList`.
  - `public PropertyValueKeyNotFoundException(string key)`
- **`public sealed class PropertyValueKeyIsNullException : Exception`**  
  Thrown when a record with a null key value is encountered.
  - `public PropertyValueKeyIsNullException(string propertyName)`
- **`public sealed class PropertyValueKeyValueChangeAttemptException : Exception`**  
  Thrown when an attempt is made to mutate a read-only key property.
  - `public PropertyValueKeyValueChangeAttemptException(string key)`
- **`public sealed class PropertyValueMissingKeyValueException : Exception`**  
  Thrown when the key property value is missing.
  - `public PropertyValueMissingKeyValueException(string key)`
- **`public sealed class PropertyValuePropertyNotFoundException : Exception`**  
  Thrown when accessing an undefined property name.
  - `public PropertyValuePropertyNotFoundException(string propertyName)`
