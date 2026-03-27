# TESTS.md

Plan de cobertura de tests para `FacturacionA4V`.
Framework: **xUnit** + **Moq**. Proyecto: `FacturacionA4V.Tests` (`net10.0-windows`).

> Los métodos `private static` de `ExcelFacturacionRepository` (`CalcularEstado`, `TryParseMontoAR`, `TryParseFecha`) deben marcarse `internal` y agregar `[assembly: InternalsVisibleTo("FacturacionA4V.Tests")]` en el proyecto principal para poder testearlos directamente.

---

## Grupo 1 — Infrastructure

### TEST-001
**Nombre**: `CalcularEstado_SinNroFactura_RetornaSinFactura`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `CalcularEstado` (internal static)
**Setup**: `nroFactura = ""`, `fechaFactura = null`, `fechaPago = null`
**Verificar**: retorna `EstadoFactura.SinFactura`

---

### TEST-002
**Nombre**: `CalcularEstado_ConNroFacturaYFechaPago_RetornaPagada`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `CalcularEstado`
**Setup**: `nroFactura = "F-001"`, `fechaFactura = DateTime.Today.AddDays(-10)`, `fechaPago = DateTime.Today`
**Verificar**: retorna `EstadoFactura.Pagada`

---

### TEST-003
**Nombre**: `CalcularEstado_ConNroFacturaSinFechaFactura_RetornaFacturadaMenor30`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `CalcularEstado`
**Setup**: `nroFactura = "F-001"`, `fechaFactura = null`, `fechaPago = null`
**Verificar**: retorna `EstadoFactura.FacturadaMenor30`

---

### TEST-004
**Nombre**: `CalcularEstado_FechaFacturaMenos20Dias_RetornaFacturadaMenor30`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `CalcularEstado`
**Setup**: `nroFactura = "F-001"`, `fechaFactura = DateTime.Today.AddDays(-20)`, `fechaPago = null`
**Verificar**: retorna `EstadoFactura.FacturadaMenor30`

---

### TEST-005
**Nombre**: `CalcularEstado_FechaFacturaMenos45Dias_RetornaFacturada30`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `CalcularEstado`
**Setup**: `nroFactura = "F-001"`, `fechaFactura = DateTime.Today.AddDays(-45)`, `fechaPago = null`
**Verificar**: retorna `EstadoFactura.Facturada30`

---

### TEST-006
**Nombre**: `CalcularEstado_FechaFacturaMenos75Dias_RetornaFacturada60`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `CalcularEstado`
**Setup**: `nroFactura = "F-001"`, `fechaFactura = DateTime.Today.AddDays(-75)`, `fechaPago = null`
**Verificar**: retorna `EstadoFactura.Facturada60`

---

### TEST-007
**Nombre**: `CalcularEstado_FechaFacturaMenos100Dias_RetornaFacturada90`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `CalcularEstado`
**Setup**: `nroFactura = "F-001"`, `fechaFactura = DateTime.Today.AddDays(-100)`, `fechaPago = null`
**Verificar**: retorna `EstadoFactura.Facturada90`

---

### TEST-008
**Nombre**: `TryParseMontoAR_MontoValido_RetornaDecimal`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `TryParseMontoAR` (internal static)
**Setup**: `raw = "1.500,75"` (formato es-AR)
**Verificar**: retorna `1500.75m`

---

### TEST-009
**Nombre**: `TryParseMontoAR_CadenaVacia_RetornaNull`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `TryParseMontoAR`
**Setup**: `raw = ""`
**Verificar**: retorna `null`

---

### TEST-010
**Nombre**: `TryParseMontoAR_CadenaInvalida_RetornaNull`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `TryParseMontoAR`
**Setup**: `raw = "abc"`
**Verificar**: retorna `null`

---

