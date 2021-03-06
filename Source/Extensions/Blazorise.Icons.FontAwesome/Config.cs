#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Blazorise.Icons.FontAwesome
{
    public static class Config
    {
        public static IServiceCollection AddFontAwesomeIcons( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddSingleton<IIconProvider, FontAwesomeIconProvider>();

            return serviceCollection;
        }

        public static IComponentsApplicationBuilder UseFontAwesomeIcons( this IComponentsApplicationBuilder app )
        {
            var componentMapper = app.Services.GetRequiredService<IComponentMapper>();

            componentMapper.Register<Blazorise.Icon, FontAwesome.Icon>();

            return app;
        }
    }
}
