using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Common.Models.DTO;

public class UserRatingResponse
{
    /// <summary>
    /// Количество здесь у пользователя
    /// </summary>
    /// <value>Количество здесь у пользователя</value>
    [Range(0, 100)]
    [DataMember(Name="stars")]
    public int Stars { get; set; }
}