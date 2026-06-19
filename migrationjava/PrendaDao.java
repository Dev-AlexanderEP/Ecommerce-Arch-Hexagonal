package com.ecommerce.server.model.dao.prenda;

import com.ecommerce.server.model.dto.descuento.PrendaConDescuentoResponseDto;
import com.ecommerce.server.model.dto.descuento.PrendaConDescuentoResponseTodoDto;
import com.ecommerce.server.model.entity.prenda.Prenda;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.CrudRepository;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;
public interface PrendaDao extends CrudRepository<Prenda, Long> {
    @Query("SELECT p, dp.porcentaje, dp.activo " +
            "FROM Prenda p LEFT JOIN DescuentoPrenda dp ON p.id = dp.prenda.id " +
            "WHERE p.activo = true AND (dp.activo = true OR dp IS NULL) " +
            "AND (dp.fechaInicio <= CURRENT_DATE AND (dp.fechaFin IS NULL OR dp.fechaFin >= CURRENT_DATE) OR dp IS NULL)")
    List<Object[]> findPrendasConDescuentosActivos();

    @Query("SELECT DISTINCT pt.talla.nomTalla " +
           "FROM Prenda p " +
           "JOIN p.tallas pt " +
           "JOIN pt.talla t " +
           "JOIN p.categoria c " +
           "WHERE p.activo = true " +
           "AND LOWER(c.nomCategoria) LIKE LOWER(CONCAT('%', :categoria, '%')) " +
           "ORDER BY t.nomTalla")
    List<String> findTallasByCategoriaLike(@Param("categoria") String categoria);

    @Query("SELECT DISTINCT t.nomTalla " +
            "FROM Prenda p " +
            "JOIN p.tallas pt " +
            "JOIN pt.talla t " +
            "JOIN p.genero g " +
            "WHERE p.activo = true " +
            "AND LOWER(g.nomGenero) LIKE LOWER(CONCAT('%', :genero, '%')) " +
            "ORDER BY t.nomTalla")
    List<String> findTallasByGeneroLike(@Param("genero") String genero);

    @Query("SELECT DISTINCT m.nomMarca " +
           "FROM Prenda p " +
           "JOIN p.marca m " +
           "JOIN p.categoria c " +
           "WHERE LOWER(c.nomCategoria) LIKE LOWER(CONCAT('%', :categoria, '%')) " +
           "ORDER BY m.nomMarca")
    List<String> findMarcasByCategoriaLike(@Param("categoria") String categoria);


    @Query("SELECT DISTINCT m.nomMarca " +
            "FROM Prenda p " +
            "JOIN p.marca m " +
            "JOIN p.genero g " +
            "WHERE LOWER(g.nomGenero) LIKE LOWER(CONCAT('%', :genero, '%')) " +
            "ORDER BY m.nomMarca")
    List<String> findMarcasByGeneroLike(@Param("genero") String genero);

    @Query("SELECT MIN(p.precio), ROUND(AVG(p.precio), 2), MAX(p.precio) " +
           "FROM Prenda p " +
           "JOIN p.categoria c " +
           "WHERE LOWER(c.nomCategoria) LIKE LOWER(CONCAT('%', :categoria, '%'))")
    Object[] findEstadisticasPreciosByCategoriaLike(@Param("categoria") String categoria);

    @Query("SELECT MIN(p.precio), ROUND(AVG(p.precio), 2), MAX(p.precio) " +
            "FROM Prenda p " +
            "JOIN p.genero g " +
            "WHERE LOWER(g.nomGenero) LIKE LOWER(CONCAT('%', :genero, '%'))")
    Object[] findEstadisticasPreciosByGeneroLike(@Param("genero") String genero);

    @Query(value = "SELECT DISTINCT COALESCE(dp.porcentaje, dc.porcentaje) AS descuento " +
            "FROM prenda p " +
            "JOIN categoria c ON c.id = p.categoria_id " +
            "LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id " +
            "    AND dp.activo = true " +
            "    AND dp.fecha_inicio <= CURRENT_DATE " +
            "    AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE) " +
            "LEFT JOIN descuento_categoria dc ON dc.categoria_id = c.id " +
            "    AND dc.activo = true " +
            "    AND dc.fecha_inicio <= CURRENT_DATE " +
            "    AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE) " +
            "WHERE p.activo = true " +
            "AND LOWER(c.nom_categoria) LIKE LOWER(CONCAT('%', :categoria, '%')) " +
            "AND (dp.porcentaje IS NOT NULL OR dc.porcentaje IS NOT NULL)",
            nativeQuery = true)
    List<Double> findSoloDescuentosPorCategoria(@Param("categoria") String categoria);


