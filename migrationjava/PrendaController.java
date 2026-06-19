package com.ecommerce.server.controller.prenda;

import com.ecommerce.server.model.dao.prenda.ImagenDao;
import com.ecommerce.server.model.dao.prenda.PrendaCustomRepository;
import com.ecommerce.server.model.dao.prenda.PrendaDao;
import com.ecommerce.server.model.dto.PageResponseDto;
import com.ecommerce.server.model.dto.descuento.PrendaConDescuentoResponseDto;
import com.ecommerce.server.model.dto.descuento.PrendaConDescuentoResponseTodoDto;
import com.ecommerce.server.model.dto.prenda.*;
import com.ecommerce.server.model.entity.prenda.*;
import com.ecommerce.server.model.payload.Mensajes;
import com.ecommerce.server.service.prenda.*;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.dao.DataAccessException;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@CrossOrigin(origins = {
        "http://localhost:5173",
        "http://localhost:4200",
        "http://localhost:5174",
        "https://sv-02udg1brnilz4phvect8.cloud.elastika.pe",
        "*"
})
@RestController
@RequestMapping("/api/v1")
public class PrendaController {



    @Autowired
    private IPrendaService prendaService;

    @Autowired
    private IMarcaService marcaService;

    @Autowired
    private ITallaService tallaService;

    @Autowired
    private ICategoriaService categoriaService;

    @Autowired
    private IProveedorService proveedorService;

    @Autowired
    private PrendaDao prendaDao;

    @Autowired
    private ImagenDao imagenDao;

    private Mensajes msg = new Mensajes();



    // nuevo
    @Autowired
    private IGeneroService generoService;

