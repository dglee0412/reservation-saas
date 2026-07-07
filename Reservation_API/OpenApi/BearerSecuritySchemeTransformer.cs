using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Reservation_API.OpenApi;

internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider
) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        // Bearer 인증이 설정돼 있을 때만
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Access Token 입력 (Bearer 접두사 자동)"
            };

            document.Components ??= new OpenApiComponents();
            document.AddComponent("Bearer", bearerScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            };

            // 모든 엔드포인트에 적용
            foreach (var operation in document.Paths.Values.SelectMany(p => p.Operations))
            {
                operation.Value.Security ??= new List<OpenApiSecurityRequirement>();
                operation.Value.Security.Add(securityRequirement);
            }
        }
    }
}