### TEST-011
**Nombre**: `TryParseFecha_CeldaDateTime_RetornaFecha`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `TryParseFecha` (internal static)
**Setup**: celda ClosedXML con `DataType = DateTime` y valor `2025-03-15`
**Verificar**: retorna `new DateTime(2025, 3, 15)`

---

### TEST-012
**Nombre**: `TryParseFecha_CeldaNumeroSerial_RetornaFecha`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `TryParseFecha`
**Setup**: celda con número serial OADate correspondiente a `2025-03-15`
**Verificar**: retorna `new DateTime(2025, 3, 15)`

---

### TEST-013
**Nombre**: `TryParseFecha_CeldaStringFecha_RetornaFecha`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `TryParseFecha`
**Setup**: celda con string `"2025-03-15"`
**Verificar**: retorna `new DateTime(2025, 3, 15)`

---

### TEST-014
**Nombre**: `TryParseFecha_CeldaStringInvalido_RetornaNull`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `TryParseFecha`
**Setup**: celda con string `"no-es-fecha"`
**Verificar**: retorna `null`

---

### TEST-015
**Nombre**: `InsertMany_ArchivoNuevo_CreaHojaConHeaderYFila`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `InsertMany`
**Setup**: path temporal sin archivo previo, una `FacturacionRow` con todos los campos completos. Mock de `GoogleDriveFileService` o stub que no hace nada.
**Verificar**: el archivo creado tiene hoja "Facturacion", fila 1 con headers, fila 2 con los valores de la row (ID, Auspiciante, Programa, Periodista, Monto, TipoFactura, MesAnio, NroFactura, FechaFactura, Nota, FechaPago).

---

### TEST-016
**Nombre**: `InsertMany_ArchivoExistente_AgregaFilaSinPisarDatos`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `InsertMany`
**Setup**: archivo existente con 1 fila de datos, insertar 1 row nueva.
**Verificar**: el archivo queda con 2 filas de datos (más el header).

---

### TEST-017
**Nombre**: `InsertMany_VariasRows_InsertaTodasEnOrden`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `InsertMany`
**Setup**: insertar 3 rows distintas.
**Verificar**: el archivo tiene 3 filas de datos en el mismo orden.

---

### TEST-018
**Nombre**: `ReadAll_ArchivoInexistente_RetornaListaVacia`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `ReadAll`
**Setup**: path que no existe en disco, Drive mock que falla silenciosamente.
**Verificar**: retorna `Array.Empty<FacturacionItem>()`.

---

### TEST-019
**Nombre**: `ReadAll_ConDatos_MapeatodosLosCamposCorrectamente`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `ReadAll`
**Setup**: crear `Facturacion.xlsx` con 1 fila de datos conocidos. Mock de Drive que no descarga nada.
**Verificar**: el `FacturacionItem` retornado tiene todos los campos mapeados (Id, Auspiciante, Programa, Periodista, MontoTexto, MesAnio, NroFactura, FechaFactura, FechaPago, Nota, Estado).

---

### TEST-020
**Nombre**: `ReadAll_CalculaEstadoCorrectamente`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `ReadAll`
**Setup**: 3 filas con distintos estados (SinFactura, Pagada, Facturada30).
**Verificar**: cada item tiene el `Estado` correcto.

---

### TEST-021
**Nombre**: `UpdateFactura_ActualizaCamposCorrectosEnExcel`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `UpdateFactura`
**Setup**: Excel con 2 rows. Actualizar solo la segunda con `NroFactura`, `FechaFactura`, `Nota`.
**Verificar**: la fila 2 tiene los nuevos valores en columnas 8, 9, 10. La fila 1 no cambió.

---

### TEST-022
**Nombre**: `UpdateFactura_ArchivoInexistente_LanzaInvalidOperationException`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `UpdateFactura`
**Setup**: path sin archivo.
**Verificar**: lanza `InvalidOperationException`.

---

### TEST-023
**Nombre**: `UpdatePago_ActualizaFechaPagoEnColumna11`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `UpdatePago`
**Setup**: Excel con 1 row. Actualizar con `PagoUpdate` con fecha.
**Verificar**: columna 11 de la fila tiene la fecha correcta.

