// PoemApp.Admin/Models/DashboardStats.cs
namespace PoemApp.Admin.Models;

public class DashboardStats
{
    public int PoemCount { get; set; }
    public int AuthorCount { get; set; }
    public int UserCount { get; set; }
    public int AudioCount { get; set; }
    public int AnnotationCount { get; set; }
    public int CategoryCount { get; set; }
}