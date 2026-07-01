using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MixAndMatch.Api.Common;
using MixAndMatch.Api.Configuration;
using MixAndMatch.Application.UseCases.Pago.Commands;
using MixAndMatch.Application.UseCases.Pago.Queries;
using MixAndMatch.Domain.Common;

namespace MixAndMatch.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
public class PagoController(IMediator _mediator) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        return this.ToActionResult(await _mediator.Send(new GetAllPagosQuery { Page = page, PageSize = pageSize }));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{nameof(RolUsuario.ADMIN)},{nameof(RolUsuario.CLIENTE)}")]
    public async Task<IActionResult> GetById(long id)
    {
        return this.ToActionResult(await _mediator.Send(new GetPagoByIdQuery
        {
            Id = id,
            SolicitanteId = CurrentUser.Id,
            EsAdmin = CurrentUser.IsAdmin
        }));
    }

    [HttpPost]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> Create([FromBody] CreatePagoCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePagoCommand command)
    {
        command.Id = id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(RolUsuario.ADMIN))]
    public async Task<IActionResult> Delete(long id)
    {
        return this.ToActionResult(await _mediator.Send(new DeletePagoCommand { Id = id }));
    }

    [HttpPost("mercadopago")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> CreateMercadoPago([FromBody] CreatePagoMercadoPagoCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPost("webhook/mercadopago")]
    [AllowAnonymous]
    public async Task<IActionResult> WebhookMercadoPago(
        [FromHeader(Name = "x-signature")] string xSignature,
        [FromHeader(Name = "x-request-id")] string xRequestId,
        [FromQuery(Name = "data.id")] string dataId)
    {
        var result = await _mediator.Send(new ProcesarWebhookMercadoPagoCommand
        {
            XSignature = xSignature,
            XRequestId = xRequestId,
            DataId = dataId
        });
        return this.ToActionResult(result);
    }

    [HttpGet("paypal/client-token")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> PayPalClientToken()
    {
        return this.ToActionResult(await _mediator.Send(new ObtenerPayPalClientTokenQuery()));
    }

    [HttpPost("paypal")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> CreatePayPal([FromBody] CreatePagoPayPalCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPost("paypal/capturar")]
    [Authorize(Roles = nameof(RolUsuario.CLIENTE))]
    public async Task<IActionResult> CapturarPayPal([FromBody] CapturarPagoPayPalCommand command)
    {
        command.SolicitanteId = CurrentUser.Id;
        return this.ToActionResult(await _mediator.Send(command));
    }

    [HttpPost("webhook/paypal")]
    [AllowAnonymous]
    public async Task<IActionResult> WebhookPayPal(
        [FromBody] JsonElement body,
        [FromHeader(Name = "Paypal-Transmission-Id")] string transmissionId,
        [FromHeader(Name = "Paypal-Transmission-Time")] string transmissionTime,
        [FromHeader(Name = "Paypal-Cert-Url")] string certUrl,
        [FromHeader(Name = "Paypal-Auth-Algo")] string authAlgo,
        [FromHeader(Name = "Paypal-Transmission-Sig")] string transmissionSig)
    {
        var result = await _mediator.Send(new ProcesarWebhookPayPalCommand
        {
            TransmissionId = transmissionId,
            TransmissionTime = transmissionTime,
            CertUrl = certUrl,
            AuthAlgo = authAlgo,
            TransmissionSig = transmissionSig,
            EventBody = body.GetRawText()
        });
        return this.ToActionResult(result);
    }
}