---

### TEST-024
**Nombre**: `UpdatePago_ArchivoInexistente_LanzaInvalidOperationException`
**Archivo bajo prueba**: `Infrastructure/ExcelFacturacionRepository.cs`
**Método**: `UpdatePago`
**Setup**: path sin archivo.
**Verificar**: lanza `InvalidOperationException`.

---

### TEST-025
**Nombre**: `ExcelDatosInicioRepository_Load_ArchivoInexistente_LanzaFileNotFoundException`
**Archivo bajo prueba**: `Infrastructure/ExcelDatosInicioRepository.cs`
**Método**: `Load`
**Setup**: path inexistente.
**Verificar**: lanza `FileNotFoundException`.

---

### TEST-026
**Nombre**: `ExcelDatosInicioRepository_Load_HojaFaltante_LanzaInvalidOperationException`
**Archivo bajo prueba**: `Infrastructure/ExcelDatosInicioRepository.cs`
**Método**: `Load`
**Setup**: Excel con solo las hojas "Auspiciantes" y "Programas" (falta "Periodistas").
**Verificar**: lanza `InvalidOperationException` con mensaje que incluye "Periodistas".

---

### TEST-027
**Nombre**: `ExcelDatosInicioRepository_Load_RetornaDatosOrdenadosSinDuplicados`
**Archivo bajo prueba**: `Infrastructure/ExcelDatosInicioRepository.cs`
**Método**: `Load` → `ReadColumnA`
**Setup**: hoja "Auspiciantes" con valores `["Beta", "Alfa", "Beta", "Gamma"]`.
**Verificar**: `DatosInicioCache.Auspiciantes` = `["Alfa", "Beta", "Gamma"]` (sin duplicados, ordenado).

---

### TEST-028
**Nombre**: `ExcelDatosInicioRepository_Load_HojaVacia_RetornaListaVacia`
**Archivo bajo prueba**: `Infrastructure/ExcelDatosInicioRepository.cs`
**Método**: `ReadColumnA`
**Setup**: hoja sin datos.
**Verificar**: la lista correspondiente en el cache está vacía.

---

## Grupo 2 — Domain

### TEST-029
**Nombre**: `MesItem_Constructor_NombreEnEspaniolArgentino`
**Archivo bajo prueba**: `Domain/MesItem.cs`
**Setup**: `new MesItem(1)`
**Verificar**: `Nombre == "ene"`, `Numero == 1`

---

### TEST-030
**Nombre**: `MesItem_Constructor_TodosLosMeses_NombresCorrectos`
**Archivo bajo prueba**: `Domain/MesItem.cs`
**Setup**: crear `MesItem` para meses 1 a 12.
**Verificar**: los nombres coinciden con la cultura `es-AR` (ene, feb, mar, abr, may, jun, jul, ago, sep, oct, nov, dic).

---

### TEST-031
**Nombre**: `MesItem_Seleccionado_DispararaPropertyChanged`
**Archivo bajo prueba**: `Domain/MesItem.cs`
**Setup**: suscribir `PropertyChanged`, cambiar `Seleccionado = true`.
**Verificar**: el evento se dispara con `PropertyName == "Seleccionado"`.

---

### TEST-032
**Nombre**: `DatosInicioCache_Constructor_ExponePropiedadesCorrectas`
**Archivo bajo prueba**: `Domain/DatosInicioCache.cs`
**Setup**: `new DatosInicioCache(["A"], ["P"], ["Per"])`
**Verificar**: `Auspiciantes`, `Programas`, `Periodistas` retornan las listas pasadas.

---

### TEST-033
**Nombre**: `FacturacionItem_IsSelected_DisparaPropertyChanged`
**Archivo bajo prueba**: `Domain/FacturacionItem.cs`
**Setup**: suscribir `PropertyChanged`, cambiar `IsSelected = true`.
**Verificar**: el evento se dispara con `PropertyName == "IsSelected"`.

