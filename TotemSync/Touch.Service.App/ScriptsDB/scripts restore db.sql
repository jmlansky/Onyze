truncate tipo_atributo cascade;
truncate categoria cascade;
truncate tipo_articulo cascade;
truncate fabricante cascade;
truncate atributo cascade;
truncate articulo cascade;
truncate codigo_barra cascade;
truncate provincia cascade;
truncate localidad cascade;
truncate zona cascade;
truncate barrio cascade;
truncate tipo_archivo cascade;

insert into fabricante (nombre, creado, eliminado) 
values
	('Abbott Laboratories S.A.',current_timestamp, false),
	('Bayer S.A.', current_timestamp, false),
	('Biotenk', current_timestamp, false),
	('DroFAr', current_timestamp, false),
	('Elisium S.A.', current_timestamp, false),
	('Laboratorios Bagó S.A.', current_timestamp, false),
	('Laboratorios Roemmers', current_timestamp, false),
	('Novartis Argentina S.A.', current_timestamp, false),
	('Pizer', current_timestamp, false),
	('Productos Roche S.A.Q.e I.', current_timestamp, false),
	('Otros', current_timestamp, false);

insert into tipo_atributo (nombre, creado, eliminado) 
values
	('Color', current_timestamp, false),
	('Fragancia', current_timestamp, false),
	('Sabor', current_timestamp, false),
	('Talle', current_timestamp, false);
		
insert into categoria (nombre, creado, eliminado) 
values 
	('Medicamento', current_timestamp, false),
	('Tintura para cabello', current_timestamp, false),
	('Venta libre', current_timestamp, false),
	('Pintura para uñas', current_timestamp, false),
	('Limpieza facial', current_timestamp, false),
	('Varios', current_timestamp, false);
		
insert into tipo_articulo (nombre, creado, eliminado) 
values 
	('Pastilla', current_timestamp, false),
	('Aerosol', current_timestamp, false),
	('Líquido', current_timestamp, false),
	('Cuidado personal', current_timestamp, false),
	('Varios', current_timestamp, false);
		
insert into atributo (nombre, creado, eliminado, id_tipo) 
values
	('Rojo', current_timestamp, false, (select id from tipo_atributo where nombre = 'Color')),
	('Verde', current_timestamp, false, (select id from tipo_atributo where nombre = 'Color')),
	('Azul', current_timestamp, false, (select id from tipo_atributo where nombre = 'Color')),
	('Florar', current_timestamp, false, (select id from tipo_atributo where nombre = 'Fragancia')),
	('Aromático', current_timestamp, false, (select id from tipo_atributo where nombre = 'Fragancia')),
	('Citrico', current_timestamp, false, (select id from tipo_atributo where nombre = 'Fragancia')),
	('Menta', current_timestamp, false, (select id from tipo_atributo where nombre = 'Sabor')),
	('Frutilla', current_timestamp, false, (select id from tipo_atributo where nombre = 'Sabor')),
	('Small', current_timestamp, false, (select id from tipo_atributo where nombre = 'Talle')),
	('Medium', current_timestamp, false, (select id from tipo_atributo where nombre = 'Talle')),
	('Large', current_timestamp, false, (select id from tipo_atributo where nombre = 'Talle')),
	('Extra Large', current_timestamp, false, (select id from tipo_atributo where nombre = 'Talle'));
		
insert into articulo (nombre, sku, precio, creado, eliminado, id_tipo, id_fabricante) 
values
	('Ibuprofeno 600', 'IBU600BAGO', 250, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Pastilla'),
		(select id from fabricante where nombre = 'Laboratorios Bagó S.A.')),
	('Ibuprofeno 800', 'IBU800BAGO', 300, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Pastilla'),
		(select id from fabricante where nombre = 'Laboratorios Bagó S.A.')),
	('Ibuprofeno 1000', 'IBU1000BAGO', 350, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Pastilla'),
		(select id from fabricante where nombre = 'Laboratorios Bagó S.A.')),
	('Ibuprofeno 600', 'IBU600BAYER', 260, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Pastilla'),
		(select id from fabricante where nombre = 'Bayer S.A.')),
	('Ibuprofeno 800', 'IBU800BAYER', 310, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Pastilla'),
		(select id from fabricante where nombre = 'Bayer S.A.')),
	('Ibuprofeno 1000', 'IBU1000BAYER', 360, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Pastilla'),
		(select id from fabricante where nombre = 'Bayer S.A.')),
	('Tintura azul para cabello', 'TINTURA', 600, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Cuidado personal'),
		(select id from fabricante where nombre = 'Otros')),
	('Esmalte para uñas blanco', 'ESMBCO', 620, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Cuidado personal'),
		(select id from fabricante where nombre = 'Otros')),
	('Coltix', 'COLROEMMERS', 450, current_timestamp, false, 
		(select id from tipo_articulo where nombre = 'Líquido'),
		(select id from fabricante where nombre = 'Laboratorios Bagó S.A.'));