    @GetMapping("/prendas/paginado")
    public ResponseEntity<?> getPrendasPaginado(
            @RequestParam(defaultValue = "0") int page,
            @RequestParam(defaultValue = "10") int size
    ) {
        try {
            Pageable pageable = PageRequest.of(page, size, Sort.by("id").descending());
            Page<Prenda> prendas = prendaDao.findAll(pageable);
            if (prendas.isEmpty()) {
                return msg.NoGet();
            }
            PageResponseDto<Prenda> response = new PageResponseDto<>(
                    prendas.getContent(),
                    prendas.getNumber(),
                    prendas.getSize(),
                    prendas.getTotalElements(),
                    prendas.getTotalPages()
            );
            return msg.Get(response);
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }


    @GetMapping("/prendas/buscar")
    public List<PrendaConDescuentoResponseDto> buscarPrendas(
            @RequestParam(required = false) String nombre,
            @RequestParam(required = false) String categoria,
            @RequestParam(required = false) String genero
    ) {
        return prendaDao.buscarPrendasConDescuentoPorNombreCategoriaYGenero(nombre, categoria, genero);
    }

    @GetMapping("/prendas/buscar-por-nombre-genero")
    public List<PrendaConDescuentoResponseDto> buscarPrendasPorNombreYGenero(
            @RequestParam(required = false) String nombre,
            @RequestParam(required = false) String genero
    ) {
        return prendaDao.buscarPrendasConDescuentoPorNombreYGenero(nombre, genero);
    }

    @GetMapping("/prenda-tallas/{categoria}")
    public ResponseEntity<?> getTallasPorCategoria(@PathVariable String categoria) {
        List<String> tallas = prendaDao.findTallasByCategoriaLike(categoria);
        return msg.Get(tallas);
    }
    @GetMapping("/prenda-marcas/{categoria}")
    public ResponseEntity<?> getMarcasPorCategoria(@PathVariable String categoria) {
        List<String> marcas = prendaDao.findMarcasByCategoriaLike(categoria);
        return msg.Get(marcas);
    }
    @GetMapping("/prenda-precios/{categoria}")
    public ResponseEntity<?> getEstadisticasPreciosPorCategoria(@PathVariable String categoria) {
        Object[] stats = prendaDao.findEstadisticasPreciosByCategoriaLike(categoria);
        // stats[0]: promedio, stats[1]: mínimo, stats[2]: máximo
        return msg.Get(stats);
    }
// no sirve
    @GetMapping("/prendas-por-categoria/{categoria}")
    public ResponseEntity<?> getPrendasPorCategoria(@PathVariable String categoria) {
        List<Prenda> prendas = prendaDao.findPrendasByCategoriaLike(categoria);
        return msg.Get(prendas);
    }

    @GetMapping("/prendas/todos-descuentos/{categoria}")
    public ResponseEntity<?> getTodosDescuentosPorCategoria(@PathVariable String categoria) {
        try {
            List<Double> descuentos = prendaDao.findSoloDescuentosPorCategoria(categoria);
            return msg.Get(descuentos);
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }

    @GetMapping("/prendas/descuentos-aplicados")
    public ResponseEntity<?> getPrendasConDescuentoAplicadoPorCategoriaYGenero(
            @RequestParam(required = false) String categoria,
            @RequestParam(required = false) String genero) {
        try {
            List<PrendaConDescuentoResponseDto> resultados = prendaDao.findPrendasConDescuentoAplicadoPorCategoriaYGenero(categoria, genero);
            return msg.Get(resultados);
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }
    //prendas por genero aleatorio

   @GetMapping("/prendas/descuentos-aplicados-por-genero/{genero}")
    public ResponseEntity<?> getPrendasConDescuentoAplicadoPorGeneroAleatorio(@PathVariable String genero) {
        try {
            List<PrendaConDescuentoResponseTodoDto> resultados = prendaDao.findPrendasConDescuentoAplicadoPorGeneroAleatorio(genero);
            return msg.Get(resultados);
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }

    @GetMapping("/prendas/tallas-por-genero/{genero}")
    public ResponseEntity<?> obtenerTallasPorGenero(@PathVariable String genero) {
        List<String> tallas = prendaDao.findTallasByGeneroLike(genero);
        return msg.Get(tallas);
    }

    @GetMapping("/prendas/marcas-por-genero/{genero}")
    public ResponseEntity<?> obtenerMarcasPorGenero(@PathVariable String genero) {
        List<String> marcas = prendaDao.findMarcasByGeneroLike(genero);
        return msg.Get(marcas);
    }

    @GetMapping("/prendas/estadisticas-precios-por-genero/{genero}")
    public ResponseEntity<?> obtenerEstadisticasPreciosPorGenero(@PathVariable String genero) {
        Object[] estadisticas = prendaDao.findEstadisticasPreciosByGeneroLike(genero);
        return msg.Get(estadisticas);
    }

    @GetMapping("/prendas/descuentos-por-genero/{genero}")
    public ResponseEntity<?> obtenerDescuentosPorGenero(@PathVariable String genero) {
        try {
            List<Double> descuentos = prendaDao.findSoloDescuentosPorGenero(genero);
            return msg.Get(descuentos);
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }

    @GetMapping("/prendas/categorias-por-genero/{genero}")
    public ResponseEntity<?> obtenerCategoriasPorGenero(@PathVariable String genero) {
        List<String> categorias = prendaDao.findCategoriasByGeneroLike(genero);
        return msg.Get(categorias);
    }

    @GetMapping("/prendas-filtradas")
    public ResponseEntity<?> filtrarPrendasDinamico(
            @RequestParam(required = false) String talla,
            @RequestParam(required = false) String categoria,
            @RequestParam(required = false) String marca,
            @RequestParam(required = false) String genero,
            @RequestParam(required = false) Double precioMin,
            @RequestParam(required = false) Double precioMax,
            @RequestParam(required = false) Double descMin,
            @RequestParam(required = false) Double descMax
    ) {
        try {
            List<PrendaConDescuentoResponseDto> prendas = prendaDao.filtrarPrendasDinamico(
                    talla, categoria, marca,genero, precioMin, precioMax, descMin, descMax
            );
            return msg.Get(prendas);
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }
    @GetMapping("/todas-prendas-filtradas")
    public ResponseEntity<?> filtrarTodasPrendasDinamico(
            @RequestParam(required = false) String talla,
            @RequestParam(required = false) String categoria,
            @RequestParam(required = false) String marca,
            @RequestParam(required = false) String genero,
            @RequestParam(required = false) Double precioMin,
            @RequestParam(required = false) Double precioMax,
            @RequestParam(required = false) Double descMin,
            @RequestParam(required = false) Double descMax
    ) {
        try {
            List<PrendaConDescuentoResponseDto> prendas = prendaDao.filtrarPrendasDinamico(
                    talla, categoria, marca, genero, precioMin, precioMax, descMin, descMax
            );
            return msg.Get(prendas);
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }


    //    @PreAuthorize("hasAnyAuthority('SCOPE_ADMIN')")
    @GetMapping("/prendas")
    public ResponseEntity<?> showAllPrendas() {
        List<Prenda> getList = prendaService.getPrendas();
        if (getList.isEmpty()) {
            return msg.NoGet();
        }
        return msg.Get(getList);
    }

    //    @PreAuthorize("hasAnyAuthority('SCOPE_ADMIN')")
    //falta tallas
    @GetMapping("/prenda/{id}")
    public ResponseEntity<?> showPrendaById(@PathVariable Long id) {
        Prenda prenda = prendaService.getPrenda(id);
        Marca marca = marcaService.getMarca(prenda.getMarca().getId());
        Categoria categoria = categoriaService.getCategoria(prenda.getCategoria().getId());
        Proveedor proveedor = proveedorService.getProveedor(prenda.getProveedor().getId());
        Imagen imagen = imagenDao.findById(prenda.getImagen().getId()).orElse(null);

        if (prenda == null) {
            return msg.NoGetId();
        }
        return msg.Get(prenda);
    }

    //    @PreAuthorize("hasAnyAuthority('SCOPE_ADMIN')")
    @PostMapping("/prenda")
    public ResponseEntity<?> create(@RequestBody PrendaRequestDto prendaRequestDto) {
        try {
            Imagen imagen = imagenDao.findById(prendaRequestDto.getImagenId()).orElse(null);
            Marca marca = marcaService.getMarca(prendaRequestDto.getMarcaId());
            Categoria categoria = categoriaService.getCategoria(prendaRequestDto.getCategoriaId());
            Proveedor proveedor = proveedorService.getProveedor(prendaRequestDto.getProveedorId());
            Genero genero = generoService.getGenero(prendaRequestDto.getGeneroId());

            Prenda prenda = Prenda.builder()
                    .nombre(prendaRequestDto.getNombre())
                    .descripcion(prendaRequestDto.getDescripcion())
                    .imagen(imagen)
                    .marca(marca)
                    .categoria(categoria)
                    .proveedor(proveedor)
                    .genero(genero)
                    .precio(prendaRequestDto.getPrecio())
                    .activo(prendaRequestDto.getActivo())
                    .build();

            Prenda prendaSave = prendaService.save(prenda);

            return msg.Post(PrendaDto.builder()
                    .id(prendaSave.getId())
                    .nombre(prendaSave.getNombre())
                    .descripcion(prendaSave.getDescripcion())
                    .imagen(imagen)
                    .marca(MarcaDto.builder().id(marca.getId()).nomMarca(marca.getNomMarca()).build())
                    .categoria(CategoriaDto.builder().id(categoria.getId()).nomCategoria(categoria.getNomCategoria()).build())
                    .proveedor(ProveedorDto.builder().id(proveedor.getId()).nomProveedor(proveedor.getNomProveedor()).build())
                    .genero(GeneroDto.builder().id(genero.getId()).nomGenero(genero.getNomGenero()).build())
                    .precio(prendaSave.getPrecio())
                    .activo(prendaSave.getActivo())
                    .createdAt(prendaSave.getCreatedAt())
                    .tallas(prendaSave.getTallas())
                    .build());
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }

    // PUT
    @PutMapping("/prenda/{id}")
    public ResponseEntity<?> update(@PathVariable Long id, @RequestBody PrendaRequestDto prendaRequestDto) {
        try {
            if (prendaService.existsById(id)) {
                Imagen imagen = imagenDao.findById(prendaRequestDto.getImagenId()).orElse(null);
                Marca marca = marcaService.getMarca(prendaRequestDto.getMarcaId());
                Categoria categoria = categoriaService.getCategoria(prendaRequestDto.getCategoriaId());
                Proveedor proveedor = proveedorService.getProveedor(prendaRequestDto.getProveedorId());
                Genero genero = generoService.getGenero(prendaRequestDto.getGeneroId());

                Prenda prenda = Prenda.builder()
                        .id(id)
                        .nombre(prendaRequestDto.getNombre())
                        .descripcion(prendaRequestDto.getDescripcion())
                        .imagen(imagen)
                        .marca(marca)
                        .categoria(categoria)
                        .proveedor(proveedor)
                        .genero(genero)
                        .precio(prendaRequestDto.getPrecio())
                        .activo(prendaRequestDto.getActivo())
                        .build();

                Prenda prendaUpdate = prendaService.save(prenda);

                return msg.Post(PrendaDto.builder()
                        .id(prendaUpdate.getId())
                        .nombre(prendaUpdate.getNombre())
                        .descripcion(prendaUpdate.getDescripcion())
                        .imagen(imagen)
                        .marca(MarcaDto.builder().id(marca.getId()).nomMarca(marca.getNomMarca()).build())
                        .categoria(CategoriaDto.builder().id(categoria.getId()).nomCategoria(categoria.getNomCategoria()).build())
                        .proveedor(ProveedorDto.builder().id(proveedor.getId()).nomProveedor(proveedor.getNomProveedor()).build())
                        .genero(GeneroDto.builder().id(genero.getId()).nomGenero(genero.getNomGenero()).build())
                        .precio(prendaUpdate.getPrecio())
                        .activo(prendaUpdate.getActivo())
                        .createdAt(prendaUpdate.getCreatedAt())
                        .tallas(prendaUpdate.getTallas())
                        .build());
            } else {
                return msg.NoPut();
            }
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }


    //    @PreAuthorize("hasAnyAuthority('SCOPE_ADMIN')")
    @DeleteMapping("/prenda/{id}")
    public ResponseEntity<?> delete(@PathVariable("id") Long id) {
        try {
            Prenda prendaDelete = prendaService.getPrenda(id);
            if (prendaDelete != null) {
                prendaService.deletePrenda(prendaDelete);
                return msg.Delete(prendaDelete);
            }
            return msg.NoGetId();
        } catch (DataAccessException e) {
            return msg.Error(e);
        }
    }

//    @GetMapping("/prendas/con-descuentos")
//    public ResponseEntity<?> obtenerPrendasConDescuentos() {
//        try {
//            List<PrendaConDescuentoResponseDto> prendasConDescuentos = prendaService.obtenerPrendasConDescuentos();
//            if (prendasConDescuentos.isEmpty()) {
//                return msg.NoGet();
//            }
//            return msg.Get(prendasConDescuentos);
//        } catch (DataAccessException e) {
//            return msg.Error(e);
//        }
//    }

}