---

### TEST-034
**Nombre**: `FacturacionItem_IsSelected_MismoValor_NoDisparaPropertyChanged`
**Archivo bajo prueba**: `Domain/FacturacionItem.cs`
**Setup**: `IsSelected = false` (default), asignar `IsSelected = false` de nuevo.
**Verificar**: el evento NO se dispara.

---

## Grupo 3 — ViewModel base

### TEST-035
**Nombre**: `ObservableObject_OnPropertyChanged_DisparaEventoConNombreCorrecto`
**Archivo bajo prueba**: `UI/ViewModel/ObservableObject.cs`
**Setup**: subclase concreta que llama `OnPropertyChanged("MiPropiedad")`.
**Verificar**: `PropertyChanged` se dispara con `PropertyName == "MiPropiedad"`.

---

### TEST-036
**Nombre**: `ObservableObject_OnPropertyChanged_SinSuscriptores_NoLanzaExcepcion`
**Archivo bajo prueba**: `UI/ViewModel/ObservableObject.cs`
**Setup**: llamar `OnPropertyChanged` sin suscriptores.
**Verificar**: no lanza excepción.

---

### TEST-037
**Nombre**: `RelayCommand_Execute_LlamaAccion`
**Archivo bajo prueba**: `UI/ViewModel/RelayCommand.cs`
**Setup**: `new RelayCommand(() => called = true)`.
**Verificar**: `Execute(null)` → `called == true`.

---

### TEST-038
**Nombre**: `RelayCommand_CanExecute_SinFuncion_RetornaTrue`
**Archivo bajo prueba**: `UI/ViewModel/RelayCommand.cs`
**Setup**: `new RelayCommand(() => {})` sin `canExecute`.
**Verificar**: `CanExecute(null) == true`.

---

### TEST-039
**Nombre**: `RelayCommand_CanExecute_ConFuncionFalsa_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/RelayCommand.cs`
**Setup**: `new RelayCommand(() => {}, () => false)`.
**Verificar**: `CanExecute(null) == false`.

---

### TEST-040
**Nombre**: `RelayCommand_RaiseCanExecuteChanged_DisparaEvento`
**Archivo bajo prueba**: `UI/ViewModel/RelayCommand.cs`
**Setup**: suscribir `CanExecuteChanged`, llamar `RaiseCanExecuteChanged()`.
**Verificar**: el evento se dispara.

---

### TEST-041
**Nombre**: `RelayCommandT_Execute_ConParametroCorrecto_LlamaAccion`
**Archivo bajo prueba**: `UI/ViewModel/RelayCommand.cs`
**Setup**: `new RelayCommand<string>(s => received = s)`.
**Verificar**: `Execute("hola")` → `received == "hola"`.

---

### TEST-042
**Nombre**: `RelayCommandT_Execute_ConParametroIncorrecto_NoLanzaExcepcion`
**Archivo bajo prueba**: `UI/ViewModel/RelayCommand.cs`
**Setup**: `new RelayCommand<string>(s => {})`.
**Verificar**: `Execute(123)` (int en vez de string) no lanza excepción.

---

### TEST-043
**Nombre**: `RelayCommandT_CanExecute_ConParametroIncorrecto_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/RelayCommand.cs`
**Setup**: `new RelayCommand<string>(_ => {}, _ => true)`.
**Verificar**: `CanExecute(123)` retorna `false`.

---

### TEST-044
**Nombre**: `SelectableItem_IsSelected_DisparaPropertyChanged`
**Archivo bajo prueba**: `UI/Behaviors/SelectableItem.cs`
**Setup**: `new SelectableItem<string>("valor")`, suscribir evento.
**Verificar**: cambiar `IsSelected = true` → evento disparado con `PropertyName == "IsSelected"`.

---

