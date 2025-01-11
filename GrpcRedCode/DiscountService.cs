using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcRedCode
{
    //using Grpc.Core;
    //using System;
    //using System.Collections.Generic;
    //using System.IO;
    //using System.Linq;
    //using System.Threading.Tasks;

    public class DiscountService : Discount.DiscountBase
    {
        private static readonly string storagePath = "discount_codes.txt";
        private static readonly object lockObj = new object();

        public override Task<GenerateResponse> GenerateDiscountCodes(GenerateRequest request, ServerCallContext context)
        {
            var codes = new List<string>();
            var random = new Random();

            lock (lockObj)
            {
                var existingCodes = File.Exists(storagePath) ? File.ReadAllLines(storagePath).ToHashSet() : new HashSet<string>();

                for (int i = 0; i < request.Count && existingCodes.Count < 2000; i++)
                {
                    string code;
                    do
                    {
                        code = GenerateRandomCode(random);
                    } while (existingCodes.Contains(code));

                    existingCodes.Add(code);
                    codes.Add(code);
                }

                File.WriteAllLines(storagePath, existingCodes);
            }

            return Task.FromResult(new GenerateResponse { Codes = { codes } });
        }

        public override Task<UseResponse> UseDiscountCode(UseRequest request, ServerCallContext context)
        {
            bool success = false;

            lock (lockObj)
            {
                if (File.Exists(storagePath))
                {
                    var codes = File.ReadAllLines(storagePath).ToList();
                    if (codes.Remove(request.Code))
                    {
                        success = true;
                        File.WriteAllLines(storagePath, codes);
                    }
                }
            }

            return Task.FromResult(new UseResponse { Success = success });
        }

        private string GenerateRandomCode(Random random)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 7).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