insert into atributos_articulos (id_articulo, id_atributo, creado, eliminado)
values
	((select id from articulo where sku = 'IBU600BAGO'), (select id from atributo where nombre = 'Small'), current_timestamp, false),
	((select id from articulo where sku = 'IBU600BAGO'), (select id from atributo where nombre = 'Rojo'), current_timestamp, false),
	((select id from articulo where sku = 'IBU800BAGO'), (select id from atributo where nombre = 'Medium'), current_timestamp, false),
	((select id from articulo where sku = 'IBU800BAGO'), (select id from atributo where nombre = 'Verde'), current_timestamp, false),
	((select id from articulo where sku = 'IBU1000BAGO'), (select id from atributo where nombre = 'Large'), current_timestamp, false),
	((select id from articulo where sku = 'IBU1000BAGO'), (select id from atributo where nombre = 'Azul'), current_timestamp, false),
	((select id from articulo where sku = 'IBU600BAYER'), (select id from atributo where nombre = 'Small'), current_timestamp, false),
	((select id from articulo where sku = 'IBU600BAYER'), (select id from atributo where nombre = 'Rojo'), current_timestamp, false),
	((select id from articulo where sku = 'IBU800BAYER'), (select id from atributo where nombre = 'Medium'), current_timestamp, false),
	((select id from articulo where sku = 'IBU800BAYER'), (select id from atributo where nombre = 'Verde'), current_timestamp, false),
	((select id from articulo where sku = 'IBU1000BAYER'), (select id from atributo where nombre = 'Large'), current_timestamp, false),
	((select id from articulo where sku = 'IBU1000BAYER'), (select id from atributo where nombre = 'Azul'), current_timestamp, false),
	((select id from articulo where sku = 'COLROEMMERS'), (select id from atributo where nombre = 'Small'), current_timestamp, false),
	((select id from articulo where sku = 'ESMBCO'), (select id from atributo where nombre = 'Small'), current_timestamp, true);

insert into categorias_articulos (id_articulo, id_categoria)
values
	((select id from articulo where sku = 'IBU600BAGO'), (select id from categoria where nombre = 'Medicamento')),
	((select id from articulo where sku = 'IBU600BAGO'), (select id from categoria where nombre = 'Venta libre')),
	((select id from articulo where sku = 'IBU800BAGO'), (select id from categoria where nombre = 'Medicamento')),
	((select id from articulo where sku = 'IBU800BAGO'), (select id from categoria where nombre = 'Venta libre')),
	((select id from articulo where sku = 'IBU1000BAGO'), (select id from categoria where nombre = 'Medicamento')),
	((select id from articulo where sku = 'IBU600BAYER'), (select id from categoria where nombre = 'Medicamento')),
	((select id from articulo where sku = 'IBU600BAYER'), (select id from categoria where nombre = 'Venta libre')),
	((select id from articulo where sku = 'IBU800BAYER'), (select id from categoria where nombre = 'Medicamento')),
	((select id from articulo where sku = 'IBU800BAYER'), (select id from categoria where nombre = 'Venta libre')),
	((select id from articulo where sku = 'IBU1000BAYER'), (select id from categoria where nombre = 'Medicamento')),
	((select id from articulo where sku = 'TINTURA'), (select id from categoria where nombre = 'Tintura para cabello')),
	((select id from articulo where sku = 'ESMBCO'), (select id from categoria where nombre = 'Pintura para uñas')),
	((select id from articulo where sku = 'COLROEMMERS'), (select id from categoria where nombre = 'Medicamento')),
	((select id from articulo where sku = 'COLROEMMERS'), (select id from categoria where nombre = 'Venta libre'));

