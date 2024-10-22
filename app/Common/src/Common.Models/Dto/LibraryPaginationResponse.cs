using System.Runtime.Serialization;

namespace Common.Models.DTO;

public class LibraryPaginationResponse
{
    /// <summary>
    /// Номер страницы
    /// </summary>
    /// <value>Номер страницы</value>
    [DataMember(Name="page")]
    public int? Page { get; set; }

    /// <summary>
    /// Количество элементов на странице
    /// </summary>
    /// <value>Количество элементов на странице</value>
    [DataMember(Name="pageSize")]
    public int? PageSize { get; set; }

    /// <summary>
    /// Общее количество элементов
    /// </summary>
    /// <value>Общее количество элементов</value>
    [DataMember(Name="totalElements")]
    public int TotalElements { get; set; }

    /// <summary>
    /// Gets or Sets Items
    /// </summary>
    [DataMember(Name="items")]
    public List<LibraryResponse> Items { get; set; }
}