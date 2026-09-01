namespace online_corse.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId
        {
            get
            {
                if (string.IsNullOrEmpty(RequestId))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
