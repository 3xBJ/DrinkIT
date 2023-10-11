namespace DrinkIT.Domain.BaseClasses
{
    public class ObjectWithInternalValidation
    {
        private readonly List<Error> validationErrors = new();

        public IReadOnlyList<Error> ValidationErrors => this.validationErrors;
        public bool HasErrors => this.validationErrors.Count > 0;

        public string ErrorMessage
        {
            get
            {
                IEnumerable<string> errors = ValidationErrors.Select(error => error.Message);
                return errors.Any() ? errors.Aggregate((message1, message2) => $"{message1}, {message2}") :
                                      string.Empty;
            }
        }
                                          

        protected void AddError(string message) => this.validationErrors.Add(new Error(message));
        protected void AddError(string message, Exception ex) => this.validationErrors.Add(new Error(message, ex));
    }
}