### TEST-045
**Nombre**: `SelectableItem_IsSelected_MismoValor_NoDisparaEvento`
**Archivo bajo prueba**: `UI/Behaviors/SelectableItem.cs`
**Setup**: `IsSelected = false`, asignar `false` de nuevo.
**Verificar**: evento NO disparado.

---

## Grupo 4 — ViewModels

### TEST-046
**Nombre**: `AgregarFacturaViewModel_ValoresIniciales_FechaHoyYNroVacio`
**Archivo bajo prueba**: `UI/ViewModel/AgregarFacturaViewModel.cs`
**Setup**: `new AgregarFacturaViewModel()`
**Verificar**: `NroFactura == ""`, `FechaFactura == DateTime.Today`, `Nota == null`.

---

### TEST-047
**Nombre**: `AgregarFacturaViewModel_IsValid_NroVacio_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/AgregarFacturaViewModel.cs`
**Setup**: `NroFactura = ""`
**Verificar**: `IsValid == false`.

---

### TEST-048
**Nombre**: `AgregarFacturaViewModel_IsValid_NroSoloEspacios_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/AgregarFacturaViewModel.cs`
**Setup**: `NroFactura = "   "`
**Verificar**: `IsValid == false`.

---

### TEST-049
**Nombre**: `AgregarFacturaViewModel_IsValid_NroValido_RetornaTrue`
**Archivo bajo prueba**: `UI/ViewModel/AgregarFacturaViewModel.cs`
**Setup**: `NroFactura = "F-001"`
**Verificar**: `IsValid == true`.

---

### TEST-050
**Nombre**: `AgregarFacturaViewModel_CambioNroFactura_DisparaPropertyChangedDeIsValid`
**Archivo bajo prueba**: `UI/ViewModel/AgregarFacturaViewModel.cs`
**Setup**: suscribir `PropertyChanged`, cambiar `NroFactura`.
**Verificar**: se dispara `PropertyChanged` con `PropertyName == "IsValid"`.

---

### TEST-051
**Nombre**: `AgregarPagoViewModel_ValoresIniciales_FechaHoy`
**Archivo bajo prueba**: `UI/ViewModel/AgregarPagoViewModel.cs`
**Setup**: `new AgregarPagoViewModel()`
**Verificar**: `FechaPago == DateTime.Today`.

---

### TEST-052
**Nombre**: `AgregarPagoViewModel_IsValid_FechaDefault_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/AgregarPagoViewModel.cs`
**Setup**: `FechaPago = default`
**Verificar**: `IsValid == false`.

---

### TEST-053
**Nombre**: `AgregarPagoViewModel_IsValid_FechaValida_RetornaTrue`
**Archivo bajo prueba**: `UI/ViewModel/AgregarPagoViewModel.cs`
**Setup**: `FechaPago = DateTime.Today`
**Verificar**: `IsValid == true`.

---

### TEST-054
**Nombre**: `MainViewModel_Constructor_ExponeCantidadesDesdeCache`
**Archivo bajo prueba**: `UI/ViewModel/MainViewModel.cs`
**Setup**: `cache` con 3 auspiciantes, 2 programas, 4 periodistas.
**Verificar**: `AuspiciantesCount == 3`, `ProgramasCount == 2`, `PeriodistasCount == 4`.

---

### TEST-055
**Nombre**: `CargaViewModel_ValoresIniciales_AnioActualYTipoFacturaA`
**Archivo bajo prueba**: `UI/ViewModel/CargaViewModel.cs`
**Setup**: `new CargaViewModel(cache, repoMock)`.
**Verificar**: `AnioTexto == DateTime.Now.Year.ToString()`, `TipoFactura == "Factura A"`, ningún mes seleccionado.

---

### TEST-056
**Nombre**: `CargaViewModel_PuedeProcesar_SinSelecciones_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/CargaViewModel.cs`
**Setup**: vm recién creado, sin seleccionar nada.
**Verificar**: `ProcesarCommand.CanExecute(null) == false`.

