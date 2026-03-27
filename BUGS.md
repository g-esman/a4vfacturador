# BUGS.md

Bugs identificados en el código fuente.

---

## BUG-001 — Double read en `Cargar()` ignora filtros activos

**Archivo**: `UI/ViewModel/ResultadosViewModel.cs` — método `Cargar()` (línea ~176)

**Descripción**: `Cargar()` llama `AplicarFiltros()` para poblar `Filtrados`, pero inmediatamente después hace `Filtrados.Clear()` y repobla desde una segunda llamada a `_repo.ReadAll()`, ignorando los filtros. El resultado es que al recargar (por ejemplo, tras agregar una factura o pago) la grilla muestra todos los registros sin filtrar.

**Reproducción**: Seleccionar un filtro de año → agregar factura o pago → la grilla vuelve a mostrar todos los registros.

---

## BUG-002 — `SaveLocal()` no se awaita: lectura con datos desactualizados

**Archivo**: `Infrastructure/ExcelFacturacionRepository.cs` — método `ReadAll()` (línea ~96)

**Descripción**: `ReadAll()` llama `SaveLocal()` (que descarga el archivo desde Google Drive de forma asíncrona) sin awaitar el resultado. La ejecución continúa inmediatamente y lee el archivo local que puede estar desactualizado o inexistente. El archivo de Drive se termina de descargar después de que ya se leyeron los datos.

---

## BUG-003 — `UploadToDrive()` silencia errores de upload

**Archivo**: `Infrastructure/ExcelFacturacionRepository.cs` — método `UploadToDrive()` (línea ~29) e `Infrastructure/IExcelStorage.cs` — método `Upload()` (línea ~46)

**Descripción**: En `GoogleDriveFileService.Upload()`, `request.UploadAsync()` no se awaita — es fire-and-forget puro dentro de un `Task.Run`. Los errores de subida a Drive se pierden silenciosamente. El usuario ve que la operación "funcionó" pero el archivo en Drive puede no haberse actualizado.

---

## BUG-004 — `FacturacionItem` depende de `ObservableObject` del namespace de UI

**Archivo**: `Domain/FacturacionItem.cs`

**Descripción**: `FacturacionItem` hereda de `ObservableObject` que vive en `FacturacionA4V.UI.ViewModel`. Un modelo de dominio no debería depender de una clase de la capa de UI. Esto impide testear `FacturacionItem` sin referenciar la capa de UI y acopla ambas capas.
