using System;
namespace ShoppingApp.Domain.ResponseModels
{
	public class GeneralWorkflowResponse
	{
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public GeneralWorkflowResponse()
        {
            this.Success = false;
            this.Message = "";
            this.StatusCode = 500;
        }

        public GeneralWorkflowResponse(bool success, string message, int statusCode)
        {
            this.Success = success;
            this.Message = message;
            this.StatusCode = statusCode;
        }
    }
}

