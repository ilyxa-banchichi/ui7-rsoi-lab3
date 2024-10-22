using System.Runtime.Serialization;
using Common.Models.Enums;

namespace Common.Models.DTO;

[DataContract]
public class UpdateBookConditionResponse
{
    /// <summary>
    /// UUID книги
    /// </summary>
    /// <value>UUID книги</value>
    [DataMember(Name="bookUid")]
    public Guid BookUid { get; set; }

    /// <summary>
    /// UUID библиотеки
    /// </summary>
    /// <value>UUID библиотеки</value>
    [DataMember(Name="libraryUid")]
    public Guid LibraryUid { get; set; }

    public BookCondition OldCondition  { get; set; }
    public BookCondition NewCondition  { get; set; }
}