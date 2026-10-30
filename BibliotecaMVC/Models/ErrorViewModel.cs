namespace BibliotecaMVC.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        // Código HTTP que originó la página (404, 500, etc.), si se conoce.
        public int? StatusCode { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
