public class TableInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsSelected { get; set; } = true;
    public int RowCount { get; set; } = 0;

    public TableInfo() { }

    public TableInfo(string name, string displayName, bool isSelected = true)
    {
        Name = name;
        DisplayName = displayName;
        IsSelected = isSelected;
    }
}