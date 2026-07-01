using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using MixAndMatch.Domain.Common;
using MixAndMatch.Domain.Entities;
using MixAndMatch.Domain.Ports.IServices;
using MixAndMatch.Infrastructure.Configuration;

namespace MixAndMatch.Infrastructure.Adapters.Services;

public class ReporteExcelService(MixAndMatchDbContext context) : IReporteService
{
    // ── Paleta de colores ──────────────────────────────────────────────────────
    private static readonly XLColor CNavy    = XLColor.FromHtml("#1B3A6B");
    private static readonly XLColor CAzul    = XLColor.FromHtml("#2D6FBF");
    private static readonly XLColor CHeader  = XLColor.FromHtml("#3B82F6");
    private static readonly XLColor CFila    = XLColor.FromHtml("#EFF6FF");
    private static readonly XLColor CVerde   = XLColor.FromHtml("#059669");
    private static readonly XLColor CBorde   = XLColor.FromHtml("#93C5FD");
    private static readonly XLColor CRojo    = XLColor.FromHtml("#FEE2E2");
    private static readonly XLColor CAmarillo= XLColor.FromHtml("#FEF3C7");

    private static readonly XLColor CTRojo   = XLColor.FromHtml("#991B1B");
    private static readonly XLColor CTBrown  = XLColor.FromHtml("#92400E");
    private static readonly XLColor CTVerde  = XLColor.FromHtml("#065F46");

    // ═══════════════════════════════════════════════════════════════════════════
    //  REPORTE DE STOCK
    // ═══════════════════════════════════════════════════════════════════════════
    public async Task<byte[]> GenerarReporteStock(string? nombre, string? genero, string? categoria)
    {
        var query = context.Set<PrendaTalla>()
            .AsNoTracking()
            .Include(pt => pt.Prenda).ThenInclude(p => p.Categoria)
            .Include(pt => pt.Prenda).ThenInclude(p => p.Genero)
            .Include(pt => pt.Prenda).ThenInclude(p => p.Marca)
            .Include(pt => pt.Talla)
            .Where(pt => pt.Prenda.Activo);

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(pt => EF.Functions.ILike(pt.Prenda.Nombre, $"%{nombre}%"));
        if (!string.IsNullOrWhiteSpace(genero))
            query = query.Where(pt => EF.Functions.ILike(pt.Prenda.Genero.NomGenero, $"%{genero}%"));
        if (!string.IsNullOrWhiteSpace(categoria))
            query = query.Where(pt => EF.Functions.ILike(pt.Prenda.Categoria.NomCategoria, $"%{categoria}%"));

        var data = await query
            .OrderBy(pt => pt.Prenda.Categoria.NomCategoria)
            .ThenBy(pt => pt.Prenda.Nombre)
            .ThenBy(pt => pt.Talla.NomTalla)
            .ToListAsync();

        using var wb = new XLWorkbook();
        HojaDetalleStock(wb, data, nombre, genero, categoria);
        HojaResumenStock(wb, data);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private void HojaDetalleStock(XLWorkbook wb, List<PrendaTalla> data,
        string? nombre, string? genero, string? categoria)
    {
        var ws = wb.Worksheets.Add("Detalle de Stock");

        // Título
        MergeYEstilar(ws, "A1:H1", "REPORTE DE STOCK — MIX AND MATCH", CNavy, XLColor.White, 14, 28);

        var filtros = FiltrosTexto(nombre, genero, categoria);
        MergeYEstilar(ws, "A2:H2",
            $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}   |   Filtros: {filtros}   |   Total registros: {data.Count}",
            CAzul, XLColor.White, 10, 20);

        ws.Row(3).Height = 8;

        // Cabecera de tabla
        string[] cols  = { "#", "Nombre Prenda", "Categoría", "Género", "Marca", "Talla", "Stock", "Estado" };
        int[]    anchos = {  4,   32,              18,           14,       18,       10,       10,       16 };
        int hRow = 4;

        for (int i = 0; i < cols.Length; i++)
        {
            var c = ws.Cell(hRow, i + 1);
            c.Value = cols[i];
            EstilarHeader(c);
            ws.Column(i + 1).Width = anchos[i];
        }
        ws.Row(hRow).Height = 22;

        // Filas de datos
        for (int i = 0; i < data.Count; i++)
        {
            var pt  = data[i];
            int row = hRow + 1 + i;

            // Color de fila según stock
            var bg = pt.Stock == 0 ? CRojo
                   : pt.Stock < 5  ? CAmarillo
                   : i % 2 == 0    ? CFila
                   :                  XLColor.White;

            ws.Cell(row, 1).Value = i + 1;
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 2).Value = pt.Prenda.Nombre;
            ws.Cell(row, 3).Value = pt.Prenda.Categoria.NomCategoria;
            ws.Cell(row, 4).Value = pt.Prenda.Genero.NomGenero;
            ws.Cell(row, 5).Value = pt.Prenda.Marca.NomMarca;
            ws.Cell(row, 6).Value = pt.Talla.NomTalla;
            ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var cStock = ws.Cell(row, 7);
            cStock.Value = pt.Stock;
            cStock.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cStock.Style.Font.Bold = pt.Stock < 5;

            var cEst = ws.Cell(row, 8);
            cEst.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cEst.Style.Font.Bold = true;
            if (pt.Stock == 0)
            {
                cEst.Value = "SIN STOCK";
                cEst.Style.Font.FontColor = CTRojo;
            }
            else if (pt.Stock < 5)
            {
                cEst.Value = "STOCK BAJO";
                cEst.Style.Font.FontColor = CTBrown;
            }
            else
            {
                cEst.Value = "DISPONIBLE";
                cEst.Style.Font.FontColor = CTVerde;
                cEst.Style.Font.Bold = false;
            }

            var rng = ws.Range(row, 1, row, 8);
            rng.Style.Fill.BackgroundColor = bg;
            rng.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rng.Style.Border.BottomBorderColor = CBorde;
        }

