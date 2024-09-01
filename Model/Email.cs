namespace ccsflowserver.Model;

public readonly record struct Email(string SenderEmail, string SenderName, string Subject, string Message);