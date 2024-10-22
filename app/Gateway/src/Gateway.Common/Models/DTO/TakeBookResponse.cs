using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Common.Models.DTO;
using Common.Models.Enums;
using Common.Models.Serialization;

namespace Gateway.Common.Models.DTO;

[DataContract]
public class TakeBookResponse
{
    /// <summary>
    /// UUID бронирования
    /// </summary>
    /// <value>UUID бронирования</value>
    [DataMember(Name="reservationUid")]
    public Guid ReservationUid { get; set; }

    /// <summary>
    /// Статус бронирования книги
    /// </summary>
    /// <value>Статус бронирования книги</value>
    [DataMember(Name="status")]
    public ReservationStatus Status { get; set; }

    /// <summary>
    /// Дата начала бронирования
    /// </summary>
    /// <value>Дата начала бронирования</value>
    [DataMember(Name="startDate")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Дата окончания бронирования
    /// </summary>
    /// <value>Дата окончания бронирования</value>
    [DataMember(Name="tillDate")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly TillDate { get; set; }

    /// <summary>
    /// Gets or Sets Book
    /// </summary>
    [DataMember(Name="book")]
    public BookInfo Book { get; set; }

    /// <summary>
    /// Gets or Sets Library
    /// </summary>
    [DataMember(Name="library")]
    public LibraryResponse Library { get; set; }

    /// <summary>
    /// Gets or Sets Rating
    /// </summary>
    [DataMember(Name="rating")]
    public UserRatingResponse Rating { get; set; }
}