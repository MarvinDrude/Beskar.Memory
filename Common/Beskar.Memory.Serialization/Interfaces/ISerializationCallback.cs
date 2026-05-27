namespace Beskar.Memory.Serialization.Interfaces;

/// <summary>
/// Allows a class or struct to perform custom logic before serialization and after deserialization.
/// </summary>
public interface ISerializationCallback
{
   /// <summary>
   /// Called right before serialization begins.
   /// </summary>
   public void OnBeforeSerialize();

   /// <summary>
   /// Called right after deserialization has successfully completed.
   /// </summary>
   public void OnAfterDeserialize();
}
