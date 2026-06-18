# Prompt de uso — implementar un endpoint / use case

Un **solo** prompt sirve para los dos casos: el agente **detecta solo** si el módulo ya existe
(agrega solo el *delta*) o no (crea el módulo completo). Ambos caminos siguen
[`ESTANDAR_MODULOS.md`](ESTANDAR_MODULOS.md) (las reglas) y [`GUIA_NUEVO_MODULO.md`](GUIA_NUEVO_MODULO.md)
(el paso a paso). Lo único que cambias cada vez es el bloque **FUNCIONALIDAD / ENDPOINT A IMPLEMENTAR**.

---

## Prompt (copiar y pegar)

```text
Implementa esto en el proyecto siguiendo EXACTAMENTE ESTANDAR_MODULOS.md y GUIA_NUEVO_MODULO.md
(raíz del repo), que son la fuente de verdad.

FUNCIONALIDAD / ENDPOINT A IMPLEMENTAR:
<pega aquí el método del @RestController + el @Service + la query del @Repository de Java,
 o describe el endpoint: método HTTP, ruta, entrada, salida y reglas>

CÓMO PROCEDER:
1. Detecta a qué módulo de UseCases pertenece, según la ENTIDAD QUE DEVUELVE.
   - Si el módulo YA EXISTE → agrega solo el delta (no recrees nada, no toques los otros use cases).
   - Si NO existe → crea el módulo completo (pasos 0–8 de la guía).
2. Repositorio:
   - Si la consulta no la cubre IGenericRepository, agrega un método a I<Entidad>Repository (+ impl EF
     con AnyAsync/Where, sin GetAll()-en-memoria).
   - Si la entidad aún no tiene repo propio y lo necesita, créalo y exponlo como propiedad lazy del
     UnitOfWork (NUNCA en DI).
3. Crea el Command/Query + handler. ErrorType correcto (no todo 404): 201 al crear, 200 [] en listas
   vacías, 400/403/404/409 según el caso. Nada de _mediator.Send dentro de un handler.
4. Si hay input con forma (body o filtros con reglas), agrega validador FluentValidation; longitudes =
   columnas de init.sql. Campos de token/ruta van con [JsonIgnore].
5. Une la acción al controller correspondiente con sus roles ([Authorize]) y this.ToActionResult
   (nunca return Ok). Ownership (si aplica) se valida en el handler → 403.
6. Compila Infrastructure y Application (--no-incremental -clp:ErrorsOnly); MSB3027/MSB3021 = lock, no es
   error de C#.

ANTES DE TOCAR NADA: dame un PLAN CORTO (módulo destino y si existe o es nuevo; método de repo; use
cases; validadores; cambios en el controller) y ESPERA mi OK. No inventes; si algo no encaja en el
estándar, pregúntame.
```

---

## Notas

- **Lo único que rellenas** es el bloque `FUNCIONALIDAD / ENDPOINT A IMPLEMENTAR`.
- ¿Ya sabes el módulo y quieres saltarte la detección? Agrega al inicio: `Módulo destino: <Nombre>`.
  No es necesario — el prompt lo resuelve solo.
- El paso final (*plan corto + esperar OK*) es a propósito: evita que se implemente algo que no encaje
  en el estándar.