---

### TEST-057
**Nombre**: `CargaViewModel_PuedeProcesar_TodoCompleto_RetornaTrue`
**Archivo bajo prueba**: `UI/ViewModel/CargaViewModel.cs`
**Setup**: seleccionar auspiciante, programa, periodista, monto y al menos un mes.
**Verificar**: `ProcesarCommand.CanExecute(null) == true`.

---

### TEST-058
**Nombre**: `CargaViewModel_PuedeProcesar_AuspicianteNoEnCache_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/CargaViewModel.cs`
**Setup**: `AuspicianteSeleccionado = "Uno que no existe"`, resto completo.
**Verificar**: `ProcesarCommand.CanExecute(null) == false`.

---

### TEST-059
**Nombre**: `CargaViewModel_Procesar_LlamaInsertManyConRowsCorrectas`
**Archivo bajo prueba**: `UI/ViewModel/CargaViewModel.cs`
**Método**: `Procesar` (via `ProcesarCommand.Execute`)
**Setup**: seleccionar auspiciante válido, programa, periodista, monto = "1000", año = "2025", meses 1 y 3. Mock de `IFacturacionRepository`.
**Verificar**: `InsertMany` llamado una vez con 2 rows: `MesAnio = "01/2025"` y `MesAnio = "03/2025"`.

---

### TEST-060
**Nombre**: `CargaViewModel_Procesar_LimpiaFormularioDepues`
**Archivo bajo prueba**: `UI/ViewModel/CargaViewModel.cs`
**Setup**: completar todo y ejecutar `ProcesarCommand`.
**Verificar**: después de procesar, `AuspicianteSeleccionado == null`, `ProgramaSeleccionado == null`, `PeriodistaSeleccionado == null`, `MontoTexto == null`, todos los meses `Seleccionado == false`.

---

### TEST-061
**Nombre**: `CargaViewModel_ToggleMes_AgregaMes`
**Archivo bajo prueba**: `UI/ViewModel/CargaViewModel.cs`
**Setup**: llamar `ToggleMes("01")`.
**Verificar**: `MesesSeleccionados.Contains("01") == true`.

---

### TEST-062
**Nombre**: `CargaViewModel_ToggleMes_MismoMesDosVeces_LoQuita`
**Archivo bajo prueba**: `UI/ViewModel/CargaViewModel.cs`
**Setup**: llamar `ToggleMes("01")` dos veces.
**Verificar**: `MesesSeleccionados.Contains("01") == false`.

---

### TEST-063
**Nombre**: `ResultadosViewModel_Cargar_PopulaFiltrosDesdeRepo`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: mock de `IFacturacionRepository.ReadAll()` retornando 2 items con años distintos, auspiciantes distintos.
**Verificar**: `AuspiciantesDisponibles`, `ProgramasDisponibles`, `PeriodistasDisponibles`, `AniosDisponibles` contienen los valores únicos de los items.

---

### TEST-064
**Nombre**: `ResultadosViewModel_AplicarFiltros_SinFiltros_MuestraTodos`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: 3 items en repo mock, sin filtro activo.
**Verificar**: `Filtrados.Count == 3`.

---

### TEST-065
**Nombre**: `ResultadosViewModel_AplicarFiltros_PorAuspiciante_MuestraSolo Matching`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: 3 items, 2 con `Auspiciante = "A"` y 1 con `Auspiciante = "B"`. Establecer `AuspicianteSeleccionado = "A"`.
**Verificar**: `Filtrados.Count == 2`.

---

### TEST-066
**Nombre**: `ResultadosViewModel_AplicarFiltros_PorAnio_MuestraSoloEseAnio`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: items con `MesAnio = "01/2024"` y `MesAnio = "01/2025"`. Establecer `AnioSeleccionado = 2025`.
**Verificar**: `Filtrados.Count == 1` con `MesAnio = "01/2025"`.