    @Query(value = "SELECT DISTINCT COALESCE(dp.porcentaje, dc.porcentaje) AS descuento " +
            "FROM prenda p " +
            "JOIN genero g ON g.id = p.genero_id " +
            "LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id " +
            "    AND dp.activo = true " +
            "    AND dp.fecha_inicio <= CURRENT_DATE " +
            "    AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE) " +
            "LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id " +
            "    AND dc.activo = true " +
            "    AND dc.fecha_inicio <= CURRENT_DATE " +
            "    AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE) " +
            "WHERE p.activo = true " +
            "AND LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', :genero, '%')) " +
            "AND (dp.porcentaje IS NOT NULL OR dc.porcentaje IS NOT NULL)",
            nativeQuery = true)
    List<Double> findSoloDescuentosPorGenero(@Param("genero") String genero);
    // categoria por genero
    @Query("SELECT DISTINCT c.nomCategoria " +
            "FROM Prenda p " +
            "JOIN p.categoria c " +
            "JOIN p.genero g " +
            "WHERE LOWER(g.nomGenero) LIKE LOWER(CONCAT('%', :genero, '%')) " +
            "ORDER BY c.nomCategoria")
    List<String> findCategoriasByGeneroLike(@Param("genero") String genero);


    @Query("SELECT p FROM Prenda p JOIN p.categoria c WHERE LOWER(c.nomCategoria) LIKE LOWER(CONCAT('%', :categoria, '%'))")
    List<Prenda> findPrendasByCategoriaLike(@Param("categoria") String categoria);

    @Query(value = "SELECT " +
            "p.id, " +
            "p.nombre, " +
            "p.precio, " +
            "i.principal AS imagen_principal, " +
            "i.hover AS imagen_hover, " +
            "m.nom_marca AS marca, " +
            "COALESCE(dp.porcentaje, dc.porcentaje, 0) AS descuento_aplicado, " +
            "CASE " +
            "    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda' " +
            "    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria' " +
            "    ELSE 'sin_descuento' " +
            "END AS tipo_descuento " +
            "FROM prenda p " +
            "JOIN categoria c ON c.id = p.categoria_id " +
            "JOIN marca m ON m.id = p.marca_id " +
            "JOIN genero g ON g.id = p.genero_id " +
            "LEFT JOIN imagen i ON i.id = p.imagen_id " +
            "LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id " +
            "    AND dp.activo = true " +
            "    AND dp.fecha_inicio <= CURRENT_DATE " +
            "    AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE) " +
            "LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id " +
            "    AND dc.activo = true " +
            "    AND dc.fecha_inicio <= CURRENT_DATE " +
            "    AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE) " +
            "WHERE p.activo = true " +
            "AND (:categoria IS NULL OR LOWER(c.nom_categoria) LIKE LOWER(CONCAT('%', :categoria, '%'))) " +
            "AND (:genero IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', :genero, '%')))",
            nativeQuery = true)
    List<PrendaConDescuentoResponseDto> findPrendasConDescuentoAplicadoPorCategoriaYGenero(
            @Param("categoria") String categoria,
            @Param("genero") String genero
    );

    @Query(value = "SELECT " +
            "p.id, " +
            "p.nombre, " +
            "p.precio, " +
            "i.principal AS imagen_principal, " +
            "i.hover AS imagen_hover, " +
            "m.nom_marca AS marca, " +
            "COALESCE(dp.porcentaje, dc.porcentaje, 0) AS descuento_aplicado, " +
            "CASE " +
            "    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda' " +
            "    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria' " +
            "    ELSE 'sin_descuento' " +
            "END AS tipo_descuento, " +
            "c.nom_categoria AS categoria " +
            "FROM prenda p " +
            "JOIN categoria c ON c.id = p.categoria_id " +
            "JOIN marca m ON m.id = p.marca_id " +
            "JOIN genero g ON g.id = p.genero_id " +
            "LEFT JOIN imagen i ON i.id = p.imagen_id " +
            "LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id AND dp.activo = true " +
            "LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id AND dc.activo = true " +
            "WHERE p.activo = true " +
            "AND (:genero IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', :genero, '%'))) " +
            "ORDER BY RANDOM()",
            nativeQuery = true)
    List<PrendaConDescuentoResponseTodoDto> findPrendasConDescuentoAplicadoPorGeneroAleatorio(
            @Param("genero") String genero
    );


