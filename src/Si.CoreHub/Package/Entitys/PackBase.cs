using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Si.CoreHub.Package.Abstraction;

namespace Si.CoreHub.Package.Entitys
{
    public abstract class PackBase
    {
        public abstract void ConfigurationServices(WebApplicationBuilder builder,IServiceCollection services);

        public abstract void Configuration(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider);
    }
}
