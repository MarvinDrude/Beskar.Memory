using Beskar.Memory.Code.Common.Enums;
using Beskar.Memory.Code.Common.Specs;
using Beskar.Memory.Code.Models.Symbols.Archetypes;
using Beskar.Memory.Code;
using Microsoft.CodeAnalysis;

namespace Beskar.Memory.Code.Common.Archetypes;

public static class NamedTypeSymbolArchetypeExtensions
{
   extension(ref NamedTypeSymbolArchetype archetype)
   {
      public bool IsGuid => archetype.Symbol.IsGuid;

      public string GetGenericParametersString()
      {
         var writer = new CodeTextWriter(stackalloc char[256], stackalloc char[12]);

         try
         {
            var typeParameters = archetype.NamedType.TypeParameters.Array;
            if (typeParameters.Length is 0) return string.Empty;
            
            writer.WriteInterpolated($"<{string.Join(", ", typeParameters.Select(p => p.Symbol.Name))}>");
            return writer.ToString();
         }
         finally
         {
            writer.Dispose();
         }
      }
      
      public string GetClassStructModifiersString(bool addPartial = false)
      {
         var writer = new CodeTextWriter(stackalloc char[256], stackalloc char[12]);

         try
         {
            var visibility = archetype.Symbol.Accessibility.ToKeywordString();
            writer.WriteInterpolated($"{visibility} ");

            if (archetype.Type.Kind is TypeKind.Class)
            {
               if (archetype.Symbol.IsStatic)
               {
                  writer.Write("static ");
               }
               else if (archetype.Symbol.IsAbstract)
               {
                  writer.Write("abstract ");
               }
               else if (archetype.Symbol.IsSealed)
               {
                  writer.Write("sealed ");
               }
               
               if (addPartial)
               {
                  writer.Write("partial ");
               }

               writer.Write(archetype.Type.IsRecord ? "record" : "class");
            }
            else if (archetype.Type.Kind is TypeKind.Struct)
            {
               if (archetype.Type.IsReadOnly)
               {
                  writer.Write("readonly ");
               }

               if (archetype.Type.IsRefLikeType)
               {
                  writer.Write("ref ");
               }

               if (addPartial)
               {
                  writer.Write("partial ");
               }

               if (archetype.Type.IsRecord)
               {
                  writer.Write("record ");
               }
               
               writer.Write("struct");
            }
            else
            {
               throw new InvalidOperationException();
            }
            
            return writer.ToString();
         }
         finally
         {
            writer.Dispose();
         }
      }
   }
}