    @Query(value = "SELECT " +
            "p.id, " +
            "p.nombre, " +
            "p.precio, " +
            "i.principal AS imagen_principal, " +
            "i.hover AS imagen_hover, " +
            "m.nom_marca AS marca, " +
            "COALESCE(dp.porcentaje, dc.porcentaje, 0) AS descuento_aplicado, " +
            "CASE " +
            "    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda' " +
            "    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria' " +
            "    ELSE 'sin_descuento' " +
            "END AS tipo_descuento " +
            "FROM prenda p " +
            "JOIN categoria c ON c.id = p.categoria_id " +
            "JOIN marca m ON m.id = p.marca_id " +
            "JOIN genero g ON g.id = p.genero_id " +
            "LEFT JOIN imagen i ON i.id = p.imagen_id " +
            "LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id " +
            "    AND dp.activo = true " +
            "    AND dp.fecha_inicio <= CURRENT_DATE " +
            "    AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE) " +
            "LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id " +
            "    AND dc.activo = true " +
            "    AND dc.fecha_inicio <= CURRENT_DATE " +
            "    AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE) " +
            "WHERE p.activo = true " +
            "AND (:categoria IS NULL OR LOWER(c.nom_categoria) LIKE LOWER(CONCAT('%', :categoria, '%'))) " +
            "AND (:genero IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', :genero, '%'))) " +
            "AND (:nombre IS NULL OR LOWER(p.nombre) LIKE LOWER(CONCAT('%', :nombre, '%')))",
            nativeQuery = true)
    List<PrendaConDescuentoResponseDto> buscarPrendasConDescuentoPorNombreCategoriaYGenero(
            @Param("nombre") String nombre,
            @Param("categoria") String categoria,
            @Param("genero") String genero
    );

    @Query(value = "SELECT " +
            "p.id, " +
            "p.nombre, " +
            "p.precio, " +
            "i.principal AS imagen_principal, " +
            "i.hover AS imagen_hover, " +
            "m.nom_marca AS marca, " +
            "COALESCE(dp.porcentaje, dc.porcentaje, 0) AS descuento_aplicado, " +
            "CASE " +
            "    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda' " +
            "    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria' " +
            "    ELSE 'sin_descuento' " +
            "END AS tipo_descuento " +
            "FROM prenda p " +
            "JOIN categoria c ON c.id = p.categoria_id " +
            "JOIN marca m ON m.id = p.marca_id " +
            "JOIN genero g ON g.id = p.genero_id " +
            "LEFT JOIN imagen i ON i.id = p.imagen_id " +
            "LEFT JOIN descuento_prenda dp ON dp.prenda_id = p.id " +
            "    AND dp.activo = true " +
            "    AND dp.fecha_inicio <= CURRENT_DATE " +
            "    AND (dp.fecha_fin IS NULL OR dp.fecha_fin >= CURRENT_DATE) " +
            "LEFT JOIN descuento_categoria dc ON dc.categoria_id = p.categoria_id " +
            "    AND dc.activo = true " +
            "    AND dc.fecha_inicio <= CURRENT_DATE " +
            "    AND (dc.fecha_fin IS NULL OR dc.fecha_fin >= CURRENT_DATE) " +
            "WHERE p.activo = true " +
            "AND (:genero IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', :genero, '%'))) " +
            "AND (:nombre IS NULL OR LOWER(p.nombre) LIKE LOWER(CONCAT('%', :nombre, '%')))",
            nativeQuery = true)
    List<PrendaConDescuentoResponseDto> buscarPrendasConDescuentoPorNombreYGenero(
            @Param("nombre") String nombre,
            @Param("genero") String genero
    );

@Query(value = "SELECT " +
        "p.id, " +
        "p.nombre, " +
        "p.precio, " +
        "i.principal AS imagen_principal, " +
        "i.hover AS imagen_hover, " +
        "m.nom_marca AS marca, " +
        "COALESCE(dp.porcentaje, dc.porcentaje, 0) AS descuento_aplicado, " +
        "CASE " +
        "    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda' " +
        "    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria' " +
        "    ELSE 'sin_descuento' " +
        "END AS tipo_descuento " +
        "FROM prenda p " +
        "JOIN marca m ON p.marca_id = m.id " +
        "JOIN categoria c ON p.categoria_id = c.id " +
        "LEFT JOIN imagen i ON p.imagen_id = i.id " +
        "LEFT JOIN descuento_prenda dp ON p.id = dp.prenda_id AND dp.activo = true " +
        "LEFT JOIN descuento_categoria dc ON p.categoria_id = dc.categoria_id AND dc.activo = true " +
        "JOIN prenda_talla pt ON pt.prenda_id = p.id " +
        "JOIN talla t ON pt.talla_id = t.id " +
        "WHERE p.activo = true " +
        "AND (:talla IS NULL OR EXISTS (" +
        "    SELECT 1 FROM prenda_talla pt " +
        "    JOIN talla t ON pt.talla_id = t.id " +
        "    WHERE pt.prenda_id = p.id AND t.nom_talla = :talla" +
        ")) " +
        "AND (:categoria IS NULL OR c.nom_categoria = :categoria) " +
        "AND (:marca IS NULL OR m.nom_marca = :marca) " +
        "AND ((:precioMin IS NULL OR :precioMax IS NULL) OR (p.precio BETWEEN :precioMin AND :precioMax)) " +
        "AND ((:descMin IS NULL OR :descMax IS NULL) OR (COALESCE(dp.porcentaje, dc.porcentaje, 0) BETWEEN :descMin AND :descMax)) " +
        "ORDER BY p.nombre",
        nativeQuery = true)
List<PrendaConDescuentoResponseDto> filtrarPrendasDinamico(
        @Param("talla") String talla,
        @Param("categoria") String categoria,
        @Param("marca") String marca,
        @Param("precioMin") Double precioMin,
        @Param("precioMax") Double precioMax,
        @Param("descMin") Double descMin,
        @Param("descMax") Double descMax
);


