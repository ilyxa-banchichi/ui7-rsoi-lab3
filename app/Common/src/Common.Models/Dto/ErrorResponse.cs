using System.Runtime.Serialization;

namespace Common.Models.DTO;

[DataContract]
public class ErrorResponse(string message)
{
    /// <summary>
    /// Информация об ошибке
    /// </summary>
    /// <value>Информация об ошибке</value>
    [DataMember(Name = "message")]
    public string Message { get; set; } = message;
}