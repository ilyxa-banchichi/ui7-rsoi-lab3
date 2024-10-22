using System.Runtime.Serialization;
using Common.Models.Enums;

namespace Gateway.Common.Models.DTO;

[DataContract]
public class ReturnBookRequest
{
    /// <summary>
    /// Состояние книги
    /// </summary>
    /// <value>Состояние книги</value>
    [DataMember(Name="condition")]
    public BookCondition Condition { get; set; }

    /// <summary>
    /// Дата возврата
    /// </summary>
    /// <value>Дата возврата</value>
    [DataMember(Name="date")]
    public DateOnly Date { get; set; }
}