using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Common.Models.Enums;

namespace Common.Models.DTO;

public class LibraryBookResponse
{
    /// <summary>
    /// UUID книги
    /// </summary>
    /// <value>UUID книги</value>
    [DataMember(Name="bookUid")]
    public Guid BookUid { get; set; }

    /// <summary>
    /// Название книги
    /// </summary>
    /// <value>Название книги</value>
    [DataMember(Name="name")]
    public string Name { get; set; }

    /// <summary>
    /// Автор
    /// </summary>
    /// <value>Автор</value>
    [DataMember(Name="author")]
    public string Author { get; set; }

    /// <summary>
    /// Жанр
    /// </summary>
    /// <value>Жанр</value>
    [DataMember(Name="genre")]
    public string Genre { get; set; }

    /// <summary>
    /// Состояние книги
    /// </summary>
    /// <value>Состояние книги</value>
    [DataMember(Name="condition")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BookCondition Condition { get; set; }

    /// <summary>
    /// Количество книг, доступных для аренды в библиотеке
    /// </summary>
    /// <value>Количество книг, доступных для аренды в библиотеке</value>
    [DataMember(Name="availableCount")]
    public int AvailableCount { get; set; }
}