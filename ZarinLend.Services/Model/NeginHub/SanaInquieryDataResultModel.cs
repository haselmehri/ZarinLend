using Microsoft.Extensions.Logging;
using System;

namespace Services.Model.NeginHub
{
    public class SanaInquieryDataResultModel : NeginHubBaseResult
    {
        private readonly ILogger<SanaInquieryDataResultModel> logger;

        public SanaInquieryDataResultModel()
        {

        }

        public SanaInquieryDataResultModel(ILogger<SanaInquieryDataResultModel> logger)
        {
            this.logger = logger;
        }

        // Data section
        public bool Registered { get; set; }
        public string Message { get; set; }
        public string TrackId { get; set; }

        // Meta section
        public string Code { get; set; }
        public string? MetaMessage { get; set; }
        public object? Errors { get; set; }

        public string FormattedErrorMessage
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(ErrorMessage))
                    {
                        return ErrorMessage;
                    }
                    else if (Errors != null)
                    {
                        return Errors.ToString();
                    }
                }
                catch (Exception exp)
                {
                    logger.LogError(exp, "Error formatting error message");
                }

                return "No error message available.";
            }
        }

        public void MapFromApiResponse(dynamic resultObject)
        {
            try
            {
                if (resultObject?.data != null)
                {
                    Registered = resultObject.data.registered;
                    Message = resultObject.data.message;
                    TrackId = resultObject.data.trackId;
                }

                if (resultObject?.meta != null)
                {
                    IsSuccess = resultObject.meta.isSuccess;
                    Code = resultObject.meta.code;
                    ErrorMessage = resultObject.meta.errorMessage;
                    MetaMessage = resultObject.meta.message;
                    Errors = resultObject.meta.errors;
                }
            }
            catch (Exception exp)
            {
                logger.LogError(exp, "Error mapping SanaInquieryDataResultModel from API response.");
            }
        }
    }
}
