using System;
using ShoppingApp.Common.Models;
using ShoppingApp.Data;
using ShoppingApp.Domain.Services;

namespace ShoppingApp.Domain.Workflows
{
    public class GetProdcutsWorkflow
    {

        private readonly IDbContextFactory dbContextFactory;


        public GetProdcutsWorkflow(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<List<Product>> Execute()
        {
            ProductService service = new(dbContextFactory);
            return await service.GetAllProdcuts();
        }
    }
}