---

### TEST-067
**Nombre**: `ResultadosViewModel_AplicarFiltros_PorPrograma_FiltraCorrectamente`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: items con distintos programas.
**Verificar**: solo aparecen los que coinciden con `ProgramaSeleccionado`.

---

### TEST-068
**Nombre**: `ResultadosViewModel_AplicarFiltros_PorPeriodista_FiltraCorrectamente`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: items con distintos periodistas.
**Verificar**: solo aparecen los que coinciden con `PeriodistaSeleccionado`.

---

### TEST-069
**Nombre**: `ResultadosViewModel_AplicarFiltros_MultiplesFiltros_InterseccionCorrecta`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: filtrar por auspiciante + año combinados.
**Verificar**: solo items que cumplen ambas condiciones.

---

### TEST-070
**Nombre**: `ResultadosViewModel_RecalcularTotales_SumaCorrectamente`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: 3 items: uno `SinFactura` ($1000), uno `Pagada` ($2000), uno `Facturada30` ($500).
**Verificar**: `TotalFacturado == 3500`, `TotalCobrado == 2000`, `TotalPendiente == 1500`.

---

### TEST-071
**Nombre**: `ResultadosViewModel_RecalcularTotales_ItemSinMontoParsed_NoSuma`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: 1 item con `MontoParsed = null`.
**Verificar**: `TotalFacturado == 0`.

---

### TEST-072
**Nombre**: `ResultadosViewModel_PuedeAgregarFactura_SinSeleccionados_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: ningún item seleccionado.
**Verificar**: `AgregarFacturaCommand.CanExecute(null) == false`.

---

### TEST-073
**Nombre**: `ResultadosViewModel_PuedeAgregarFactura_SeleccionadoSinFactura_RetornaTrue`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: item con `NroFactura = null` y `IsSelected = true`.
**Verificar**: `AgregarFacturaCommand.CanExecute(null) == true`.

---

### TEST-074
**Nombre**: `ResultadosViewModel_PuedeAgregarFactura_SeleccionadoConFactura_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: item con `NroFactura = "F-001"` y `IsSelected = true`.
**Verificar**: `AgregarFacturaCommand.CanExecute(null) == false`.

---