    @Query(value = "SELECT  " +
            "p.id, " +
            "p.nombre, " +
            "p.precio, " +
            "i.principal AS imagen_principal, " +
            "i.hover AS imagen_hover, " +
            "m.nom_marca AS marca, " +
            "COALESCE(dp.porcentaje, dc.porcentaje, 0) AS descuento_aplicado, " +
            "CASE " +
            "    WHEN dp.porcentaje IS NOT NULL THEN 'descuento_prenda' " +
            "    WHEN dc.porcentaje IS NOT NULL THEN 'descuento_categoria' " +
            "    ELSE 'sin_descuento' " +
            "END AS tipo_descuento " +
            "FROM prenda p " +
            "JOIN marca m ON p.marca_id = m.id " +
            "JOIN categoria c ON p.categoria_id = c.id " +
            "JOIN genero g ON p.genero_id = g.id " +
            "LEFT JOIN imagen i ON p.imagen_id = i.id " +
            "LEFT JOIN descuento_prenda dp ON p.id = dp.prenda_id AND dp.activo = true " +
            "LEFT JOIN descuento_categoria dc ON p.categoria_id = dc.categoria_id AND dc.activo = true " +
            "JOIN prenda_talla pt ON pt.prenda_id = p.id " +
            "JOIN talla t ON pt.talla_id = t.id " +
            "WHERE p.activo = true " +
            "AND (:talla IS NULL OR EXISTS (" +
            "    SELECT 1 FROM prenda_talla pt " +
            "    JOIN talla t ON pt.talla_id = t.id " +
            "    WHERE pt.prenda_id = p.id AND t.nom_talla = :talla" +
            ")) " +
            "AND (:categoria IS NULL OR c.nom_categoria = :categoria) " +
            "AND (:marca IS NULL OR m.nom_marca = :marca) " +
            "AND (:genero IS NULL OR LOWER(g.nom_genero) LIKE LOWER(CONCAT('%', :genero, '%'))) " +
            "AND ((:precioMin IS NULL OR :precioMax IS NULL) OR (p.precio BETWEEN :precioMin AND :precioMax)) " +
            "AND ((:descMin IS NULL OR :descMax IS NULL) OR (COALESCE(dp.porcentaje, dc.porcentaje, 0) BETWEEN :descMin AND :descMax)) " +
            "ORDER BY p.nombre",
            nativeQuery = true)
    List<PrendaConDescuentoResponseDto> filtrarPrendasDinamico(
            @Param("talla") String talla,
            @Param("categoria") String categoria,
            @Param("marca") String marca,
            @Param("genero") String genero,
            @Param("precioMin") Double precioMin,
            @Param("precioMax") Double precioMax,
            @Param("descMin") Double descMin,
            @Param("descMax") Double descMax
    );
    Page<Prenda> findAll(Pageable pageable);

}
