namespace TalentCRM.Services;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

public class NotFoundException : Exception
{
    public NotFoundException(string what, int id) : base($"{what} #{id} was not found.") { }
}