### TEST-075
**Nombre**: `ResultadosViewModel_PuedeAgregarPago_SinSeleccionados_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: ningún item seleccionado.
**Verificar**: `AgregarPagoCommand.CanExecute(null) == false`.

---

### TEST-076
**Nombre**: `ResultadosViewModel_PuedeAgregarPago_SeleccionadoConFacturaSinPago_RetornaTrue`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: item con `NroFactura = "F-001"`, `FechaPago = ""`, `IsSelected = true`.
**Verificar**: `AgregarPagoCommand.CanExecute(null) == true`.

---

### TEST-077
**Nombre**: `ResultadosViewModel_PuedeAgregarPago_SeleccionadoConPago_RetornaFalse`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: item con `NroFactura = "F-001"`, `FechaPago = "15/03/2025"`, `IsSelected = true`.
**Verificar**: `AgregarPagoCommand.CanExecute(null) == false`.

---

### TEST-078
**Nombre**: `ResultadosViewModel_LimpiarFiltros_ReseteatodoYMuestraTodos`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: aplicar filtros, luego ejecutar `LimpiarFiltrosCommand`.
**Verificar**: `AuspicianteSeleccionado == null`, `AnioSeleccionado == null`, `Filtrados.Count` igual al total de items.

---

### TEST-079
**Nombre**: `ResultadosViewModel_Seleccion_ActualizaCanExecuteDeComandos`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Setup**: item sin factura en `Filtrados`, `IsSelected = false`. Suscribir `CanExecuteChanged` de ambos comandos.
**Verificar**: al cambiar `IsSelected = true` se disparan `CanExecuteChanged` de `AgregarFacturaCommand` y `AgregarPagoCommand`.

---

### TEST-080
**Nombre**: `ShellViewModel_Constructor_CurrentViewModelEsInicio`
**Archivo bajo prueba**: `UI/ViewModel/ShellViewModel.cs`
**Setup**: factories que retornan objetos distinguibles (`"inicio"`, `"carga"`, `"resultados"`).
**Verificar**: `CurrentViewModel == "inicio"` al construir.

---

### TEST-081
**Nombre**: `ShellViewModel_IrACarga_CambiaCurrent ViewModel`
**Archivo bajo prueba**: `UI/ViewModel/ShellViewModel.cs`
**Setup**: ejecutar `IrACargaCommand`.
**Verificar**: `CurrentViewModel == "carga"`.

---

### TEST-082
**Nombre**: `ShellViewModel_IrAResultados_CambiaCurrentViewModel`
**Archivo bajo prueba**: `UI/ViewModel/ShellViewModel.cs`
**Setup**: ejecutar `IrAResultadosCommand`.
**Verificar**: `CurrentViewModel == "resultados"`.

---

### TEST-083
**Nombre**: `ShellViewModel_IrAInicio_CambiaCurrentViewModel`
**Archivo bajo prueba**: `UI/ViewModel/ShellViewModel.cs`
**Setup**: navegar a carga primero, luego ejecutar `IrAInicioCommand`.
**Verificar**: `CurrentViewModel == "inicio"`.

---

### TEST-084
**Nombre**: `ShellViewModel_Navegacion_DisparaPropertyChangedDeCurrentViewModel`
**Archivo bajo prueba**: `UI/ViewModel/ShellViewModel.cs`
**Setup**: suscribir `PropertyChanged`, ejecutar `IrACargaCommand`.
**Verificar**: `PropertyChanged` disparado con `PropertyName == "CurrentViewModel"`.

---

## Grupo 5 — Bugs conocidos (tests de regresión)

### TEST-085 *(BUG-001)*
**Nombre**: `ResultadosViewModel_Cargar_MantieneElFiltroActivo`
**Archivo bajo prueba**: `UI/ViewModel/ResultadosViewModel.cs`
**Describe**: verifica que BUG-001 está corregido — `Cargar()` no debe resetear el filtro activo.
**Setup**: 3 items (2 del año 2024, 1 del 2025). Establecer `AnioSeleccionado = 2025`. Luego llamar `Cargar()`.
**Verificar**: `Filtrados.Count == 1` (el filtro se mantiene tras recargar).

---

### TEST-086 *(BUG-003)*
**Nombre**: `GoogleDriveFileService_Upload_AguardaCompletarUpload`
**Archivo bajo prueba**: `Infrastructure/IExcelStorage.cs` — `GoogleDriveFileService.Upload`
**Describe**: verifica que BUG-003 está corregido — `UploadAsync` debe ser awaited.
**Setup**: mock/stub de `DriveService` que registra si `UploadAsync` fue completado.
**Verificar**: el método `Upload` retorna solo después que `UploadAsync` completó.

---

## Resumen de cobertura

| Capa | Clases cubiertas | Tests |
|------|-----------------|-------|
| Infrastructure | `ExcelFacturacionRepository`, `ExcelDatosInicioRepository`, `GoogleDriveFileService` | TEST-001 al TEST-028, TEST-086 |
| Domain | `MesItem`, `DatosInicioCache`, `FacturacionItem` | TEST-029 al TEST-034 |
| ViewModel base | `ObservableObject`, `RelayCommand`, `RelayCommand<T>`, `SelectableItem<T>` | TEST-035 al TEST-045 |
| ViewModels | `AgregarFacturaViewModel`, `AgregarPagoViewModel`, `MainViewModel`, `CargaViewModel`, `ResultadosViewModel`, `ShellViewModel` | TEST-046 al TEST-084 |
| Regresión bugs | BUG-001, BUG-003 | TEST-085, TEST-086 |
| **Total** | | **86 tests** |
