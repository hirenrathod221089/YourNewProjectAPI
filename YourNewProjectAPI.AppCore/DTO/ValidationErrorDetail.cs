namespace YourNewProjectAPI.AppCore.Dto;

// Simply holds the field name and why it failed validation
public record ValidationErrorDetail(string Field, string Message);
