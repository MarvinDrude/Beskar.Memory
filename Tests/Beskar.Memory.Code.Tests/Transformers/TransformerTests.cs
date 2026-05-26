using System;
using System.Linq;
using Xunit;
using Microsoft.CodeAnalysis;
using Beskar.Memory.Code.Models.Symbols;
using Beskar.Memory.Code.Transformers.Symbols;
using Beskar.Memory.Code.Transformers.Archetypes;
using Beskar.Memory.Code.Transformers.Archetypes.Options;
using Beskar.Memory.Code.Transformers.Symbols.Options;
using Beskar.Memory.Code.Tests.Helpers;

namespace Beskar.Memory.Code.Tests.Transformers
{
   public class TransformerTests
   {
      private const string Source = @"
using System;
using System.Threading.Tasks;

public interface ICustomInterface {}

public class ComplexClass<T> where T : class, ICustomInterface, new()
{
   public const int MaxCount = 100;
   public string Title { get; private set; }
   
   public Task<int> ProcessAsync(ref int value, string label = ""default"")
   {
      return Task.FromResult(MaxCount);
   }
}
";

      [Fact]
      public void TransformField_ConstantField_ExtractsCorrectMetadata()
      {
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(Source, "ComplexClass`1");
         var fieldSymbol = classSymbol.GetMembers("MaxCount").OfType<IFieldSymbol>().First();
         var options = new ArchetypeTransformOptions
         {
            Fields = FieldTransformOptions.Full
         };

         var fieldArchetype = FieldSymbolArchetypeTransformer.Transform(fieldSymbol, options: options);

         Assert.Equal("MaxCount", fieldArchetype.Symbol.Name);
         Assert.Equal(Accessibility.Public, fieldArchetype.Symbol.Accessibility);
         Assert.True(fieldArchetype.Symbol.IsStatic);
         Assert.True(fieldArchetype.Field.IsConst);
         Assert.Equal("Int32", fieldArchetype.Field.Type.Symbol.Name);
      }

      [Fact]
      public void TransformProperty_PrivateSetter_ExtractsCorrectAccessibility()
      {
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(Source, "ComplexClass`1");
         var propertySymbol = classSymbol.GetMembers("Title").OfType<IPropertySymbol>().First();
         var options = new ArchetypeTransformOptions
         {
            Properties = PropertyTransformOptions.Full
         };

         var propertyArchetype = PropertySymbolArchetypeTransformer.Transform(propertySymbol, options: options);

         Assert.Equal("Title", propertyArchetype.Symbol.Name);
         Assert.True(propertyArchetype.Property.HasGetter);
         Assert.True(propertyArchetype.Property.HasSetter);
         Assert.Equal(Accessibility.Public, propertyArchetype.Symbol.Accessibility);
         
         Assert.NotNull(propertyArchetype.Property.Setter);
         Assert.Equal(Accessibility.Private, propertyArchetype.Property.Setter.Value.Symbol.Accessibility);
      }

      [Fact]
      public void TransformMethodAndParameters_AsyncWithRefAndDefault_ExtractsCorrectMetadata()
      {
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(Source, "ComplexClass`1");
         var methodSymbol = classSymbol.GetMembers("ProcessAsync").OfType<IMethodSymbol>().First();
         var options = new ArchetypeTransformOptions
         {
            Methods = MethodTransformOptions.Full,
            Parameters = ParameterTransformOptions.Full
         };

         var methodArchetype = MethodSymbolArchetypeTransformer.Transform(methodSymbol, options: options);

         Assert.Equal("ProcessAsync", methodArchetype.Symbol.Name);
         Assert.False(methodArchetype.Method.IsAsync);
         Assert.Equal("Task", methodArchetype.Method.ReturnType.Value.Symbol.Name);

         // Validate parameters
         Assert.Equal(2, methodArchetype.Method.Parameters.Count);

         // Ref parameter
         var refParam = methodArchetype.Method.Parameters[0];
         Assert.Equal("value", refParam.Symbol.Name);
         Assert.Equal(RefKind.Ref, refParam.Parameter.RefKind);
         Assert.False(refParam.Parameter.IsOptional);

         // Optional parameter with default value
         var optParam = methodArchetype.Method.Parameters[1];
         Assert.Equal("label", optParam.Symbol.Name);
         Assert.Equal(RefKind.None, optParam.Parameter.RefKind);
         Assert.True(optParam.Parameter.IsOptional);
         Assert.True(optParam.Parameter.HasExplicitDefaultValue);
      }

      [Fact]
      public void TransformTypeParameter_ConstraintsLoaded_ExtractsConstraints()
      {
         var classSymbol = TestCompilationHelper.GetNamedTypeSymbol(Source, "ComplexClass`1");
         var typeParamSymbol = classSymbol.TypeParameters.First();
         var options = new ArchetypeTransformOptions
         {
            TypeParameters = TypeParameterTransformOptions.Full
         };

         var typeParamArchetype = TypeParameterSymbolArchetypeTransformer.Transform(typeParamSymbol, options: options);

         Assert.Equal("T", typeParamArchetype.Symbol.Name);
         Assert.True(typeParamArchetype.TypeParameter.HasConstructorConstraint); // new()
         Assert.True(typeParamArchetype.TypeParameter.HasReferenceTypeConstraint); // class

         // Interface constraint
         Assert.Single(typeParamArchetype.TypeParameter.ConstraintTypes);
         Assert.Equal("ICustomInterface", typeParamArchetype.TypeParameter.ConstraintTypes[0].Symbol.Name);
      }
   }
}
