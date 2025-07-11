﻿using AutoMapper; //permite convertir (mapear) objetos entre clases (por ejemplo, convertir un `Helado` en un `HeladoDto`, o viceversa) de forma automática.
using HeladeriaAPI.Models.Categoria;
using HeladeriaAPI.Models.Categoria.Dto;
using HeladeriaAPI.Models.Estado;

using HeladeriaAPI.Models.Estado.Dto;

//Importa los DTOs que vas a usar para definir los mapeos.
//Importa los modelos reales de base de datos que se van a mapear hacia/desde los DTOs.
using HeladeriaAPI.Models.Helado;
using HeladeriaAPI.Models.Helado.Dto;
using HeladeriaAPI.Models.Ingrediente;
using HeladeriaAPI.Models.Ingrediente.Dto;

namespace HeladeriaAPI.Config
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            // Para no convertir los atributos 'int?' a 0 en la conversion de los 'null'
            // valor defecto int -> 0
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);

            // Para los precios utilizamos el mismo concepto.
            // valor defecto double -> 0
            CreateMap<double?, double>().ConvertUsing((src, dest) => src ?? dest);

            // Aqui es necesario hacer esto con bool? ya que tampoco devuelve el tipo 'null'.
            // valor defecto bool -> false
            CreateMap<bool?, bool>().ConvertUsing((src, dest) => src ?? dest);

            // Lo mismo con las listas.
            // valor defecto List -> []
            CreateMap<List<string>?, List<string>>().ConvertUsing((src, dest) => src ?? dest);

            //PD: Esta solución hay que aplicarla para todos aquellos tipos que no tengan como valor por defecto 'null'

            CreateMap<Helado, AllHeladoDTO>()
                 .ForMember(dest => dest.nombreCategoria, opt => opt.MapFrom(src => src.Categoria.nombreCategoria))
                 .ForMember(dest => dest.nombreEstado, opt => opt.MapFrom(src => src.Estado.nombreEstado))
                 .ForMember(dest => dest.Ingredientes, opt => opt.MapFrom(src => src.IngredienteHelado.Select(ih => ih.Ingrediente.nombreIngrediente).ToList()))
                 .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                 .ForMember(dest => dest.IsArtesanal, opt => opt.MapFrom(src => src.IsArtesanal));





            CreateMap<CreateHeladoDTO, Helado>().ReverseMap();

            // Actualizar y no parsear los valores 'NULL'
            // NO PONER 'ReverseMap()' ya que no queremos convertir de Helado a UpdateHeladoDTO y no va a funcionar la condición de los miembros.
            CreateMap<UpdateHeladoDTO, Helado>()
                .ForAllMembers(opts =>
                {
                    opts.Condition((src, dest, srcMember) => srcMember != null);
                });


            // Ingrediente mappings
            CreateMap<CreateIngredienteDTO, Ingrediente>().ReverseMap();
            CreateMap<UpdateIngredienteDTO, Ingrediente>()
                .ForAllMembers(opts =>
                {
                    opts.Condition((src, dest, srcMember) => srcMember != null);
                });

            CreateMap<CreateCategoriaDTO, Categoria>().ReverseMap();

            CreateMap<UpdateCategoriaDTO, Categoria>()
                .ForAllMembers(opts =>
                {
                    opts.Condition((src, dest, srcMember) => srcMember != null);
                });

            CreateMap<Categoria, AllCategoriaDTO>()
                .ForMember(dest => dest.nombreCategoria, opt => opt.MapFrom(src => src.nombreCategoria));

            CreateMap<Estado, AllEstadoDTO>()
                .ForMember(dest => dest.nombreEstado, opt => opt.MapFrom(src => src.nombreEstado));


            CreateMap<IngredienteHelado, AllIngredienteHeladoDTO>()
                .ForMember(dest => dest.nombreHelado, opt => opt.MapFrom(src => src.Helado.nombreHelado))
                .ForMember(dest => dest.Ingredientes, opt => opt.MapFrom(src => src.Ingrediente.nombreIngrediente));


        }
    }
}
