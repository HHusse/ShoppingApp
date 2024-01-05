using System;
using ShoppingApp.Common.Models;
using ShoppingApp.Data;
using ShoppingApp.Domain.Services;

namespace ShoppingApp.Domain.Workflows
{
    public class GetSpecificProductWorkflow
    {
        private readonly IDbContextFactory dbContextFactory;


        public GetSpecificProductWorkflow(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<Product> Execute(string productCode)
        {
            ProductService service = new(dbContextFactory);
            return await service.GetProductByUid(productCode);
        }
    }
}

