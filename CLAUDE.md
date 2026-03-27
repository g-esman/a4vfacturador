# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
dotnet restore                          # Restore NuGet dependencies
dotnet build                            # Build the project
dotnet run                              # Run the application
dotnet build --configuration Release   # Release build
dotnet publish --configuration Release # Publish
```

No test or lint infrastructure is configured yet.

## Architecture

**WPF desktop application** (.NET 10.0, Windows only) for invoice/billing management ("A4V Facturador"). Uses MVVM without a DI container — dependencies are wired manually in `App.xaml.cs`.

### Stack
- **UI**: WPF with MVVM pattern (`ObservableObject`, `RelayCommand` in `UI/ViewModel/`)
- **Excel**: ClosedXML for reading/writing `.xlsx` files
- **Cloud**: Google Drive API v3 (service account auth) — async fire-and-forget upload after every write
- **Config**: `Microsoft.Extensions.Configuration` + `appsettings.json`

### Startup flow (`App.xaml.cs`)
1. Loads `appsettings.json` (Google Drive file ID + service account path)
2. `ExcelDatosInicioRepository.Load()` reads `Datos inicio.xlsx` → `DatosInicioCache` (master lists: Auspiciantes, Programas, Periodistas)
3. Creates `MainViewModel` → `MainWindow` shown

### Navigation
`ShellViewModel` holds `CurrentViewModel` (the active screen). `MainWindow.xaml` uses `DataTemplate` keyed on ViewModel type to render the correct `UserControl`. Navigation is via factory `Func<>` injected into `ShellViewModel`.

### Key data concepts
- **`MesAnio`**: stored as `"MM/YYYY"` string (e.g. `"03/2026"`). Year is parsed via `.Split('/')[1]`, month via `.Split('/')[0]`.
- **`FacturacionRow`**: the write model (inserted via `InsertMany`)
- **`FacturacionItem`**: the read/display model — includes `Estado` (derived, not in Excel) and `IsSelected` for multi-row operations
- **`EstadoFactura`**: enum — `SinFactura`, `Pagada`, `FacturadaMenor30`, `Facturada30`, `Facturada60`, `Facturada90`. Calculated from `NroFactura`, `FechaFactura`, `FechaPago` at read time.

### Excel files (in app base directory)
| File | Purpose |
|------|---------|
| `Datos inicio.xlsx` | Master lists — sheets: Auspiciantes, Programas, Periodistas (column A) |
| `Facturacion.xlsx` | Main data — sheet: Facturacion, 11 columns (ID, Auspiciante, Programa, Periodista, Monto, TipoFactura, MesAnio, NroFactura, FechaFactura, Nota, FechaPago) |

### Known issues in the codebase
- **Double read in `Cargar()`** (`ResultadosViewModel`): calls `_repo.ReadAll()` twice — once to populate filter dropdowns, then again after `AplicarFiltros()` to populate `Filtrados`, ignoring the applied filters.
- **`SaveLocal()` not awaited**: `ReadAll()` calls `SaveLocal()` (async) without awaiting, so the read proceeds with potentially stale local data before the Drive download completes.
- **`UploadToDrive()`**: `request.UploadAsync()` inside `Upload()` in `GoogleDriveFileService` is not awaited — silent fire-and-forget with no error surfacing.
- **`FacturacionItem` depends on `ObservableObject`** from the `UI.ViewModel` namespace — domain model leaks a UI concern.
