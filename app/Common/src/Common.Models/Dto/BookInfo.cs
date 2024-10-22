using System.Runtime.Serialization;

namespace Common.Models.DTO;

public class BookInfo
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
}