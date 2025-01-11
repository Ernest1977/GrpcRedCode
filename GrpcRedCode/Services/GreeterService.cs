using Grpc.Core;
using GrpcRedCode;

namespace GrpcRedCode.Services
{
    public class GreeterService : Discount.DiscountBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }


        public override Task<GenerateResponse> GenerateDiscountCodes(GenerateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GenerateResponse
            {
               Count = request.Count
            });
        }
    }
}