insert into codigo_barra (ean, creado, eliminado, id_articulo)
values
	('BAGO600', current_timestamp, false, (select id from articulo where sku = 'IBU600BAGO')),
	('BAGO800', current_timestamp, false, (select id from articulo where sku = 'IBU800BAGO')),
	('BAGO1000', current_timestamp, false, (select id from articulo where sku = 'IBU1000BAGO')),
	('BAGO1000', current_timestamp, true, (select id from articulo where sku = 'IBU1000BAGO')),
	('BAYER600', current_timestamp, false, (select id from articulo where sku = 'IBU600BAYER')),
	('BAYER800', current_timestamp, false, (select id from articulo where sku = 'IBU800BAYER')),
	('BAYER1000', current_timestamp, false, (select id from articulo where sku = 'IBU1000BAYER')),
	('TINT500', current_timestamp, false, (select id from articulo where sku = 'TINTURA')),
	('ESMALTE100', current_timestamp, false, (select id from articulo where sku = 'ESMBCO')),
	('COLTIX120ML', current_timestamp, false, (select id from articulo where sku = 'COLROEMMERS'));

insert into provincia (nombre, modificado, eliminado)
values
	('Córdoba', current_timestamp, false),
	('Buenos Aires', current_timestamp, false),
	('Chubut', current_timestamp, false),
	('Santa Fe', current_timestamp, false),
	('Entre Rios', current_timestamp, false),
	('Tierra del Fuego', current_timestamp, false),
	('Tucuman', current_timestamp, false);

insert into localidad (nombre, creado, eliminado, id_provincia)
values
	('Santa Fe', current_timestamp, false, (select id from provincia where nombre = 'Santa Fe')),
	('Rafaela', current_timestamp, false, (select id from provincia where nombre = 'Santa Fe')),
	('Córdoba', current_timestamp, false, (select id from provincia where nombre = 'Córdoba')),
	('Trelew', current_timestamp, false, (select id from provincia where nombre = 'Chubut')),
	('CABA', current_timestamp, false, (select id from provincia where nombre = 'Buenos Aires')),
	('Parana', current_timestamp, false, (select id from provincia where nombre = 'Entre Rios')),
	('Ushuaia', current_timestamp, false, (select id from provincia where nombre = 'Tierra del Fuego')),
	('Tucuman', current_timestamp, false, (select id from provincia where nombre = 'Tucuman')),
	('Villa María', current_timestamp, false, (select id from provincia where nombre = 'Córdoba')),
	('Rosario', current_timestamp, false, (select id from provincia where nombre = 'Santa Fe'));

insert into zona (nombre, creado, eliminado)
values
	('Norte', current_timestamp, false),
	('Sur', current_timestamp, false),
	('Este', current_timestamp, false),
	('Oeste', current_timestamp, false);

insert into barrio (nombre, codigo_postal, creado, eliminado, id_localidad)
values
	('Centro', '5000', current_timestamp, false, (select id from localidad where nombre = 'Córdoba')),
	('Centro', '2000', current_timestamp, false, (select id from localidad where nombre = 'Santa Fe')),
	('Centro', '2300', current_timestamp, false, (select id from localidad where nombre = 'Rafaela')),
	('Ilolay', '2300', current_timestamp, false, (select id from localidad where nombre = 'Rafaela')),
	('Centro', '9100', current_timestamp, false, (select id from localidad where nombre = 'Trelew')),
	('Centro', '9410', current_timestamp, false, (select id from localidad where nombre = 'Ushuaia')),
	('General Paz', '5000', current_timestamp, false, (select id from localidad where nombre = 'Córdoba')),
	('Villa Allende', '5000', current_timestamp, false, (select id from localidad where nombre = 'Córdoba')),
	('Guemes', '5000', current_timestamp, false, (select id from localidad where nombre = 'Córdoba')),
	('Brigadier Lopez', '2300', current_timestamp, false, (select id from localidad where nombre = 'Rafaela'));

insert into tipo_archivo(nombre, creado, eliminado)
values
	('Imagen 4k', current_timestamp, false),
	('Imagen miniatura', current_timestamp, false),
	('Video', current_timestamp, false);