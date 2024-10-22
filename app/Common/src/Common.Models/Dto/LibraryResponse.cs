using System.Runtime.Serialization;

namespace Common.Models.DTO;

public class LibraryResponse
{
    /// <summary>
    /// UUID библиотеки
    /// </summary>
    /// <value>UUID библиотеки</value>
    [DataMember(Name="libraryUid")]
    public string LibraryUid { get; set; }

    /// <summary>
    /// Название библиотеки
    /// </summary>
    /// <value>Название библиотеки</value>
    [DataMember(Name="name")]
    public string Name { get; set; }

    /// <summary>
    /// Адрес библиотеки
    /// </summary>
    /// <value>Адрес библиотеки</value>
    [DataMember(Name="address")]
    public string Address { get; set; }

    /// <summary>
    /// Город, в котором находится библиотека
    /// </summary>
    /// <value>Город, в котором находится библиотека</value>
    [DataMember(Name="city")]
    public string City { get; set; }
}