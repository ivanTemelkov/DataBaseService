using System.Diagnostics.CodeAnalysis;

namespace Ivtem.TSqlParsing.Feature.CompatibilityLevel;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum TSqlCompatibilityLevel
{
    TSql80,
    TSql90,
    TSql100,
    TSql110,
    TSql120,
    TSql130,
    TSql140,
    TSql150,
    TSql160
}