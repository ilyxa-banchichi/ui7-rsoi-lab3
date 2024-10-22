namespace LibraryService.Common.Models;

public class Library
{
    public int Id { get; set; }
    public Guid LibraryUid { get; set; } // Для uuid используем Guid
    public string Name { get; set; }
    public string City { get; set; }
    public string Address { get; set; }

    // Связь один-ко-многим с таблицей LibraryBooks
    public List<LibraryBooks> LibraryBooks { get; set; }
}