        // Leyenda
        int legRow = hRow + data.Count + 2;
        ws.Range(legRow, 1, legRow, 8).Merge();
        var leg = ws.Cell(legRow, 1);
        leg.Value = "Leyenda:   Rojo = Sin stock (0)     Amarillo = Stock bajo (< 5)     Verde claro = Disponible (>= 5)";
        leg.Style.Font.Italic = true;
        leg.Style.Font.FontSize = 9;
        leg.Style.Font.FontColor = XLColor.DimGray;
    }

    private void HojaResumenStock(XLWorkbook wb, List<PrendaTalla> data)
    {
        var ws = wb.Worksheets.Add("Resumen");
        ws.Column(1).Width = 26;
        ws.Column(2).Width = 16;
        ws.Column(3).Width = 26;
        ws.Column(4).Width = 16;

        MergeYEstilar(ws, "A1:D1", "RESUMEN DE STOCK", CNavy, XLColor.White, 13, 25);

        // KPIs
        int row = 3;
        var kpis = new[]
        {
            ("Total SKUs (tallas)",     data.Count.ToString()),
            ("Total unidades en stock", data.Sum(x => x.Stock).ToString("N0")),
            ("SKUs sin stock",          data.Count(x => x.Stock == 0).ToString()),
            ("SKUs con stock bajo",     data.Count(x => x.Stock > 0 && x.Stock < 5).ToString()),
        };
        foreach (var (label, val) in kpis)
        {
            var cL = ws.Cell(row, 1);
            cL.Value = label;
            cL.Style.Fill.BackgroundColor = CVerde;
            cL.Style.Font.FontColor = XLColor.White;
            cL.Style.Font.Bold = true;
            cL.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            cL.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var cV = ws.Cell(row, 2);
            cV.Value = val;
            cV.Style.Font.Bold = true;
            cV.Style.Font.FontSize = 12;
            cV.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cV.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            row++;
        }

        // ── Por Categoría ──────────────────────────────────────────────────────
        row += 2;
        MergeYEstilar(ws, $"A{row}:D{row}", "STOCK POR CATEGORÍA", CAzul, XLColor.White, 11, 20);
        row++;

        string[] hCat = { "Categoría", "Unidades", "Nº SKUs", "Sin Stock" };
        for (int i = 0; i < hCat.Length; i++) EstilarHeader(ws.Cell(row, i + 1), hCat[i]);
        row++;

        var porCat = data
            .GroupBy(pt => pt.Prenda.Categoria.NomCategoria)
            .OrderByDescending(g => g.Sum(x => x.Stock))
            .ToList();

        for (int i = 0; i < porCat.Count; i++)
        {
            var g = porCat[i];
            FilaTabla(ws, row, i, g.Key, g.Sum(x => x.Stock), g.Count(), g.Count(x => x.Stock == 0));
            row++;
        }
        FilaTotales(ws, row, 4, "TOTAL",
            data.Sum(x => x.Stock), data.Count, data.Count(x => x.Stock == 0));
        row += 3;

        // ── Por Género ─────────────────────────────────────────────────────────
        MergeYEstilar(ws, $"A{row}:D{row}", "STOCK POR GÉNERO", CAzul, XLColor.White, 11, 20);
        row++;

        string[] hGen = { "Género", "Unidades", "Nº SKUs", "Sin Stock" };
        for (int i = 0; i < hGen.Length; i++) EstilarHeader(ws.Cell(row, i + 1), hGen[i]);
        row++;

        var porGen = data
            .GroupBy(pt => pt.Prenda.Genero.NomGenero)
            .OrderByDescending(g => g.Sum(x => x.Stock))
            .ToList();

        for (int i = 0; i < porGen.Count; i++)
        {
            var g = porGen[i];
            FilaTabla(ws, row, i, g.Key, g.Sum(x => x.Stock), g.Count(), g.Count(x => x.Stock == 0));
            row++;
        }
        FilaTotales(ws, row, 4, "TOTAL",
            data.Sum(x => x.Stock), data.Count, data.Count(x => x.Stock == 0));
    }

    // ═══════════════════════════════════════════════════════════════════════════
    //  REPORTE DE VENTAS
    // ═══════════════════════════════════════════════════════════════════════════
    public async Task<byte[]> GenerarReporteVentas(string periodo)
    {
        var ahora  = DateTime.UtcNow;
        var cutoff = periodo.ToLowerInvariant() switch
        {
            "mensual"    => ahora.AddMonths(-12),
            "trimestral" => ahora.AddMonths(-12),
            _            => ahora.AddDays(-30),
        };

        var ventas = await context.Set<Venta>()
            .AsNoTracking()
            .Include(v => v.Usuario)
            .Include(v => v.VentasDetalles)
            .Where(v => (v.Estado == EstadoVenta.PAGADO
                      || v.Estado == EstadoVenta.ENVIADO
                      || v.Estado == EstadoVenta.ENTREGADO)
                     && v.FechaCreacion >= cutoff)
            .OrderBy(v => v.FechaCreacion)
            .ToListAsync();

        using var wb = new XLWorkbook();
        HojaDetalleVentas(wb, ventas, periodo);
        HojaResumenVentas(wb, ventas, periodo);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    private void HojaDetalleVentas(XLWorkbook wb, List<Venta> ventas, string periodo)
    {
        var ws = wb.Worksheets.Add("Detalle de Ventas");

        MergeYEstilar(ws, "A1:G1", "REPORTE DE VENTAS — MIX AND MATCH", CNavy, XLColor.White, 14, 28);
        MergeYEstilar(ws, "A2:G2",
            $"Período: {periodo.ToUpperInvariant()}   |   Generado: {DateTime.Now:dd/MM/yyyy HH:mm}   |   Total ventas: {ventas.Count}",
            CAzul, XLColor.White, 10, 20);
        ws.Row(3).Height = 8;

        string[] cols  = { "#", "Fecha", "N° Venta", "Cliente", "Items", "Estado", "Total (S/.)" };
        int[]    anchos = {  4,   18,       10,          24,        8,      14,        14 };
        int hRow = 4;

        for (int i = 0; i < cols.Length; i++)
        {
            EstilarHeader(ws.Cell(hRow, i + 1), cols[i]);
            ws.Column(i + 1).Width = anchos[i];
        }
        ws.Row(hRow).Height = 22;

        for (int i = 0; i < ventas.Count; i++)
        {
            var v   = ventas[i];
            int row = hRow + 1 + i;
            decimal total = v.VentasDetalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            int items     = v.VentasDetalles.Sum(d => d.Cantidad);

            ws.Cell(row, 1).Value = i + 1;
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 2).Value = v.FechaCreacion.ToLocalTime().ToString("dd/MM/yyyy");

            ws.Cell(row, 3).Value = v.Id;
            ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row, 4).Value = v.Usuario.NombreUsuario;

            ws.Cell(row, 5).Value = items;
            ws.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var cEst = ws.Cell(row, 6);
            cEst.Value = v.Estado?.ToString() ?? "-";
            cEst.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cEst.Style.Font.Bold = true;
            cEst.Style.Font.FontColor = v.Estado switch
            {
                EstadoVenta.ENTREGADO => CTVerde,
                EstadoVenta.ENVIADO   => XLColor.FromHtml("#1D4ED8"),
                _                     => XLColor.FromHtml("#1A1A2E"),
            };

            var cTot = ws.Cell(row, 7);
            cTot.Value = total;
            cTot.Style.NumberFormat.Format = "#,##0.00";
            cTot.Style.Font.Bold = true;
            cTot.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            var rng = ws.Range(row, 1, row, 7);
            rng.Style.Fill.BackgroundColor = i % 2 == 0 ? CFila : XLColor.White;
            rng.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            rng.Style.Border.BottomBorderColor = CBorde;
        }

        // Fila total general
        if (ventas.Count > 0)
        {
            int totRow = hRow + ventas.Count + 1;
            ws.Range(totRow, 1, totRow, 6).Merge();
            var cL = ws.Cell(totRow, 1);
            cL.Value = "TOTAL GENERAL";
            cL.Style.Fill.BackgroundColor = CNavy;
            cL.Style.Font.FontColor = XLColor.White;
            cL.Style.Font.Bold = true;
            cL.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            var cT = ws.Cell(totRow, 7);
            cT.Value = ventas.Sum(v => v.VentasDetalles.Sum(d => d.Cantidad * d.PrecioUnitario));
            cT.Style.NumberFormat.Format = "#,##0.00";
            cT.Style.Fill.BackgroundColor = CNavy;
            cT.Style.Font.FontColor = XLColor.White;
            cT.Style.Font.Bold = true;
            cT.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }
    }

    private void HojaResumenVentas(XLWorkbook wb, List<Venta> ventas, string periodo)
    {
        var ws = wb.Worksheets.Add("Resumen por Período");
        ws.Column(1).Width = 20;
        ws.Column(2).Width = 16;
        ws.Column(3).Width = 18;

        MergeYEstilar(ws, "A1:C1", $"RESUMEN — {periodo.ToUpperInvariant()}", CNavy, XLColor.White, 13, 25);

        decimal totalMonto = ventas.Sum(v => v.VentasDetalles.Sum(d => d.Cantidad * d.PrecioUnitario));

        int row = 3;
        var kpis = new[]
        {
            ("Total de ventas",    ventas.Count.ToString("N0")),
            ("Monto total (S/.)",  totalMonto.ToString("#,##0.00")),
            ("Ticket promedio",    ventas.Count > 0
                                    ? (totalMonto / ventas.Count).ToString("#,##0.00")
                                    : "0.00"),
        };
        foreach (var (label, val) in kpis)
        {
            var cL = ws.Cell(row, 1);
            cL.Value = label;
            cL.Style.Fill.BackgroundColor = CVerde;
            cL.Style.Font.FontColor = XLColor.White;
            cL.Style.Font.Bold = true;
            cL.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            cL.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            var cV = ws.Cell(row, 2);
            cV.Value = val;
            cV.Style.Font.Bold = true;
            cV.Style.Font.FontSize = 12;
            cV.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cV.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            row++;
        }

        // Desglose por período
        row += 2;
        MergeYEstilar(ws, $"A{row}:C{row}", "DESGLOSE POR PERÍODO", CAzul, XLColor.White, 11, 20);
        row++;

        string[] hCols = { "Período", "Ventas", "Monto (S/.)" };
        for (int i = 0; i < hCols.Length; i++) EstilarHeader(ws.Cell(row, i + 1), hCols[i]);
        row++;

        var grupos = periodo.ToLowerInvariant() switch
        {
            "mensual" => ventas
                .GroupBy(v => new { v.FechaCreacion.Year, v.FechaCreacion.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => (
                    Periodo: $"{g.Key.Year}-{g.Key.Month:D2}",
                    Cant:    g.Count(),
                    Monto:   g.Sum(v => v.VentasDetalles.Sum(d => d.Cantidad * d.PrecioUnitario))
                )).ToList(),

            "trimestral" => ventas
                .GroupBy(v => new
                {
                    v.FechaCreacion.Year,
                    Trim = (v.FechaCreacion.Month - 1) / 3 + 1,
                })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Trim)
                .Select(g => (
                    Periodo: $"{g.Key.Year} Q{g.Key.Trim}",
                    Cant:    g.Count(),
                    Monto:   g.Sum(v => v.VentasDetalles.Sum(d => d.Cantidad * d.PrecioUnitario))
                )).ToList(),

            _ => ventas
                .GroupBy(v => v.FechaCreacion.Date)
                .OrderBy(g => g.Key)
                .Select(g => (
                    Periodo: g.Key.ToString("dd/MM/yyyy"),
                    Cant:    g.Count(),
                    Monto:   g.Sum(v => v.VentasDetalles.Sum(d => d.Cantidad * d.PrecioUnitario))
                )).ToList(),
        };

        for (int i = 0; i < grupos.Count; i++)
        {
            var (p, cant, monto) = grupos[i];
            var bg = i % 2 == 0 ? CFila : XLColor.White;

            ws.Cell(row, 1).Value = p;
            ws.Cell(row, 2).Value = cant;
            ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 3).Value = monto;
            ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            var rng = ws.Range(row, 1, row, 3);
            rng.Style.Fill.BackgroundColor = bg;
            rng.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rng.Style.Border.OutsideBorderColor = CBorde;
            row++;
        }

        // Fila de totales
        ws.Cell(row, 1).Value = "TOTAL";
        ws.Cell(row, 1).Style.Fill.BackgroundColor = CNavy;
        ws.Cell(row, 1).Style.Font.FontColor = XLColor.White;
        ws.Cell(row, 1).Style.Font.Bold = true;

        ws.Cell(row, 2).Value = ventas.Count;
        ws.Cell(row, 2).Style.Fill.BackgroundColor = CNavy;
        ws.Cell(row, 2).Style.Font.FontColor = XLColor.White;
        ws.Cell(row, 2).Style.Font.Bold = true;
        ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Cell(row, 3).Value = totalMonto;
        ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
        ws.Cell(row, 3).Style.Fill.BackgroundColor = CNavy;
        ws.Cell(row, 3).Style.Font.FontColor = XLColor.White;
        ws.Cell(row, 3).Style.Font.Bold = true;
        ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static void MergeYEstilar(IXLWorksheet ws, string rango, string texto,
        XLColor fondo, XLColor fuente, int size, double height)
    {
        var cell = ws.Range(rango).Merge().FirstCell();
        cell.Value = texto;
        cell.Style.Fill.BackgroundColor = fondo;
        cell.Style.Font.FontColor = fuente;
        cell.Style.Font.Bold = true;
        cell.Style.Font.FontSize = size;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        var rowNum = cell.Address.RowNumber;
        ws.Row(rowNum).Height = height;
    }

    private static void EstilarHeader(IXLCell cell, string? texto = null)
    {
        if (texto is not null) cell.Value = texto;
        cell.Style.Fill.BackgroundColor = CHeader;
        cell.Style.Font.FontColor = XLColor.White;
        cell.Style.Font.Bold = true;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        cell.Style.Border.OutsideBorderColor = CBorde;
    }

    private static void FilaTabla(IXLWorksheet ws, int row, int idx,
        string label, int unidades, int skus, int sinStock)
    {
        var bg = idx % 2 == 0 ? CFila : XLColor.White;
        ws.Cell(row, 1).Value = label;
        ws.Cell(row, 2).Value = unidades;
        ws.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(row, 3).Value = skus;
        ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(row, 4).Value = sinStock;
        ws.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        if (sinStock > 0)
        {
            ws.Cell(row, 4).Style.Font.FontColor = CTRojo;
            ws.Cell(row, 4).Style.Font.Bold = true;
        }
        ws.Range(row, 1, row, 4).Style.Fill.BackgroundColor = bg;
        ws.Range(row, 1, row, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.Range(row, 1, row, 4).Style.Border.OutsideBorderColor = CBorde;
    }

    private static void FilaTotales(IXLWorksheet ws, int row, int cols,
        string label, int unidades, int skus, int sinStock)
    {
        ws.Cell(row, 1).Value = label;
        ws.Cell(row, 2).Value = unidades;
        ws.Cell(row, 3).Value = skus;
        ws.Cell(row, 4).Value = sinStock;
        var rng = ws.Range(row, 1, row, cols);
        rng.Style.Fill.BackgroundColor = CNavy;
        rng.Style.Font.FontColor = XLColor.White;
        rng.Style.Font.Bold = true;
        rng.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
    }

    private static string FiltrosTexto(string? nombre, string? genero, string? categoria)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(nombre))    parts.Add($"Nombre: {nombre}");
        if (!string.IsNullOrWhiteSpace(genero))    parts.Add($"Género: {genero}");
        if (!string.IsNullOrWhiteSpace(categoria)) parts.Add($"Categoría: {categoria}");
        return parts.Count > 0 ? string.Join("   |   ", parts) : "Ninguno (todos)";
    }
}
